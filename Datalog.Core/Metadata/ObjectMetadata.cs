using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public class ObjectMetadata<T> : ReadOnlyDictionary<string, SensorMetadata>
    {
        public ObjectMetadata(CultureInfo? culture = null) : base(BuildMetadata(typeof(T), culture))
        {
        }

        public static SensorMetadata? GetUnitMetadata(PropertyInfo property, CultureInfo? culture = null)
        {
            return MetadataCache.Instance.GetOrCreate(property, () =>
            {
                if (!property.DeclaringType.IsAssignableFrom(typeof(T)))
                    throw new InvalidOperationException($"{property.Name} is not a member of {typeof(T)}");

                var metadataAttribute = property.GetCustomAttribute<SensorMetadataAttribute>(inherit: true);
                if (metadataAttribute == null)
                    return null;

                return new SensorMetadata(metadataAttribute, culture);
            });
        }

        public static SensorMetadata? GetUnitMetadata(string propertyName, CultureInfo? culture = null)
        {
            return GetUnitMetadata(typeof(T).GetProperty(propertyName), culture);
        }

        private static IDictionary<string, SensorMetadata> BuildMetadata(Type frameType, CultureInfo? culture)
        {            
            return (
                from property in frameType.GetProperties()
                let metadata = GetUnitMetadata(property, culture)
                where metadata != null
                select (key: property.Name, value: metadata))
                .ToDictionary(k => k.key, v => v.value);
        }
    }
}
