using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes.Tests.TestData;

public class Employee
{
    public int Number { get; set; }
    public string? Name { get; set; }
    [Quantity(CoolnessUnit.MegaFonzie, typeof(Coolness))]
    public double Coolness { get; set; }
}