using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata;

public sealed class GlobalMetadataProvider<TMetadataAttribute, TMetadata> : IMetadataProvider<TMetadataAttribute, TMetadata>
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    static GlobalMetadataProvider()
    {
        Instance.RegisterProvider(AnnotationMetadataProvider<TMetadataAttribute, TMetadata>.Instance, 2);
    }

    private static readonly Lazy<GlobalMetadataProvider<TMetadataAttribute, TMetadata>> _instance = new(() => new());

    private readonly ConcurrentDictionary<Type, (IMetadataProvider<TMetadataAttribute, TMetadata> provider, int priority)> _providers = new();

    public static GlobalMetadataProvider<TMetadataAttribute, TMetadata> Instance => _instance.Value;

    public void RegisterProvider(IMetadataProvider<TMetadataAttribute, TMetadata> provider, int priority = 0)
    {
        _providers.AddOrUpdate(provider.GetType(), (provider, priority), (_, _) => (provider, priority));
    }

    public void ClearProviders()
    {
        _providers.Clear();
    }

    public bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null)
    {
        foreach (var provider in _providers.Values.OrderBy(p => p.priority).Select(p => p.provider))
        {
            if (provider.TryGetMetadata(property, out metadata, culture))
                return true;
        }
        metadata = default;
        return false;
    }

    public void ValidateMetadata(PropertyInfo property)
    {
        foreach (var provider in _providers.Values.OrderBy(p => p.priority).Select(p => p.provider))
            provider.ValidateMetadata(property);
    }
}
