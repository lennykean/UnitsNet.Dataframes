using UnitsNet.Dataframes.Attributes;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.TestData
{
    public class VehicleTelemetry
    {
        [Quantity(FuelEfficiencyUnit.KilometerPerLiter)]
        public virtual double Efficency { get; set; }
        [Quantity(RatioUnit.Percent)]
        public virtual double FuelLevel { get; set; }
        [Quantity(SpeedUnit.KilometerPerHour)]
        public virtual double Speed { get; set; }
    }
}
