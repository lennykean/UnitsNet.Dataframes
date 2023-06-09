using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Utils;

namespace UnitsNet.Dataframes;

public sealed class DefaultDataframeMetadataProvider<TMetadataAttribute, TMetadata> : IDataframeMetadataProvider<TMetadataAttribute, TMetadata>
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
{
    private static readonly Lazy<DefaultDataframeMetadataProvider<TMetadataAttribute, TMetadata>> _instance = new(() => new DefaultDataframeMetadataProvider<TMetadataAttribute, TMetadata>());

    private DefaultDataframeMetadataProvider()
    {
    }

    public static DefaultDataframeMetadataProvider<TMetadataAttribute, TMetadata> Instance => _instance.Value;

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

    public void ValidateMetadata(PropertyInfo property)
    {
        if (!TryGetMetadata(property, out var metadata))
            return;

        metadata.Validate();
    }
}