namespace UnitsNet.Metadata.Tests.TestData;

public class HardDrive : IHardDrive
{
    public string Manufacturer { get; set; } = "";
    public string Model { get; set; } = "";
    public string SerialNumber { get; set; } = "";
    public double Capacity { get; set; }
    double IHardDrive.FreeSpace { get; set; }
}
