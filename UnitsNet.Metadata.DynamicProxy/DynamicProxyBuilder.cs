using System.Collections;
using System.Globalization;
using System.Linq.Expressions;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Reflection;

namespace UnitsNet.Metadata.DynamicProxy;

public class DynamicProxyBuilder<TObject, TMetadataAttribute, TMetadata>
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private readonly CultureInfo? _culture;
    private readonly IEnumerable _objects;
    private readonly DynamicProxyMetadataProvider<TMetadataAttribute, TMetadata> _metadataProvider;

    public DynamicProxyBuilder(IEnumerable<TObject> objects, IMetadataProvider<TMetadataAttribute, TMetadata> baseMetadataProvider, CultureInfo? culture = null)
    {
        _culture = culture;
        _objects = objects ?? throw new ArgumentNullException(nameof(objects));
        _metadataProvider = new(baseMetadataProvider);
    }

    private DynamicProxyBuilder(IEnumerable objects, DynamicProxyMetadataProvider<TMetadataAttribute, TMetadata> dynamicMetadataProvider)
    {
        _objects = objects;
        _metadataProvider = dynamicMetadataProvider;
    }

    public DynamicProxyBuilder<TObject, TMetadataAttribute, TMetadata> WithConversion(string propertyName, Enum to)
    {
        var property = typeof(TObject).GetPropertyFlat(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TObject).Name}");

        _metadataProvider.AddConversion(property, to, _culture);

        return this;
    }

    public DynamicProxyBuilder<TObject, TMetadataAttribute, TMetadata> WithConversion(Expression<Func<TObject, QuantityValue>> propertySelectorExpression, Enum to)
    {
        var propertyName = propertySelectorExpression.ExtractPropertyName();
        var property = typeof(TObject).GetPropertyFlat(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TObject).Name}");

        _metadataProvider.AddConversion(property, to, _culture);

        return this;
    }

    public DynamicProxyBuilder<TSuperObject, TMetadataAttribute, TMetadata> As<TSuperObject>() where TSuperObject : class
    {
        if (!typeof(TSuperObject).IsAssignableFrom(typeof(TObject)))
            throw new InvalidOperationException($"{nameof(TObject)} ({typeof(TObject).Name}) is not derived from {typeof(TSuperObject).Name}");

        _metadataProvider.HoistMetadata<TSuperObject, TObject>(_culture);
        return new(_objects, _metadataProvider);
    }

    public IDynamicProxyEnumerable<TObject, TMetadataAttribute, TMetadata> Build()
    {
        ((IMetadataProvider<TMetadataAttribute, TMetadata>)_metadataProvider).ValidateAllMetadata(typeof(TObject));
        return new DynamicQuantityEnumerable<TObject, TMetadataAttribute, TMetadata>(_objects, _metadataProvider);
    }
}