using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata.Annotations;

internal class AnnotationMetadataProvider<TMetadataAttribute, TMetadata> : IMetadataProvider<TMetadataAttribute, TMetadata>
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private static readonly Lazy<AnnotationMetadataProvider<TMetadataAttribute, TMetadata>> _instance = new(() => new AnnotationMetadataProvider<TMetadataAttribute, TMetadata>());

    private AnnotationMetadataProvider()
    {
    }

    public static AnnotationMetadataProvider<TMetadataAttribute, TMetadata> Instance => _instance.Value;

    public bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null)
    {
        var cache = EphemeralValueCache<(Type, Type, string), TMetadata>.Instance;

        if (cache.TryGet((property.DeclaringType, property.PropertyType, property.Name), out metadata))
            return true;

        var metadataAttribute = property.GetCustomAttribute<TMetadataAttribute>(inherit: true);
        if (metadataAttribute is null)
            return false;

        metadataAttribute.Validate();

        metadata = cache.GetOrAdd((property.DeclaringType, property.PropertyType, property.Name), p =>
        {
            var allowedConversionAttributes = property.GetCustomAttributes<AllowUnitConversionAttribute>(inherit: true);
            return metadataAttribute.ToMetadata(property, metadataAttribute.BuildAllowedConversionsMetadata(allowedConversionAttributes, culture));
        });
        return true;
    }

    public void ValidateMetadata(PropertyInfo property)
    {
        if (!TryGetMetadata(property, out var metadata))
            return;

        metadata.Validate();
    }
}