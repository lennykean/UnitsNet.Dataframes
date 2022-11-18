using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public class MetadataCache
    {
        private static readonly Lazy<MetadataCache> _lazyInstance = new(() => new());
        
        private readonly ConcurrentDictionary<PropertyInfo, SensorMetadata?> _sensorMetadataCache;
        private readonly ConcurrentDictionary<Type, QuantityTypeMetadata?> _quantityTypeMetadataCache;
        private readonly ConcurrentDictionary<Enum, UnitMetadataFull?> _unitMetadataFullCache;
        private readonly ConcurrentDictionary<Enum, UnitMetadata?> _unitMetadataCache;

        private MetadataCache()
        {
            _sensorMetadataCache = new();
            _quantityTypeMetadataCache = new();
            _unitMetadataFullCache = new();
            _unitMetadataCache = new();
        }

        public static MetadataCache Instance => _lazyInstance.Value;

        public SensorMetadata? GetOrCreate(PropertyInfo property, Func<SensorMetadata?> getter)
        {
            return _sensorMetadataCache.GetOrAdd(property, _ => getter());
        }

        public QuantityTypeMetadata? GetOrCreate(Type type, Func<QuantityTypeMetadata?> getter)
        {
            return _quantityTypeMetadataCache.GetOrAdd(type, _ => getter());
        }

        public UnitMetadataFull? GetOrCreate(Enum value, Func<UnitMetadataFull> getter)
        {
            return _unitMetadataFullCache.GetOrAdd(value, _ => getter());
        }

        public UnitMetadata? GetOrCreate(Enum value, Func<UnitMetadata> getter)
        {
            return _unitMetadataCache.GetOrAdd(value, _ => getter());
        }

        public void Purge()
        {
            _sensorMetadataCache.Clear();
            _quantityTypeMetadataCache.Clear();
            _unitMetadataFullCache.Clear();
            _unitMetadataCache.Clear();
        }
    }
}