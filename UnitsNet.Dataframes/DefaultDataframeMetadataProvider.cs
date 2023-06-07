using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Utils;

namespace UnitsNet.Dataframes;

public sealed class DefaultDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata> : IDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata>
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IClonableMetadata
{
    private static readonly Lazy<DefaultDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata>> _instance = new(() => new DefaultDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata>());

    private DefaultDataframeMetadataProvider()
    {
    }

    public static DefaultDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata> Instance => _instance.Value;

    public IEnumerable<TMetadata> GetAllMetadata(CultureInfo? culture = null)
    {
        return EphemeralValueCache<Type, IEnumerable<TMetadata>>.Instance.GetOrAdd(typeof(TDataframe), key =>
            from property in key.GetProperties((BindingFlags)(-1))
            let m = (hasMetadata: TryGetMetadata(property, out var metadata, culture), metadata)
            where m.hasMetadata
            select m.metadata);
    }
    
    public bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null)
    {
        metadata = default;

        var metadataAttribute = property.GetCustomAttribute<TMetadataAttribute>(inherit: true);
        if (metadataAttribute is null)
            return false;

        metadata = EphemeralValueCache<PropertyInfo, TMetadata>.Instance.GetOrAdd(property, p =>
        {
            var allowedConversionAttributes = p.GetCustomAttributes<AllowUnitConversionAttribute>(inherit: true);
            return metadataAttribute.ToMetadata(property, metadataAttribute.GetConversionsMetadata(allowedConversionAttributes, culture));
        });
        return true;
    }
}