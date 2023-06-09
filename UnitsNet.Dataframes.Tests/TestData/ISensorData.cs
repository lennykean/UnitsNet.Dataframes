namespace UnitsNet.Dataframes.Tests.TestData;

public interface ISensorData
{
    double Power { get; set; }
    double Frequency { get; set; }
    double Temperature { get; set; }
}