using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace UnitsNet.Dataframes.Utils;

internal class EphemeralValueCache<TKey, TItem> where TItem : class?
{
    private static readonly Lazy<EphemeralValueCache<TKey, TItem>> _lazyInstance = new(() => new());

#if EPHEMERAL_CACHE
    private readonly ConcurrentDictionary<TKey, WeakReference<TItem>?> _cache;

    private EphemeralValueCache()
    {
        _cache = new();
    }
#endif

    public static EphemeralValueCache<TKey, TItem> Instance => _lazyInstance.Value;

    public bool TryGet(TKey key, [MaybeNullWhen(true)] out TItem? item)
    {
        item = default;
#if EPHEMERAL_CACHE
        if (!_cache.TryGetValue(key, out var reference) ||
            reference is null ||
            reference.TryGetTarget(out item) is not true)
        {
            _cache.TryRemove(key, out _);
            return false;
        }
        return true;
#else
        return false;
#endif
    }

    public TItem GetOrAdd(TKey key, Func<TKey, TItem> getter)
    {
#if EPHEMERAL_CACHE
        var reference = _cache.GetOrAdd(key, k =>
        {
            var item = getter(k);
            
            if (item is null)
                return null;

            return new(item);
        });
        if (reference?.TryGetTarget(out var item) is true)
            return item;

        item = getter(key);
        if (item is not null)
            _cache.TryUpdate(key, new(item), reference);
        
        return item;
#else
        return getter(key);
#endif
    }

    public void AddOrUpdate(TKey key, TItem item)
    {
#if EPHEMERAL_CACHE
        _cache.AddOrUpdate(key, _ => new(item), (_, _) => new(item));
#endif
    }

    public void Purge()
    {
#if EPHEMERAL_CACHE
        _cache.Clear();
#endif
    }
}