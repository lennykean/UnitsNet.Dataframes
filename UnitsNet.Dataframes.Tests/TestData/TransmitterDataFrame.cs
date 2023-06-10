using System;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.TestData;

public class TransmitterDataFrame : ITransmitterData
{
    public DateTime TimeStamp { get; set; }
    [Quantity(PowerUnit.Watt)]
    public double Power { get; set; }
    [Quantity(FrequencyUnit.Hertz)]
    public double Frequency { get; set; }
    [Quantity(TemperatureUnit.Kelvin)]
    public double Temperature { get; set; }
}