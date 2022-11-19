using System;
using System.Collections.Concurrent;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public class MetadataCache<TKey, TValue>
    {
        private static readonly Lazy<MetadataCache<TKey, TValue>> _lazyInstance = new(() => new());

        private readonly ConcurrentDictionary<TKey, TValue?> _cache;

        private MetadataCache()
        {
            _cache = new();
        }

        public static MetadataCache<TKey, TValue> Instance => _lazyInstance.Value;

        public TValue? GetOrCreate(TKey key, Func<TValue?> getter)
        {
            return _cache.GetOrAdd(key, _ => getter());
        }

        public void Purge()
        {
            _cache.Clear();
        }
    }
}