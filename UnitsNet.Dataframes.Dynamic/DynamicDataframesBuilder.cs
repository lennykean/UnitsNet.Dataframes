using System.Collections;
using System.Globalization;
using System.Linq.Expressions;

using Castle.DynamicProxy;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Reflection;
using UnitsNet.Dataframes.Utils;

namespace UnitsNet.Dataframes.Dynamic;

public class DynamicDataframesBuilder<TDataframe, TMetadataAttribute, TMetadata>
    where TDataframe : class
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IClonableMetadata
{
    private readonly IEnumerable _dataframes;
    private readonly DynamicMetadataProvider<TMetadataAttribute, TMetadata> _dynamicMetadataProvider;

    public DynamicDataframesBuilder(IEnumerable<TDataframe> dataframes, CultureInfo? culture = null)
    {
        _dataframes = dataframes ?? throw new ArgumentNullException(nameof(dataframes));
        _dynamicMetadataProvider = new(typeof(TDataframe), dataframes.GetDataframesMetadata<TDataframe, TMetadataAttribute, TMetadata>(culture));
    }

    private DynamicDataframesBuilder(IEnumerable dataframes, DynamicMetadataProvider<TMetadataAttribute, TMetadata> dynamicMetadataProvider)
    {
        _dataframes = dataframes;
        _dynamicMetadataProvider = dynamicMetadataProvider;
    }

    public DynamicDataframesBuilder<TDataframe, TMetadataAttribute, TMetadata> WithConversion(string propertyName, Enum to)
    {
        var property = typeof(TDataframe).GetProperties(inherit: typeof(TDataframe).IsInterface).Single(p => p.Name == propertyName);
        _dynamicMetadataProvider.AddConversion(property, to);

        return this;
    }

    public DynamicDataframesBuilder<TDataframe, TMetadataAttribute, TMetadata> WithConversion(Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, Enum to)
    {
        var property = propertySelectorExpression.ExtractProperty();
        _dynamicMetadataProvider.AddConversion(property, to);

        return this;
    }

    public DynamicDataframesBuilder<TSuperDataframe, TMetadataAttribute, TMetadata> As<TSuperDataframe>() where TSuperDataframe : class
    {
        var hoistedMetadataProvider = _dynamicMetadataProvider.HoistMetadata<TSuperDataframe>();
        return new(_dataframes, hoistedMetadataProvider);
    }

    public IEnumerable<TDataframe> Build()
    {
        var options = new ProxyGenerationOptions();
        options.AddMixinInstance(_dynamicMetadataProvider);

        var proxyGenerator = new ProxyGenerator();
        var interceptor = new DynamicQuantityInterceptor<TMetadataAttribute, TMetadata>(_dynamicMetadataProvider);

        Func<object, TDataframe> createProxy = typeof(TDataframe).IsInterface
            ? dataframe => (TDataframe)proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TDataframe), dataframe, options, interceptor)
            : dataframe => (TDataframe)proxyGenerator.CreateClassProxyWithTarget(typeof(TDataframe), dataframe, options, interceptor);

        var cache = EphemeralValueCache<(IEnumerable dataframes, object dataframe), TDataframe>.Instance;
        foreach (var dataframe in _dataframes)
            yield return cache.GetOrAdd((_dataframes, dataframe), _ => createProxy(dataframe));
    }
}
