using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public class DatalogFrameMetadata<TDataLogFrame> : ReadOnlyDictionary<string, SensorMetadata> where TDataLogFrame : IDatalogFrame
    {
        public DatalogFrameMetadata(CultureInfo? culture = null) : base(BuildMetadata(typeof(TDataLogFrame), culture))
        {
        }

        private static IDictionary<string, SensorMetadata> BuildMetadata(Type frameType, CultureInfo? culture)
        {            
            return (
                from prop in frameType.GetProperties()
                let metadataAttribute = prop.GetCustomAttribute<SensorMetadataAttribute>()
                where metadataAttribute != null
                select (key: prop.Name, value: MetadataCache.Instance.GetOrCreate(prop, () => new SensorMetadata(metadataAttribute, culture))))
                .ToDictionary(k => k.key, v => v.value);
        }
    }
}
