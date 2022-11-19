namespace HondataDotNet.Datalog.Core.Units
{
    [QuantityType(typeof(AirFuelRatio), nameof(AirFuelRatio.Info))]
    public enum AirFuelRatioUnit
    {
        Undefined = 0,
        Lambda = 1,
        GasolineAirFuelRatio = 2,
        E85AirFuelRatio = 3
    }
}