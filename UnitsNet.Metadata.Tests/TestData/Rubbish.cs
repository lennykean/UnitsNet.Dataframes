using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.Tests.TestData;

public class Rubbish
{
    [Quantity(CoolnessUnit.Fonzie)]
    public virtual double Coolness { get; set; }
}
