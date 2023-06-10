using System.Collections.Generic;
using System.Linq;

using FizzWare.NBuilder;

using NUnit.Framework;

using UnitsNet.Dataframes.Dynamic;
using UnitsNet.Dataframes.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.Dynamic;

[TestFixture]
public class AsDynamicDataframes
{
    [TestCase(TestName = "{c} (with conversions)")]
    public void WithConversionsTest()
    {
        var box = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4
        };

        var dynamicBoxes = new[] { box }.AsDynamicDataframes()
            .WithConversion(b => b.Width, LengthUnit.Centimeter)
            .WithConversion(b => b.Height, LengthUnit.Centimeter)
            .WithConversion(b => b.Depth, LengthUnit.Centimeter)
            .WithConversion(b => b.Weight, MassUnit.Gram)
            .WithConversion(b => b.Volume, VolumeUnit.CubicDecimeter)
            .Build();

        var dynamicBox = dynamicBoxes.First();
        var metadata = dynamicBox.GetDataframeMetadata();
        var collectionMetadata = dynamicBoxes.GetDataframeMetadata();

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
        var box = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4
        };

        var dynamicBox = new[] { box }.AsDynamicDataframes()
            .WithConversion(b => b.Width, LengthUnit.Centimeter)
            .WithConversion(b => b.Height, LengthUnit.Centimeter)
            .WithConversion(b => b.Depth, LengthUnit.Centimeter)
            .WithConversion(b => b.Weight, MassUnit.Gram)
            .WithConversion(b => b.Volume, VolumeUnit.CubicDecimeter)
            .Build()
            .First();

        dynamicBox.Width = 200;
        dynamicBox.Height = 300;
        dynamicBox.Depth = 400;
        dynamicBox.Weight = 5000;

        Assert.Multiple(() =>
        {
            Assert.That(box, Has.Property(nameof(Box.Width)).EqualTo(2));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Width)).EqualTo(200));
            Assert.That(box, Has.Property(nameof(Box.Height)).EqualTo(3));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Height)).EqualTo(300));
            Assert.That(box, Has.Property(nameof(Box.Depth)).EqualTo(4));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Depth)).EqualTo(400));
            Assert.That(box, Has.Property(nameof(Box.Weight)).EqualTo(5));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Weight)).EqualTo(5000));
            Assert.That(box, Has.Property(nameof(Box.Volume)).EqualTo(24));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Volume)).EqualTo(24000));
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
                Builder<TransmitterDataFrame>.CreateListOfSize(100).Build().AsDynamicDataframes()
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
        var transmitterDataFrame = new TransmitterDataFrame
        {
            Power = 2,
            Frequency = 3_000_000,
            Temperature = 100
        };

        var transmitterDataFrames = new[] { transmitterDataFrame }.AsDynamicDataframes()
            .WithConversion(b => b.Power, PowerUnit.Milliwatt)
            .WithConversion(b => b.Frequency, FrequencyUnit.Megahertz)
            .WithConversion(b => b.Temperature, TemperatureUnit.DegreeCelsius)
            .As<ITransmitterData>()
            .Build();

        var dynamicTransmitterDataFrame = transmitterDataFrames.First();
        var metadata = dynamicTransmitterDataFrame.GetDataframeMetadata();
        var collectionMetadata = dynamicTransmitterDataFrame.GetDataframeMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.ItemAt(nameof(ITransmitterData.Power))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.Milliwatt));
            Assert.That(metadata, Has.ItemAt(nameof(ITransmitterData.Frequency))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(FrequencyUnit.Megahertz));
            Assert.That(metadata, Has.ItemAt(nameof(ITransmitterData.Temperature))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TemperatureUnit.DegreeCelsius));

            Assert.That(dynamicTransmitterDataFrame, Has.Property(nameof(TransmitterDataFrame.Power)).EqualTo(2000).Within(0.01));
            Assert.That(dynamicTransmitterDataFrame, Has.Property(nameof(TransmitterDataFrame.Frequency)).EqualTo(3).Within(0.01));
            Assert.That(dynamicTransmitterDataFrame, Has.Property(nameof(TransmitterDataFrame.Temperature)).EqualTo(-173.14).Within(0.01));
        });
    }

    [TestCase(TestName = "{c} (with virtual hoisted metadata)")]
    public void WithVirtualHoistedMetadataTest()
    {
        var telemetryDataFrame = new AerospaceVehicleTelemetry
        {
            Efficency = 2,
            Speed = 10
        };

        var dynamicTelemetryDataFrames = new[] { telemetryDataFrame }.AsDynamicDataframes()
            .WithConversion(t => t.Efficency, FuelEfficiencyUnit.MilePerUsGallon)
            .WithConversion(t => t.Speed, SpeedUnit.MilePerHour)
            .As<VehicleTelemetry>()
            .Build();

        var dynamicTelemetryDataFrame = dynamicTelemetryDataFrames.First();
        var metadata = dynamicTelemetryDataFrames.GetDataframeMetadata();
        var collectionMetadata = dynamicTelemetryDataFrames.GetDataframeMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Efficency))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(FuelEfficiencyUnit.MilePerUsGallon));
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Speed))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(SpeedUnit.MilePerHour));

            Assert.That(dynamicTelemetryDataFrame, Has.Property(nameof(MotorVehicleTelemetry.Efficency)).EqualTo(4.70).Within(0.01));
            Assert.That(dynamicTelemetryDataFrame, Has.Property(nameof(MotorVehicleTelemetry.Speed)).EqualTo(6.21).Within(0.01));
        });
    }

    [TestCase(TestName = "{c} (with virtual property metadata)")]
    public void WithVirtualPropertyMetadataTest()
    {
        var telemetryDataFrame = new MotorVehicleTelemetry
        {
            Efficency = 2,
            Speed = 10
        };

        var dynamicTelemetryData = new[] { telemetryDataFrame }.AsDynamicDataframes()
            .WithConversion(t => t.Efficency, FuelEfficiencyUnit.MilePerUsGallon)
            .WithConversion(t => t.Speed, SpeedUnit.MilePerHour)
            .Build();

        var dynamicTelemetryDataFrame = dynamicTelemetryData.First();
        var metadata = dynamicTelemetryDataFrame.GetDataframeMetadata();
        var collectionMetadata = dynamicTelemetryDataFrame.GetDataframeMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Efficency))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(FuelEfficiencyUnit.MilePerUsGallon));
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Speed))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(SpeedUnit.MilePerHour));

            Assert.That(dynamicTelemetryDataFrame, Has.Property(nameof(MotorVehicleTelemetry.Efficency)).EqualTo(4.7).Within(0.01));
            Assert.That(dynamicTelemetryDataFrame, Has.Property(nameof(MotorVehicleTelemetry.Speed)).EqualTo(6.21).Within(0.01));
        });
    }

    [TestCase(TestName = "{c} (with custom attribute)")]
    public void WithCustomAttributeTest()
    {
        var dynoDataframe = new DynoDataframe
        {
            Horsepower = 300,
            Torque = 200,
            Rpm = 6000
        };

        var dynamicDynoData = new[] { dynoDataframe }.AsDynamicDataframes()
            .WithConversion(t => t.Horsepower, PowerUnit.Kilowatt)
            .WithConversion(t => t.Torque, TorqueUnit.NewtonMeter)
            .Build();

        var dynamicTelemetryDataFrame = dynamicDynoData.First();
        var metadata = dynamicTelemetryDataFrame.GetDataframeMetadata();
        var collectionMetadata = dynamicTelemetryDataFrame.GetDataframeMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Horsepower))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.Kilowatt));
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Torque))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TorqueUnit.NewtonMeter));
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Rpm))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute));

            Assert.That(dynoDataframe.ConvertQuantity(d => d.Horsepower, to: PowerUnit.Kilowatt), Has
                .Property(nameof(IQuantity.Value)).EqualTo(223.7).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(PowerUnit.Kilowatt));
            Assert.That(dynoDataframe.ConvertQuantity("Torque", to: TorqueUnit.NewtonMeter), Has
                .Property(nameof(IQuantity.Value)).EqualTo(271.16).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(TorqueUnit.NewtonMeter));
        });
    }

    [TestCase(TestName = "{c} (typed with custom attribute)")]
    public void TypedWithCustomAttributeTest()
    {
        var dynoDataframe = new DynoDataframe
        {
            Horsepower = 300,
            Torque = 200,
            Rpm = 6000
        };

        var dynamicDynoDataframes = new[] { dynoDataframe }.AsDynamicDataframes<DynoDataframe, DynoMeasurementAttribute, DynoMetadata>()
            .WithConversion(t => t.Horsepower, PowerUnit.Kilowatt)
            .WithConversion(t => t.Torque, TorqueUnit.NewtonMeter)
            .Build();

        var dynamicDynoDataframe = dynamicDynoDataframes.First();
        var metadata = dynamicDynoDataframe.GetDataframeMetadata<DynoDataframe, DynoMeasurementAttribute, DynoMetadata>();
        var collectionMetadata = dynamicDynoDataframes.GetDataframeMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Horsepower))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.Kilowatt).And
                .ItemAt(nameof(DynoDataframe.Horsepower)).Property(nameof(DynoMetadata.DisplayName)).EqualTo("Engine Horsepower"));
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Torque))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TorqueUnit.NewtonMeter).And
                .ItemAt(nameof(DynoDataframe.Torque)).Property(nameof(DynoMetadata.DisplayName)).EqualTo("Engine Torque"));
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Rpm))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute).And
                .ItemAt(nameof(DynoDataframe.Rpm)).Property(nameof(DynoMetadata.DisplayName)).EqualTo("Engine Speed"));

            Assert.That(dynoDataframe.ConvertQuantity(d => d.Horsepower, to: PowerUnit.Kilowatt), Has
                .Property(nameof(IQuantity.Value)).EqualTo(223.7).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(PowerUnit.Kilowatt));
            Assert.That(dynoDataframe.ConvertQuantity("Torque", to: TorqueUnit.NewtonMeter), Has
                .Property(nameof(IQuantity.Value)).EqualTo(271.16).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(TorqueUnit.NewtonMeter));
        });
    }
}
