using System;

namespace UnitsNet.Metadata.Tests.TestData;

public class Cylinder
{
    public double Radius { get; set; }
    public double Height { get; set; }
    public double Volume => Math.PI * Radius * Radius * Height;
}
