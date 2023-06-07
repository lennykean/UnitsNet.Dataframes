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
    where TDataframe : class
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
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
        var cache = EphemeralValueCache<PropertyInfo, TMetadata>.Instance;

        if (cache.TryGet(property, out metadata))
            return true;

        var metadataAttribute = property.GetCustomAttribute<TMetadataAttribute>(inherit: true);
        if (metadataAttribute is null)
            return false;

        metadataAttribute.Validate();

        metadata = cache.GetOrAdd(property, p =>
        {
            var allowedConversionAttributes = p.GetCustomAttributes<AllowUnitConversionAttribute>(inherit: true);
            return metadataAttribute.ToMetadata(property, metadataAttribute.BuildAllowedConversionsMetadata(allowedConversionAttributes, culture));
        });
        return true;
    }

    public void ValidateAllMetadata()
    {
        foreach (var metadata in GetAllMetadata())
        {
            if (!metadata.Property.DeclaringType.IsAssignableFrom(typeof(TDataframe)))
                throw new InvalidOperationException($"{metadata.Property.DeclaringType}.{metadata.Property} metadata is not valid on type {typeof(TDataframe).Name}");

            metadata.Validate();
        }
    }
}