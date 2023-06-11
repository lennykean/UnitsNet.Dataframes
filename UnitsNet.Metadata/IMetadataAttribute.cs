using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata;

public interface IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    TMetadata ToMetadata(PropertyInfo property, IEnumerable<UnitMetadataBasic> conversions, UnitMetadata? overrideUnit = null, CultureInfo? culture = null);
    void Validate();
}
