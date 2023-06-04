using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Utils;

namespace UnitsNet.Dataframes;

public interface IDataframeMetadata<TMetadataAttribute, TMetadata, TMapper>
    where TMetadataAttribute : QuantityAttribute
    where TMetadata : QuantityMetadata
    where TMapper : IDataframeMetadata<TMetadataAttribute, TMetadata, TMapper>.IMetadataAttributeMapper, new()
{
    public interface IMetadataAttributeMapper
    {
        TMetadata Map(TMetadataAttribute metadataAttribute, PropertyInfo property, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null);
    }

    private static readonly TMapper _mapper = new();

    public static bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)]out TMetadata? metadata, CultureInfo? culture = null)
    {
        metadata = default;
        var metadataAttribute = property.GetCustomAttribute<TMetadataAttribute>(inherit: true);
        if (metadataAttribute is null)
            return false;

        metadata = EphemeralValueCache<PropertyInfo, TMetadata>.Instance.GetOrAdd(property, p =>
        {
            return _mapper.Map(metadataAttribute, property, p.GetCustomAttributes<AllowUnitConversionAttribute>(inherit: true), culture);
        });
        return true;
    }

    public static IEnumerable<TMetadata> BuildMetadata(Type forType, CultureInfo? culture)
    {
        return EphemeralValueCache<Type, IEnumerable<TMetadata>>.Instance.GetOrAdd(forType, key =>
            from property in key.GetProperties((BindingFlags)(-1))
            let m = (hasMetadata: TryGetMetadata(property, out var metadata, culture), metadata)
            where m.hasMetadata
            select m.metadata);
    }
}