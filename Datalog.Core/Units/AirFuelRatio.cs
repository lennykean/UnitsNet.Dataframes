using System;
using System.Globalization;

using UnitsNet;

namespace HondataDotNet.Datalog.Core.Units
{
    public struct AirFuelRatio : IQuantity
    {
        public const double GasolineStoichimetricRatio = 14.7;
        public const double E85StoichimetricRatio = 9.733;

        static AirFuelRatio()
        {
            BaseDimensions = BaseDimensions.Dimensionless;
            Zero = new(0, AirFuelRatioUnit.Lambda);
            Info = new(
                nameof(AirFuelRatio),
                typeof(AirFuelRatioUnit),
                new[]
                {
                    new UnitInfo<AirFuelRatioUnit>(AirFuelRatioUnit.Lambda, nameof(AirFuelRatioUnit.Lambda), BaseUnits.Undefined),
                    new UnitInfo<AirFuelRatioUnit>(AirFuelRatioUnit.GasolineAirFuelRatio, nameof(AirFuelRatioUnit.GasolineAirFuelRatio), BaseUnits.Undefined),
                    new UnitInfo<AirFuelRatioUnit>(AirFuelRatioUnit.E85AirFuelRatio, nameof(AirFuelRatioUnit.E85AirFuelRatio), BaseUnits.Undefined),
                },
                AirFuelRatioUnit.Lambda,
                Zero,
                BaseDimensions.Dimensionless);

            UnitConverter.Default.SetConversionFunction<AirFuelRatio>(AirFuelRatioUnit.Lambda, AirFuelRatioUnit.GasolineAirFuelRatio, x => new(x.Value * GasolineStoichimetricRatio, AirFuelRatioUnit.GasolineAirFuelRatio));
            UnitConverter.Default.SetConversionFunction<AirFuelRatio>(AirFuelRatioUnit.Lambda, AirFuelRatioUnit.E85AirFuelRatio, x => new(x.Value * E85StoichimetricRatio, AirFuelRatioUnit.E85AirFuelRatio));
            UnitConverter.Default.SetConversionFunction<AirFuelRatio>(AirFuelRatioUnit.GasolineAirFuelRatio, AirFuelRatioUnit.Lambda, x => new(GasolineStoichimetricRatio / x.Value, AirFuelRatioUnit.Lambda));
            UnitConverter.Default.SetConversionFunction<AirFuelRatio>(AirFuelRatioUnit.GasolineAirFuelRatio, AirFuelRatioUnit.E85AirFuelRatio, x => new(x.Value * (E85StoichimetricRatio / GasolineStoichimetricRatio), AirFuelRatioUnit.E85AirFuelRatio));
            UnitConverter.Default.SetConversionFunction<AirFuelRatio>(AirFuelRatioUnit.E85AirFuelRatio, AirFuelRatioUnit.Lambda, x => new(E85StoichimetricRatio / x.Value, AirFuelRatioUnit.Lambda));
            UnitConverter.Default.SetConversionFunction<AirFuelRatio>(AirFuelRatioUnit.E85AirFuelRatio, AirFuelRatioUnit.GasolineAirFuelRatio, x => new(x.Value * (GasolineStoichimetricRatio / E85StoichimetricRatio), AirFuelRatioUnit.GasolineAirFuelRatio));

            UnitAbbreviationsCache.Default.MapUnitToDefaultAbbreviation(AirFuelRatioUnit.Lambda, "");
            UnitAbbreviationsCache.Default.MapUnitToDefaultAbbreviation(AirFuelRatioUnit.GasolineAirFuelRatio, ":1");
            UnitAbbreviationsCache.Default.MapUnitToDefaultAbbreviation(AirFuelRatioUnit.E85AirFuelRatio, ":1 (E85)");
        }

        public AirFuelRatio(double value, AirFuelRatioUnit unit)
        {
            Unit = unit;
            Value = value;
        }

        public static BaseDimensions BaseDimensions { get; }
        public static AirFuelRatio Zero { get; }
        public static QuantityInfo Info { get; }

        [Obsolete("This method is deprecated and will be removed at a future release")]
        public QuantityType Type => default;

        public BaseDimensions Dimensions => BaseDimensions;

        public QuantityInfo QuantityInfo => Info;


        public Enum Unit { get; }

        public double Value {get; }

        public double As(Enum unit)
        {
            return Convert.ToDouble(unit);
        }

        public double As(UnitSystem unitSystem)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider? provider)
        {
            var unitAbbriviation = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(typeof(AirFuelRatioUnit), Convert.ToInt32(Unit), provider);

            return $"{Value} {unitAbbriviation}";
        }

        [Obsolete(@"This method is deprecated and will be removed in the future.")]
        public string ToString(IFormatProvider? provider, int significantDigitsAfterRadix)
        {
            return ToString(provider);
        }
        
        [Obsolete(@"This method is deprecated and will be removed in the future.")]
        public string ToString(IFormatProvider? provider, string format, params object[] args)
        {
            return ToString(provider);
        }

        [Obsolete(@"This method is deprecated and will be removed in the future.")]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(formatProvider);
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public IQuantity ToUnit(Enum unit)
        {
            if (unit is not AirFuelRatioUnit airFuelRatioUnit)
                throw new ArgumentException($"unit must be of type {typeof(AirFuelRatioUnit)}.", nameof(unit));
                
            return new AirFuelRatio(As(unit), airFuelRatioUnit);
        }

        public IQuantity ToUnit(UnitSystem unitSystem)
        {
            throw new NotImplementedException();
        }
    }
}