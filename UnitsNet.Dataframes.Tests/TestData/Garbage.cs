using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes.Tests.TestData;

public class Garbage
{
    [Quantity("Blatt")]
    public virtual double Odor { get; set; }
}
