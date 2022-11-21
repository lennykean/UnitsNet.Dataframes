using System;
using System.Collections.Concurrent;

namespace HondataDotNet.Datalog.Core.Utils
{
    public class SimpleCache<TKey, TItem>
    {
        private static readonly Lazy<SimpleCache<TKey, TItem>> _lazyInstance = new(() => new());

        private readonly ConcurrentDictionary<TKey, TItem> _cache;

        private SimpleCache()
        {
            _cache = new();
        }

        public static SimpleCache<TKey, TItem> Instance => _lazyInstance.Value;

        public bool TryGet(TKey key, out TItem item)
        {
            return _cache.TryGetValue(key, out item);
        }

        public TItem GetOrAdd(TKey key, Func<TItem> getter)
        {
            return _cache.GetOrAdd(key, _ => getter());
        }

        public bool TryAdd(TKey key, TItem item)
        {
            return _cache.TryAdd(key, item);
        }

        public void Purge()
        {
            _cache.Clear();
        }
    }
}