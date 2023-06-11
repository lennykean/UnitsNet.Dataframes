using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace UnitsNet.Metadata;

public interface IMetadata<TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    TMetadata Clone(PropertyInfo? overrideProperty = null, IEnumerable<UnitMetadataBasic>? overrideConversions = null, UnitMetadata? overrideUnit = null, CultureInfo? overrideCulture = null);
    void Validate();
}
