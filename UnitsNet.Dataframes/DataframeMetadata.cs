using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes;

public class DataframeMetadata<TMetadataAttribute, TMetadata> : ReadOnlyDictionary<PropertyInfo, TMetadata>
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IClonableMetadata
{
    public interface IMetadataAttribute
    {
        TMetadata ToMetadata(PropertyInfo property, IEnumerable<UnitMetadataBasic> conversions, UnitMetadata? overrideUnit = null, CultureInfo? culture = null);
    }

    public interface IClonableMetadata
    {
        TMetadata Clone(PropertyInfo? overrideProperty = null, IEnumerable<UnitMetadataBasic>? overrideConversions = null, UnitMetadata? overrideUnit = null, CultureInfo? overrideCulture = null);
    }

    internal DataframeMetadata(IEnumerable<TMetadata> metadatas) : base(metadatas.ToDictionary(k => k.Property, v => v))
    {
    }
}