using UnitsNet.Dataframes.Attributes;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests;

public class Box
{
    [Quantity(LengthUnit.Meter)]
    public virtual double Width { get; set; }
    [Quantity(LengthUnit.Meter)]
    public virtual double Height { get; set; }
    [Quantity(LengthUnit.Meter)]
    public virtual double Depth { get; set; }
    [Quantity(MassUnit.Kilogram), AllowUnitConversion(MassUnit.Gram)]
    public virtual double Weight { get; set; }

    [Quantity(VolumeUnit.CubicMeter)]
    public virtual double Volume => Width * Height * Depth;
}
