using System.Collections;
using System.Globalization;
using System.Linq.Expressions;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Reflection;

namespace UnitsNet.Dataframes.Dynamic;

public class DynamicDataframesBuilder<TDataframe, TMetadataAttribute, TMetadata>
    where TDataframe : class
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
{
    private readonly CultureInfo? _culture;
    private readonly IEnumerable _dataframes;
    private readonly DynamicDataframeMetadataProvider<TMetadataAttribute, TMetadata> _dynamicMetadataProvider;

    public DynamicDataframesBuilder(IEnumerable<TDataframe> dataframes, CultureInfo? culture = null)
    {
        _culture = culture;
        _dataframes = dataframes ?? throw new ArgumentNullException(nameof(dataframes));

        var baseMetadataProvider =
            dataframes as IDataframeMetadataProvider<TMetadataAttribute, TMetadata> ??
            DefaultDataframeMetadataProvider<TMetadataAttribute, TMetadata>.Instance;

        _dynamicMetadataProvider = new(baseMetadataProvider);
    }

    private DynamicDataframesBuilder(IEnumerable dataframes, DynamicDataframeMetadataProvider<TMetadataAttribute, TMetadata> dynamicMetadataProvider)
    {
        _dataframes = dataframes;
        _dynamicMetadataProvider = dynamicMetadataProvider;
    }

    public DynamicDataframesBuilder<TDataframe, TMetadataAttribute, TMetadata> WithConversion(string propertyName, Enum to)
    {
        var property = typeof(TDataframe).GetProperties(inherit: typeof(TDataframe).IsInterface).Single(p => p.Name == propertyName);
        _dynamicMetadataProvider.AddConversion(property, to, _culture);

        return this;
    }

    public DynamicDataframesBuilder<TDataframe, TMetadataAttribute, TMetadata> WithConversion(Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, Enum to)
    {
        var property = propertySelectorExpression.ExtractProperty();
        _dynamicMetadataProvider.AddConversion(property, to, _culture);

        return this;
    }

    public DynamicDataframesBuilder<TSuperDataframe, TMetadataAttribute, TMetadata> As<TSuperDataframe>() where TSuperDataframe : class
    {
        _dynamicMetadataProvider.HoistMetadata<TSuperDataframe, TDataframe>(_culture);
        return new(_dataframes, _dynamicMetadataProvider);
    }

    public IDynamicDataframeEnumerable<TDataframe, TMetadataAttribute, TMetadata> Build()
    {
        ((IDataframeMetadataProvider<TMetadataAttribute, TMetadata>)_dynamicMetadataProvider).ValidateAllMetadata(typeof(TDataframe));
        return new DynamicDataframeEnumerable<TDataframe, TMetadataAttribute, TMetadata>(_dataframes, _dynamicMetadataProvider);
    }
}