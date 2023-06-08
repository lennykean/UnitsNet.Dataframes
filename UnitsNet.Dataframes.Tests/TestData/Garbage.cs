using UnitsNet.Dataframes.Attributes;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests;

public class Garbage
{
    [Quantity(InformationUnit.Bit)]
    public virtual string? Data { get; set; }
}
