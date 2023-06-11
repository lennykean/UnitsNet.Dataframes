using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Reflection;

namespace UnitsNet.Metadata.Fluent;

public abstract class MetadataBuilder
{
    internal MetadataBuilder()
    {
    }

    public static MetadataBuilder<TObject, TMetadataAttribute, TMetadata> CreateFor<TObject, TMetadataAttribute, TMetadata>()
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return new();
    }
    
    public static MetadataBuilder<TObject> CreateFor<TObject>()
        where TObject : class
    {
        return new();
    }
}

public class MetadataBuilder<TObject, TMetadataAttribute, TMetadata> : MetadataBuilder
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    protected ConcurrentDictionary<PropertyInfo, TMetadataAttribute> MetadataAttributes = new();
    protected ConcurrentDictionary<TMetadataAttribute, ConcurrentBag<AllowUnitConversionAttribute>> AllowedConversionAttributes = new();

    public MetadataBuilder()
    {
    }

    protected MetadataBuilder(MetadataBuilder<TObject, TMetadataAttribute, TMetadata> other)
    {
        MetadataAttributes = other.MetadataAttributes;
        AllowedConversionAttributes = other.AllowedConversionAttributes;
    }

    public QuantityMetadataBuilder<TObject, TMetadataAttribute, TMetadata> With(string propertyName, TMetadataAttribute attribute)
    {
        var property = typeof(TObject).GetProperty(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TObject).Name}");

        MetadataAttributes.AddOrUpdate(property, attribute, (_, _) => attribute);

        return new(this, attribute);
    }

    public QuantityMetadataBuilder<TObject, TMetadataAttribute, TMetadata> With(Expression<Func<TObject, QuantityValue>> propertySelectorExpression, TMetadataAttribute attribute)
    {
        var propertyName = propertySelectorExpression.ExtractPropertyName();
        
        return With(propertyName, attribute);
    }

    public IMetadataProvider<TMetadataAttribute, TMetadata> Build(bool global = true, CultureInfo? culture = null)
    {
        var provider = new FluentMetadataProvider<TObject, TMetadataAttribute, TMetadata>();

        foreach (var (property, metadataAttribute) in MetadataAttributes)
        {
            var allowedConversions = AllowedConversionAttributes.TryGetValue(metadataAttribute, out var allowUnitConversionAttributes)
                ? metadataAttribute.BuildAllowedConversionsMetadata(allowUnitConversionAttributes)
                : Enumerable.Empty<UnitMetadataBasic>();

            provider.AddMetadata(property, metadataAttribute.ToMetadata(property, allowedConversions, culture: culture));
        }
        if (global)
            GlobalMetadataProvider<TMetadataAttribute, TMetadata>.Instance.RegisterProvider(provider, priority: 1);

        return provider;
    }
}

public class MetadataBuilder<TObject> : MetadataBuilder<TObject, QuantityAttribute, QuantityMetadata>
    where TObject : class
{
    public MetadataBuilder()
    {
    }

    protected MetadataBuilder(MetadataBuilder<TObject> other)
    {
        MetadataAttributes = other.MetadataAttributes;
        AllowedConversionAttributes = other.AllowedConversionAttributes;
    }

    public QuantityMetadataBuilder<TObject> With(string propertyName, Enum asUnit, Type? quantityType = null)
    {
        var attribute = new QuantityAttribute(asUnit, quantityType);

        With(propertyName, attribute);

        return new(this, attribute);
    }

    public QuantityMetadataBuilder<TObject> With(Expression<Func<TObject, QuantityValue>> propertySelectorExpression, Enum asUnit, Type? quantityType = null)
    {
        var propertyName = propertySelectorExpression.ExtractPropertyName();

        return With(propertyName, asUnit, quantityType);
    }
}
