using UnitsNet.Dataframes.Attributes;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.TestData;

public class Blob
{
    [Quantity(InformationUnit.Bit)]
    public virtual string? Data { get; set; }
}
