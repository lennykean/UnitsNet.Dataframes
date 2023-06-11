using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.Fluent;

public class QuantityMetadataBuilder<TObject, TMetadataAttribute, TMetadata> : MetadataBuilder<TObject, TMetadataAttribute, TMetadata>
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private readonly TMetadataAttribute _attribute;

    public QuantityMetadataBuilder(MetadataBuilder<TObject, TMetadataAttribute, TMetadata> inner, TMetadataAttribute attribute) : base(inner)
    {
        _attribute = attribute;
    }

    public MetadataBuilder<TObject, TMetadataAttribute, TMetadata> WithAllowedConversion<TBuilder>(Enum to, Type? quantityType = null)
    {
        var conversionAttribute = new AllowUnitConversionAttribute(to, quantityType);

        AllowedConversionAttributes.AddOrUpdate(_attribute, _ => new() { conversionAttribute }, (_, bag) =>
        {
            bag.Add(conversionAttribute);
            return bag;
        });
        
        return this;
    }
}

public class QuantityMetadataBuilder<TObject> : MetadataBuilder<TObject>
    where TObject : class
{
    private readonly QuantityAttribute _attribute;

    public QuantityMetadataBuilder(MetadataBuilder<TObject> inner, QuantityAttribute attribute) : base(inner)
    {
        _attribute = attribute;
    }

    public QuantityMetadataBuilder<TObject> WithAllowedConversion(Enum to, Type? quantityType = null)
    {
        var conversionAttribute = new AllowUnitConversionAttribute(to, quantityType);

        AllowedConversionAttributes.AddOrUpdate(_attribute, _ => new() { conversionAttribute }, (_, bag) =>
        {
            bag.Add(conversionAttribute);
            return bag;
        });

        return this;
    }
}