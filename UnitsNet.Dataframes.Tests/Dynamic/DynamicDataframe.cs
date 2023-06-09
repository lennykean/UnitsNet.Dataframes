using System.Linq;

using FizzWare.NBuilder;

using NUnit.Framework;

using UnitsNet.Dataframes.Dynamic;
using UnitsNet.Dataframes.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.Dynamic;

[TestFixture]
public class DynamicDataframe
{
    [TestCase(TestName = "{c} (dynamically creates metadata)")]
    public void DynamicallyConvertUnitTest()
    {
        var boxes = Builder<Box>.CreateListOfSize(100).Build();

        var dynamicBoxes = boxes.AsDynamicDataframes()
            .WithConversion(b => b.Width, LengthUnit.Centimeter)
            .WithConversion(b => b.Height, LengthUnit.Centimeter)
            .WithConversion(b => b.Depth, LengthUnit.Centimeter)
            .WithConversion(b => b.Weight, MassUnit.Gram)
            .WithConversion(b => b.Volume, VolumeUnit.CubicDecimeter)
            .Build();

        var metadata = dynamicBoxes.GetDataframeMetadata();

        Assert.Multiple(() =>
        {
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
        });
    }

    [TestCase(TestName = "{c} (dynamically converts properties)")]
    public void DynamicallyConvertPropertyTest()
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

        Assert.Multiple(() =>
        {
            Assert.That(dynamicBox, Has.Property(nameof(Box.Width)).EqualTo(100));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Height)).EqualTo(200));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Depth)).EqualTo(300));
            Assert.That(dynamicBox, Has.Property(nameof(Box.Weight)).EqualTo(4000)); ;
            Assert.That(dynamicBox, Has.Property(nameof(Box.Volume)).EqualTo(6000));
        });
    }

    [TestCase(TestName = "{c} (dynamically converts property setters)")]
    public void DynamicallyConvertSetterTest()
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

    [TestCase(TestName = "{c} (throws exception on non-virtual property)")]
    public void NonVirtualPropertyTest()
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
                Builder<SensorData>.CreateListOfSize(100).Build().AsDynamicDataframes()
                    .WithConversion(b => b.Power, PowerUnit.Milliwatt)
                    .WithConversion(b => b.Frequency, FrequencyUnit.Kilohertz)
                    .WithConversion(b => b.Temperature, TemperatureUnit.DegreeCelsius)
                    .Build(),
                Throws.InvalidOperationException.With.Message.Match("(.*) is non-virtual and cannot be converted to (.*)"));
        });
    }

    [TestCase(TestName = "{c} (throws exception on missing metadata)")]
    public void MissingMetadataTest()
    {
        Assert.That(() =>
            Builder<Star>.CreateListOfSize(100).Build().AsDynamicDataframes()
                .WithConversion(s => s.Number, ScalarUnit.Amount)
                .Build(),
            Throws.InvalidOperationException.With.Message.Match("Unit metadata does not exist for (.*)"));
    }
}
