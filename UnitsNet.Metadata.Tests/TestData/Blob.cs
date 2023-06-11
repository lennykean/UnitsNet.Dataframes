using UnitsNet.Metadata.Annotations;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.TestData;

public class Blob
{
    [Quantity(InformationUnit.Bit)]
    public virtual string? Data { get; set; }
}
