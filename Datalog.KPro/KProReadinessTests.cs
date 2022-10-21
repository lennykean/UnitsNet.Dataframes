using System;

namespace HondataDotNet.Datalog.KPro
{
    [Flags]
    public enum KProReadinessTests : byte
    {
        Catalyst = 0x01,
        HeatedCatalyst = 0x02,
        EVAPSystem = 0x04,
        SecondAirSystem = 0x08,
        ACRefrigerant = 0x10,
        O2Sensor = 0x20,
        O2SensorHeater = 0x40,
        EGR = 0x80
    }
}
