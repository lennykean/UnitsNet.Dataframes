using System.Collections;
using System.Linq.Expressions;

using Castle.DynamicProxy;

using UnitsNet.Dataframes.Reflection;
using UnitsNet.Dataframes.Utils;

namespace UnitsNet.Dataframes.Dynamic;

public class DynamicDataframesBuilder<TDataframe, TMetadata> 
    where TDataframe : class
    where TMetadata : QuantityMetadata
{
    private readonly IEnumerable _dataframes;
    private readonly DynamicMetadataProvider<TMetadata> _dynamicMetadataProvider;

    public DynamicDataframesBuilder(IEnumerable<TDataframe> dataframes)
    {
        _dataframes = dataframes ?? throw new ArgumentNullException(nameof(dataframes));
        _dynamicMetadataProvider = new(typeof(TDataframe), (
            from i in dataframes.GetType().GetInterfaces()
            where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            select i.GetGenericArguments().First())
            .Single());
    }

    private DynamicDataframesBuilder(IEnumerable dataframes, DynamicMetadataProvider dynamicMetadataProvider)
    {
        _dataframes = dataframes;
        _dynamicMetadataProvider = dynamicMetadataProvider.HoistMetadata(typeof(TDataframe));
    }

    public DynamicDataframesBuilder<TDataframe> WithConversion(string propertyName, Enum to)
    {
        var property = typeof(TDataframe).GetProperties(inherit: typeof(TDataframe).IsInterface).Single(p => p.Name == propertyName);
        _dynamicMetadataProvider.AddConversion(property, to);

        return this;
    }

    public DynamicDataframesBuilder<TDataframe> WithConversion(Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, Enum to)
    {
        var property = propertySelectorExpression.ExtractProperty();
        _dynamicMetadataProvider.AddConversion(property, to);

        return this;
    }
    
    public DynamicDataframesBuilder<TDataframeInterface> As<TDataframeInterface>() where TDataframeInterface : class
    {
        return new(_dataframes, _dynamicMetadataProvider);
    }

    public IEnumerable<TDataframe> Build()
    {
        var options = new ProxyGenerationOptions();
        options.AddMixinInstance(_dynamicMetadataProvider);

        var proxyGenerator = new ProxyGenerator();
        var interceptor = new DynamicQuantityInterceptor(_dynamicMetadataProvider);

        Func<object, TDataframe> createProxy = typeof(TDataframe).IsInterface
            ? dataframe => (TDataframe)proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TDataframe), dataframe, options, interceptor)
            : dataframe => (TDataframe)proxyGenerator.CreateClassProxyWithTarget(typeof(TDataframe), dataframe, options, interceptor);

        var cache = EphemeralValueCache<(IEnumerable dataframes, object dataframe), TDataframe>.Instance;
        foreach (var dataframe in _dataframes)
            yield return cache.GetOrAdd((_dataframes, dataframe), _ => createProxy(dataframe));
    }
}
