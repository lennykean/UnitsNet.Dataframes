using UnitsNet.Dataframes.Attributes;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.TestData;

public interface IBox
{
    int SerialNumber { get; set; }
    int Priority { get; set; }
    double Width { get; set; }
    double Height { get; set; }
    double Depth { get; set; }
    double Weight { get; set; }
    int Items { get; set; }
    string? Data { get; set; }
    double Volume { get; }
}