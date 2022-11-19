using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public abstract class ObjectMetadata<TObject, TMetadataAttribute, TMetadata, TMapper> : ReadOnlyDictionary<string, TMetadata>
        where TMetadataAttribute : QuantityMetadataAttribute 
        where TMetadata : QuantityMetadata
        where TMapper : ObjectMetadata<TObject, TMetadataAttribute, TMetadata, TMapper>.IMetadataAttributeMapper, new()
    {
        public interface IMetadataAttributeMapper
        {
            TMetadata? Map(TMetadataAttribute metadataAttribute, CultureInfo? culture);
        }

        private static readonly TMapper _mapper = new();

        protected ObjectMetadata(CultureInfo? culture) : base(BuildMetadata(typeof(TObject), culture))
        {
        }

        public static TMetadata? GetQuantityMetadata(PropertyInfo property, CultureInfo? culture = null)
        {
            return MetadataCache<PropertyInfo, TMetadata>.Instance.GetOrCreate(property, () =>
            {
                if (!property.DeclaringType.IsAssignableFrom(typeof(TObject)))
                    throw new InvalidOperationException($"{property.Name} is not a member of {typeof(TObject)}");

                var metadataAttribute = property.GetCustomAttribute<TMetadataAttribute>(inherit: true);
                if (metadataAttribute == null)
                    return null;

                return _mapper.Map(metadataAttribute, culture);
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

    public sealed class ObjectMetadata<TObject> : ObjectMetadata<TObject, QuantityMetadataAttribute, QuantityMetadata, ObjectMetadata<TObject>.Mapper>
    {
        public ObjectMetadata(CultureInfo? culture = null) : base(culture)
        {
        }

        public class Mapper : IMetadataAttributeMapper
        {
            public QuantityMetadata? Map(QuantityMetadataAttribute metadataAttribute, CultureInfo? culture)
            {
                return new QuantityMetadata(metadataAttribute, culture);
            }
        }
    }
}
