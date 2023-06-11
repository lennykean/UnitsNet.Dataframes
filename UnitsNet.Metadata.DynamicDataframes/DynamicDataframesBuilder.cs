using System.Collections;
using System.Globalization;
using System.Linq.Expressions;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Reflection;

namespace UnitsNet.Metadata.DynamicDataframes;

public class DynamicDataframesBuilder<TObject, TMetadataAttribute, TMetadata>
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private readonly CultureInfo? _culture;
    private readonly IEnumerable _dataframes;
    private readonly DynamicDataframeMetadataProvider<TMetadataAttribute, TMetadata> _dynamicMetadataProvider;

    public DynamicDataframesBuilder(IEnumerable<TObject> dataframes, IMetadataProvider<TMetadataAttribute, TMetadata>  baseMetadataProvider, CultureInfo? culture = null)
    {
        _culture = culture;
        _dataframes = dataframes ?? throw new ArgumentNullException(nameof(dataframes));
        _dynamicMetadataProvider = new(baseMetadataProvider);
    }

    private DynamicDataframesBuilder(IEnumerable dataframes, DynamicDataframeMetadataProvider<TMetadataAttribute, TMetadata> dynamicMetadataProvider)
    {
        _dataframes = dataframes;
        _dynamicMetadataProvider = dynamicMetadataProvider;
    }

    public DynamicDataframesBuilder<TObject, TMetadataAttribute, TMetadata> WithConversion(string propertyName, Enum to)
    {
        var property = typeof(TObject).GetProperty(propertyName);
        _dynamicMetadataProvider.AddConversion(property, to, _culture);

        return this;
    }

    public DynamicDataframesBuilder<TObject, TMetadataAttribute, TMetadata> WithConversion(Expression<Func<TObject, QuantityValue>> propertySelectorExpression, Enum to)
    {
        var propertyName = propertySelectorExpression.ExtractPropertyName();
        var property = typeof(TObject).GetProperty(propertyName);

        _dynamicMetadataProvider.AddConversion(property, to, _culture);

        return this;
    }

    public DynamicDataframesBuilder<TSuperDataframe, TMetadataAttribute, TMetadata> As<TSuperDataframe>() where TSuperDataframe : class
    {
        if (!typeof(TSuperDataframe).IsAssignableFrom(typeof(TObject)))
            throw new InvalidOperationException($"{nameof(TObject)} ({typeof(TObject).Name}) is not derived from {typeof(TSuperDataframe).Name}");

        _dynamicMetadataProvider.HoistMetadata<TSuperDataframe, TObject>(_culture);
        return new(_dataframes, _dynamicMetadataProvider);
    }

    public IDynamicDataframeEnumerable<TObject, TMetadataAttribute, TMetadata> Build()
    {
        ((IMetadataProvider<TMetadataAttribute, TMetadata>)_dynamicMetadataProvider).ValidateAllMetadata(typeof(TObject));
        return new DynamicDataframeEnumerable<TObject, TMetadataAttribute, TMetadata>(_dataframes, _dynamicMetadataProvider);
    }
}