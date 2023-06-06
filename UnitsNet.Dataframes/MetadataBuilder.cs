using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Utils;

namespace UnitsNet.Dataframes;

internal static class MetadataBuilder
{
    public static bool TryBuildMetadata<TMetadataAttribute, TMetadata>(this PropertyInfo property, [NotNullWhen(true)] out TMetadataAttribute? metadataAttribute, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IClonableMetadata
    {
        metadata = default;

        metadataAttribute = property.GetCustomAttribute<TMetadataAttribute>(inherit: true);
        if (metadataAttribute is null)
            return false;

        var attribute = metadataAttribute;
        metadata = EphemeralValueCache<PropertyInfo, TMetadata>.Instance.GetOrAdd(property, p =>
        {
            var allowedConversionAttributes = p.GetCustomAttributes<AllowUnitConversionAttribute>(inherit: true);
            return attribute.ToMetadata(property, attribute.GetAllowedConversionsMetadata(allowedConversionAttributes, culture));
        });
        return true;
    }

    public static IEnumerable<TMetadata> BuildDataframeMetadata<TMetadataAttribute, TMetadata>(this Type type, CultureInfo? culture = null)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IClonableMetadata
    {
        return EphemeralValueCache<Type, IEnumerable<TMetadata>>.Instance.GetOrAdd(type, key =>
            from property in key.GetProperties((BindingFlags)(-1))
            let m = (hasMetadata: TryBuildMetadata<TMetadataAttribute, TMetadata>(property, out _, out var metadata, culture), metadata)
            where m.hasMetadata
            select m.metadata);
    }
}