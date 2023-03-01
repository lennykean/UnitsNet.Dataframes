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
    public class ObjectMetadata<TMetadata> : ReadOnlyDictionary<PropertyInfo, TMetadata>
        where TMetadata : QuantityMetadata
    {
        public ObjectMetadata(IEnumerable<TMetadata> metadatas) : base(metadatas.ToDictionary(k => k.Property, v => v))
        {
        }
    }

    public abstract class ObjectMetadata<TMetadataAttribute, TMetadata, TMapper> : ObjectMetadata<TMetadata>
        where TMetadataAttribute : QuantityAttribute
        where TMetadata : QuantityMetadata
        where TMapper : ObjectMetadata<TMetadataAttribute, TMetadata, TMapper>.IMetadataAttributeMapper, new()
    {
        public interface IMetadataAttributeMapper
        {
            TMetadata Map(TMetadataAttribute metadataAttribute, PropertyInfo property, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null);
        }

        private static readonly TMapper _mapper = new();

        protected ObjectMetadata(Type forType, CultureInfo? culture = null) : base(BuildMetadata(forType, culture))
        {
        }

        public static TMetadata MapAttribute(TMetadataAttribute metadataAttribute, PropertyInfo property, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
        {
            return _mapper.Map(metadataAttribute, property, allowedConversions, culture);
        }

        public static TMetadata? GetQuantityMetadata(PropertyInfo property, CultureInfo? culture = null)
        {
            return SimpleCache<PropertyInfo, TMetadata?>.Instance.GetOrAdd(property, p =>
            {
                var metadataAttribute = p.GetCustomAttribute<TMetadataAttribute>(inherit: true);
                if (metadataAttribute is null)
                    return null;

                return _mapper.Map(metadataAttribute, property, p.GetCustomAttributes<AllowUnitConversionAttribute>(inherit: true), culture);
            });
        }

        private static IEnumerable<TMetadata> BuildMetadata(Type forType, CultureInfo? culture)
        {
            return
                from property in forType.GetProperties((BindingFlags)(-1))
                let metadata = GetQuantityMetadata(property, culture)
                where metadata != null
                select metadata;
        }
    }

    public sealed class QuantityObjectMetadata : ObjectMetadata<QuantityAttribute, QuantityMetadata, QuantityObjectMetadata.Mapper>
    {
        public class Mapper : IMetadataAttributeMapper
        {
            public QuantityMetadata Map(QuantityAttribute metadataAttribute, PropertyInfo property, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
            {
                return QuantityMetadata.FromQuantityAttribute(metadataAttribute, property, allowedConversions, culture);
            }
        }

        public QuantityObjectMetadata(Type forType, CultureInfo? culture = null) : base(forType, culture)
        {
        }

        public static QuantityObjectMetadata For<TObject>(CultureInfo? culture = null)
        {
            return new QuantityObjectMetadata(typeof(TObject), culture);
        }
    }
}
