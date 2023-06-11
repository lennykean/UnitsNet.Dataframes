using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.TestData
{
    public class DynoDataframe
    {
        [DynoMeasurement(PowerUnit.MechanicalHorsepower, displayName: "Engine Horsepower")]
        public virtual double Horsepower { get; set; }
        [DynoMeasurement(TorqueUnit.PoundForceFoot, displayName: "Engine Torque")]
        public virtual double Torque { get; set; }
        [DynoMeasurement(RotationalSpeedUnit.RevolutionPerMinute, displayName: "Engine Speed")]
        public double Rpm { get; set; }
    }
}
