using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    public abstract class ObjectMetadata<TObject, TMetadataAttribute, TMetadata, TMapper> : ReadOnlyDictionary<string, TMetadata>
        where TMetadataAttribute : QuantityAttribute
        where TMetadata : QuantityMetadata
        where TMapper : ObjectMetadata<TObject, TMetadataAttribute, TMetadata, TMapper>.IMetadataAttributeMapper, new()
    {
        public interface IMetadataAttributeMapper
        {
            TMetadata? Map(TMetadataAttribute metadataAttribute, string name, CultureInfo? culture, Enum[] allowedConversions);
        }

        private static readonly TMapper _mapper = new();

        protected ObjectMetadata(CultureInfo? culture) : base(BuildMetadata(typeof(TObject), culture))
        {
        }

        public static TMetadata? GetQuantityMetadata(PropertyInfo property, CultureInfo? culture = null)
        {
            // Get metadata from cache, or get and add to cache
            return SimpleCache<PropertyInfo, TMetadata?>.Instance.GetOrAdd(property, () =>
            {
                if (!property.DeclaringType.IsAssignableFrom(typeof(TObject)))
                    throw new InvalidOperationException($"{property.Name} is not a member of {typeof(TObject)}");

                var metadataAttribute = property.GetCustomAttribute<TMetadataAttribute>(inherit: true);
                if (metadataAttribute is null)
                    return null;

                var allowedConversions = (
                    from a in property.GetCustomAttributes<AllowUnitConversionAttribute>(inherit: true) 
                    select a.Unit!).ToArray();
                var invalidAllowedConversions = (
                    from c in allowedConversions
                    let convertableUnits = metadataAttribute.QuantityInfo!.UnitInfos.Select(i => i.Value)
                    where !convertableUnits.Contains(c)
                    select c).ToList();
                if (invalidAllowedConversions.Any())
                    throw new InvalidOperationException($"Invalid unit conversion(s): from {metadataAttribute.Unit} to [{String.Join(", ", invalidAllowedConversions)}]");

                return _mapper.Map(metadataAttribute, property.Name, culture, allowedConversions);
            });
        }

        private static IDictionary<string, TMetadata> BuildMetadata(Type type, CultureInfo? culture)
        {
            return (
                from property in type.GetProperties()
                let metadata = GetQuantityMetadata(property, culture)
                where metadata != null
                select (key: property.Name, value: metadata))
                .ToDictionary(k => k.key, v => v.value);
        }
    }

    public sealed class ObjectMetadata<TObject> : ObjectMetadata<TObject, QuantityAttribute, QuantityMetadata, ObjectMetadata<TObject>.Mapper>
    {
        public ObjectMetadata(CultureInfo? culture = null) : base(culture)
        {
        }

        public class Mapper : IMetadataAttributeMapper
        {
            public QuantityMetadata? Map(QuantityAttribute metadataAttribute, string name, CultureInfo? culture, Enum[] allowedConversions)
            {
                return new QuantityMetadata(metadataAttribute, name, culture, allowedConversions);
            }
        }
    }
}
