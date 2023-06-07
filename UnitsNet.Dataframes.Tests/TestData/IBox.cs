namespace UnitsNet.Dataframes.Tests.TestData;

public interface IBox
{
    string? Data { get; set; }
    double Depth { get; set; }
    double Height { get; set; }
    int Items { get; set; }
    int Priority { get; set; }
    int SerialNumber { get; set; }
    double Volume { get; }
    double Weight { get; set; }
    double Width { get; set; }
}