using System.Linq;

using FizzWare.NBuilder;

using NUnit.Framework;

using UnitsNet.Metadata.DynamicDataframes;
using UnitsNet.Metadata.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.DynamicDataframes;

[TestFixture]
public class AsDynamicDataframes
{
    [TestCase(TestName = "{c} (with conversions)")]
    public void WithConversionsTest()
    {
        var obj = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4
        };

        var dataframe = new[] { obj }.AsDynamicDataframes()
            .WithConversion(b => b.Width, LengthUnit.Centimeter)
            .WithConversion(b => b.Height, LengthUnit.Centimeter)
            .WithConversion(b => b.Depth, LengthUnit.Centimeter)
            .WithConversion(b => b.Weight, MassUnit.Gram)
            .WithConversion(b => b.Volume, VolumeUnit.CubicDecimeter)
            .Build();

        var dynamicBox = dataframe.First();
        var metadata = dynamicBox.GetObjectMetadata();
        var collectionMetadata = dataframe.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.Count.EqualTo(6));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Width))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Centimeter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Height))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Centimeter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Depth))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Centimeter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Weight))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(MassUnit.Gram));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Volume))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(VolumeUnit.CubicDecimeter));

            Assert.That(dynamicBox, Has.Property(nameof(Box.Width)).EqualTo(100));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Height)).EqualTo(200));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Depth)).EqualTo(300));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Weight)).EqualTo(4000));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Volume)).EqualTo(6000));
        });
    }

    [TestCase(TestName = "{c} (with setters)")]
    public void WithSettersTest()
    {
        var obj = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4
        };

        var dataframe = new[] { obj }.AsDynamicDataframes()
            .WithConversion(b => b.Width, LengthUnit.Centimeter)
            .WithConversion(b => b.Height, LengthUnit.Centimeter)
            .WithConversion(b => b.Depth, LengthUnit.Centimeter)
            .WithConversion(b => b.Weight, MassUnit.Gram)
            .WithConversion(b => b.Volume, VolumeUnit.CubicDecimeter)
            .Build()
            .First();

        dataframe.Width = 200;
        dataframe.Height = 300;
        dataframe.Depth = 400;
        dataframe.Weight = 5000;

        Assert.Multiple(() =>
        {
            Assert.That(obj, Has.Property(nameof(Box.Width)).EqualTo(2));
            Assert.That(dataframe, Has.Property(nameof(Box.Width)).EqualTo(200));
            Assert.That(obj, Has.Property(nameof(Box.Height)).EqualTo(3));
            Assert.That(dataframe, Has.Property(nameof(Box.Height)).EqualTo(300));
            Assert.That(obj, Has.Property(nameof(Box.Depth)).EqualTo(4));
            Assert.That(dataframe, Has.Property(nameof(Box.Depth)).EqualTo(400));
            Assert.That(obj, Has.Property(nameof(Box.Weight)).EqualTo(5));
            Assert.That(dataframe, Has.Property(nameof(Box.Weight)).EqualTo(5000));
            Assert.That(obj, Has.Property(nameof(Box.Volume)).EqualTo(24));
            Assert.That(dataframe, Has.Property(nameof(Box.Volume)).EqualTo(24000));
        });
    }

    [TestCase(TestName = "{c} (with missing metadata)")]
    public void WithMissingMetadataTest()
    {
        Assert.That(() =>
            Builder<Star>.CreateListOfSize(100).Build().AsDynamicDataframes()
                .WithConversion(s => s.Number, ScalarUnit.Amount)
                .Build(),
            Throws.InvalidOperationException.With.Message.Match("Unit metadata does not exist for (.*)"));
    }

    [TestCase(TestName = "{c} (with non-virtual property conversions)")]
    public void WithNonVirtualPropertyConversionsTest()
    {
        Assert.Multiple(() =>
        {
            Assert.That(() =>
                Builder<Star>.CreateListOfSize(100).Build().AsDynamicDataframes()
                    .WithConversion(s => s.Mass, MassUnit.SolarMass)
                    .WithConversion(b => b.Radius, LengthUnit.SolarRadius)
                    .WithConversion(b => b.Distance, LengthUnit.AstronomicalUnit)
                    .WithConversion(b => b.Luminosity, LuminosityUnit.SolarLuminosity)
                    .Build(),
                Throws.InvalidOperationException.With.Message.Match("(.*) is non-virtual and cannot be converted to (.*)"));

            Assert.That(() =>
                Builder<TransmitterData>.CreateListOfSize(100).Build().AsDynamicDataframes()
                    .WithConversion(b => b.Power, PowerUnit.Milliwatt)
                    .WithConversion(b => b.Frequency, FrequencyUnit.Kilohertz)
                    .WithConversion(b => b.Temperature, TemperatureUnit.DegreeCelsius)
                    .Build(),
                Throws.InvalidOperationException.With.Message.Match("(.*) is non-virtual and cannot be converted to (.*)"));
        });
    }

    [TestCase(TestName = "{c} (with interface hoisted metadata)")]
    public void WithInterfaceHoistedMetadataTest()
    {
        var obj = new TransmitterData
        {
            Power = 2,
            Frequency = 3_000_000,
            Temperature = 100
        };

        var dataframes = new[] { obj }.AsDynamicDataframes()
            .WithConversion(b => b.Power, PowerUnit.Milliwatt)
            .WithConversion(b => b.Frequency, FrequencyUnit.Megahertz)
            .WithConversion(b => b.Temperature, TemperatureUnit.DegreeCelsius)
            .As<ITransmitterData>()
            .Build();

        var dataframe = dataframes.First();
        var metadata = dataframe.GetObjectMetadata();
        var collectionMetadata = dataframe.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.ItemAt(nameof(ITransmitterData.Power))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.Milliwatt));
            Assert.That(metadata, Has.ItemAt(nameof(ITransmitterData.Frequency))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(FrequencyUnit.Megahertz));
            Assert.That(metadata, Has.ItemAt(nameof(ITransmitterData.Temperature))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TemperatureUnit.DegreeCelsius));

            Assert.That(dataframe, Has.Property(nameof(TransmitterData.Power)).EqualTo(2000).Within(0.01));
            Assert.That(dataframe, Has.Property(nameof(TransmitterData.Frequency)).EqualTo(3).Within(0.01));
            Assert.That(dataframe, Has.Property(nameof(TransmitterData.Temperature)).EqualTo(-173.14).Within(0.01));
        });
    }

    [TestCase(TestName = "{c} (with virtual hoisted metadata)")]
    public void WithVirtualHoistedMetadataTest()
    {
        var obj = new AerospaceVehicleTelemetry
        {
            Efficency = 2,
            Speed = 10
        };

        var dataframes = new[] { obj }.AsDynamicDataframes()
            .WithConversion(t => t.Efficency, FuelEfficiencyUnit.MilePerUsGallon)
            .WithConversion(t => t.Speed, SpeedUnit.MilePerHour)
            .As<VehicleTelemetry>()
            .Build();

        var dataframe = dataframes.First();
        var metadata = dataframes.GetObjectMetadata();
        var collectionMetadata = dataframes.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Efficency))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(FuelEfficiencyUnit.MilePerUsGallon));
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Speed))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(SpeedUnit.MilePerHour));

            Assert.That(dataframe, Has.Property(nameof(MotorVehicleTelemetry.Efficency)).EqualTo(4.70).Within(0.01));
            Assert.That(dataframe, Has.Property(nameof(MotorVehicleTelemetry.Speed)).EqualTo(6.21).Within(0.01));
        });
    }

    [TestCase(TestName = "{c} (with virtual property metadata)")]
    public void WithVirtualPropertyMetadataTest()
    {
        var obj = new MotorVehicleTelemetry
        {
            Efficency = 2,
            Speed = 10
        };

        var dynamicTelemetryData = new[] { obj }.AsDynamicDataframes()
            .WithConversion(t => t.Efficency, FuelEfficiencyUnit.MilePerUsGallon)
            .WithConversion(t => t.Speed, SpeedUnit.MilePerHour)
            .Build();

        var dataframe = dynamicTelemetryData.First();
        var metadata = dataframe.GetObjectMetadata();
        var collectionMetadata = dataframe.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Efficency))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(FuelEfficiencyUnit.MilePerUsGallon));
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Speed))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(SpeedUnit.MilePerHour));

            Assert.That(dataframe, Has.Property(nameof(MotorVehicleTelemetry.Efficency)).EqualTo(4.7).Within(0.01));
            Assert.That(dataframe, Has.Property(nameof(MotorVehicleTelemetry.Speed)).EqualTo(6.21).Within(0.01));
        });
    }

    [TestCase(TestName = "{c} (with custom attribute)")]
    public void WithCustomAttributeTest()
    {
        var obj = new DynoData
        {
            Horsepower = 300,
            Torque = 200,
            Rpm = 6000
        };

        var dataframes = new[] { obj }.AsDynamicDataframes()
            .WithConversion(t => t.Horsepower, PowerUnit.Kilowatt)
            .WithConversion(t => t.Torque, TorqueUnit.NewtonMeter)
            .Build();

        var dataframe = dataframes.First();
        var metadata = dataframe.GetObjectMetadata();
        var collectionMetadata = dataframe.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Horsepower))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.Kilowatt));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Torque))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TorqueUnit.NewtonMeter));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Rpm))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute));

            Assert.That(obj.ConvertQuantity(d => d.Horsepower, to: PowerUnit.Kilowatt), Has
                .Property(nameof(IQuantity.Value)).EqualTo(223.7).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(PowerUnit.Kilowatt));
            Assert.That(obj.ConvertQuantity("Torque", to: TorqueUnit.NewtonMeter), Has
                .Property(nameof(IQuantity.Value)).EqualTo(271.16).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(TorqueUnit.NewtonMeter));
        });
    }

    [TestCase(TestName = "{c} (typed with custom attribute)")]
    public void TypedWithCustomAttributeTest()
    {
        var obj = new DynoData
        {
            Horsepower = 300,
            Torque = 200,
            Rpm = 6000
        };

        var dataframes = new[] { obj }.AsDynamicDataframes<DynoData, DynoMeasurementAttribute, DynoMetadata>()
            .WithConversion(t => t.Horsepower, PowerUnit.Kilowatt)
            .WithConversion(t => t.Torque, TorqueUnit.NewtonMeter)
            .Build();

        var dataframe = dataframes.First();
        var metadata = dataframe.GetObjectMetadata<DynoData, DynoMeasurementAttribute, DynoMetadata>();
        var collectionMetadata = dataframes.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Horsepower))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.Kilowatt).And
                .ItemAt(nameof(DynoData.Horsepower)).Property(nameof(DynoMetadata.DisplayName)).EqualTo("Engine Horsepower"));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Torque))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TorqueUnit.NewtonMeter).And
                .ItemAt(nameof(DynoData.Torque)).Property(nameof(DynoMetadata.DisplayName)).EqualTo("Engine Torque"));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Rpm))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute).And
                .ItemAt(nameof(DynoData.Rpm)).Property(nameof(DynoMetadata.DisplayName)).EqualTo("Engine Speed"));

            Assert.That(obj.ConvertQuantity(d => d.Horsepower, to: PowerUnit.Kilowatt), Has
                .Property(nameof(IQuantity.Value)).EqualTo(223.7).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(PowerUnit.Kilowatt));
            Assert.That(obj.ConvertQuantity("Torque", to: TorqueUnit.NewtonMeter), Has
                .Property(nameof(IQuantity.Value)).EqualTo(271.16).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(TorqueUnit.NewtonMeter));
        });
    }
}
