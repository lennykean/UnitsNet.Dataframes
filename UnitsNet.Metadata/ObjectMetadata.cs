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
    public abstract class ObjectMetadata<TMetadataAttribute, TMetadata, TMapper> : ReadOnlyDictionary<string, TMetadata>
        where TMetadataAttribute : QuantityAttribute
        where TMetadata : QuantityMetadata
        where TMapper : ObjectMetadata<TMetadataAttribute, TMetadata, TMapper>.IMetadataAttributeMapper, new()
    {
        public interface IMetadataAttributeMapper
        {
            TMetadata Map(TMetadataAttribute metadataAttribute, string name, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null);
        }

        private static readonly TMapper _mapper = new();

        private protected ObjectMetadata(IDictionary<string, TMetadata> dictionary) : base(dictionary)
        {
        }

        public static TMetadata? GetQuantityMetadata(PropertyInfo property, CultureInfo? culture = null)
        {
            return SimpleCache<PropertyInfo, TMetadata?>.Instance.GetOrAdd(property, p =>
            {
                var metadataAttribute = p.GetCustomAttribute<TMetadataAttribute>(inherit: true);
                if (metadataAttribute is null)
                    return null;

                return _mapper.Map(metadataAttribute, p.Name, p.GetCustomAttributes<AllowUnitConversionAttribute>(inherit: true), culture);
            });
        }
    }

    public abstract class ObjectMetadata<TObject, TMetadataAttribute, TMetadata, TMapper> : ObjectMetadata<TMetadataAttribute, TMetadata, TMapper>
        where TMetadataAttribute : QuantityAttribute
        where TMetadata : QuantityMetadata
        where TMapper : ObjectMetadata<TMetadataAttribute, TMetadata, TMapper>.IMetadataAttributeMapper, new()
    {
        protected ObjectMetadata(CultureInfo? culture) : base(BuildMetadata(typeof(TObject), culture))
        {
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

    public class ObjectMetadata : ObjectMetadata<QuantityAttribute, QuantityMetadata, ObjectMetadata.Mapper>
    {
        private protected ObjectMetadata(Dictionary<string, QuantityMetadata> dictionary) : base(dictionary)
        {
        }

        public class Mapper : IMetadataAttributeMapper
        {
            public QuantityMetadata Map(QuantityAttribute metadataAttribute, string name, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
            {
                return QuantityMetadata.FromQuantityAttribute(metadataAttribute, name, allowedConversions, culture);
            }
        }
    }

    public class ObjectMetadata<TObject> : ObjectMetadata<TObject, QuantityAttribute, QuantityMetadata, ObjectMetadata.Mapper>
    {
        public ObjectMetadata(CultureInfo? culture = null) : base(culture)
        {
        }
    }
}
