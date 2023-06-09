using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes.Tests.TestData;

public class Rubbish
{
    [Quantity(CoolnessUnit.Fonzie)]
    public virtual double Coolness { get; set; }
}
