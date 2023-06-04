using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace UnitsNet.Dataframes.Utils;

internal class EphemeralKeyCache<TKey, TItem> where TKey : class where TItem : class?
{
    private static readonly Lazy<EphemeralKeyCache<TKey, TItem>> _lazyInstance = new(() => new());
    
#if EPHEMERAL_CACHE
    private readonly ConditionalWeakTable<TKey, TItem> _cache;

    private EphemeralKeyCache()
    {
        _cache = new();
    }
#endif

    public static EphemeralKeyCache<TKey, TItem> Instance => _lazyInstance.Value;

    public bool TryGet(TKey key, [MaybeNullWhen(true)] out TItem? item)
    {
#if EPHEMERAL_CACHE
        if (!_cache.TryGetValue(key, out item))
            return false;

        return true;
#else
        item = default;
        return false;
#endif
    }

    public TItem GetOrAdd(TKey key, Func<TKey, TItem> getter)
    {
#if EPHEMERAL_CACHE
        return _cache.GetValue(key, k => getter(k));
#else
        return getter(key);
#endif
    }

    public void AddOrUpdate(TKey key, TItem item)
    {
#if EPHEMERAL_CACHE
        _cache.AddOrUpdate(key, item);
#endif
    }

    public void Purge()
    {
#if EPHEMERAL_CACHE
        _cache.Clear();
#endif
    }
}