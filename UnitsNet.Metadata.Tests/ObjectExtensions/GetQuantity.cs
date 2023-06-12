using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Metadata.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.ObjectExtensions;

[TestFixture]
public class GetQuantity
{
    [TestCase(TestName = "{c} (with valid metadata)")]
    public void WithValidMetadataTest()
    {
        var obj = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4,
            Items = 5
        };

        var width = obj.GetQuantity("Width");
        var height = obj.GetQuantity(b => b.Height);
        var depth = obj.GetQuantity("Depth");
        var weight = obj.GetQuantity(b => b.Weight);
        var volume = obj.GetQuantity(b => b.Volume);
        var items = obj.GetQuantity(b => b.Items);

        Assert.Multiple(() =>
        {
            Assert.That(width, Has
                .Property(nameof(IQuantity.Value)).EqualTo(1).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(height, Has
                .Property(nameof(IQuantity.Value)).EqualTo(2).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(depth, Has
                .Property(nameof(IQuantity.Value)).EqualTo(3).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(weight, Has
                .Property(nameof(IQuantity.Value)).EqualTo(4).And
                .Property(nameof(IQuantity.Unit)).EqualTo(MassUnit.Kilogram));
            Assert.That(items, Has
                .Property(nameof(IQuantity.Value)).EqualTo(5).And
                .Property(nameof(IQuantity.Unit)).EqualTo(ScalarUnit.Amount));
            Assert.That(volume, Has
                .Property(nameof(IQuantity.Value)).EqualTo(6).And
                .Property(nameof(IQuantity.Unit)).EqualTo(VolumeUnit.CubicMeter));
        });
    }

    [TestCase(TestName = "{c} (with missing metadata)")]
    public void WithMissingMetadataTest()
    {
        var obj = new Box
        {
            SerialNumber = 1,
            Priority = 2,
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => obj.GetQuantity(b => b.SerialNumber), Throws.InvalidOperationException.With.Message.Match("Unit metadata does not exist for (.*)."));
            Assert.That(() => obj.GetQuantity("Priority"), Throws.InvalidOperationException.With.Message.Match("Unit metadata does not exist for (.*)."));
        });
    }

    [TestCase(TestName = "{c} (with invalid quantity)")]
    public void WithInvalidQuantityTest()
    {
        var obj = new Blob();

        Assert.That(() => obj.GetQuantity("Data"), Throws.InvalidOperationException.With.Message.Match("Type of (.*) is not a valid quantity type"));
    }

    [TestCase(TestName = "{c} (with missing property)")]
    public void WithMissingPropertyTest()
    {
        var obj = new Blob();

        Assert.That(() => obj.GetQuantity("FakeProperty"), Throws.InvalidOperationException.With.Message.Match("(.*) is not a property of (.*)"));
    }

    [TestCase(TestName = "{c} (with invalid attribute)")]
    public void WithInvalidAttributeTest()
    {
        var obj = new Garbage();

        Assert.That(() => obj.GetQuantity(r => r.Odor), Throws.ArgumentException.With.Message.EqualTo("Unit must be an enum value"));
    }

    [TestCase(TestName = "{c} (with custom unit)")]
    public void WithCustomUnitTest()
    {
        var obj = new Employee
        {
            Name = "Cubert",
            Coolness = 40
        };

        Assert.Multiple(() =>
        {
            Assert.That(obj.GetQuantity(e => e.Coolness), Has
                .Property(nameof(IQuantity.Value)).EqualTo(40).And
                .Property(nameof(IQuantity.Unit)).EqualTo(CoolnessUnit.MegaFonzie));
            Assert.That(obj.GetQuantity("Coolness"), Has
                .Property(nameof(IQuantity.Value)).EqualTo(40).And
                .Property(nameof(IQuantity.Unit)).EqualTo(CoolnessUnit.MegaFonzie));
        });
    }

    [TestCase(TestName = "{c} (with invalid custom unit)")]
    public void WithInvalidCustomUnitTest()
    {
        var obj = new Rubbish
        {
            Coolness = 40
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => obj.GetQuantity(r => r.Coolness), Throws.ArgumentException.With.Message.Match("(.*) is not a known unit value"));
            Assert.That(() => obj.GetQuantity("Coolness"), Throws.ArgumentException.With.Message.Match("(.*) is not a known unit value"));
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

        Assert.Multiple(() =>
        {
            Assert.That(obj.GetQuantity(d => d.Horsepower), Has
                .Property(nameof(IQuantity.Value)).EqualTo(300).And
                .Property(nameof(IQuantity.Unit)).EqualTo(PowerUnit.MechanicalHorsepower));
            Assert.That(obj.GetQuantity("Torque"), Has
                .Property(nameof(IQuantity.Value)).EqualTo(200).And
                .Property(nameof(IQuantity.Unit)).EqualTo(TorqueUnit.PoundForceFoot));
            Assert.That(obj.GetQuantity(d => d.Rpm), Has
                .Property(nameof(IQuantity.Value)).EqualTo(6000).And
                .Property(nameof(IQuantity.Unit)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute));
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

        Assert.Multiple(() =>
        {
            Assert.That(obj.GetQuantity<DynoData, DynoMeasurementAttribute, DynoMetadata>(d => d.Horsepower), Has
                .Property(nameof(IQuantity.Value)).EqualTo(300).And
                .Property(nameof(IQuantity.Unit)).EqualTo(PowerUnit.MechanicalHorsepower));
            Assert.That(obj.GetQuantity<DynoData, DynoMeasurementAttribute, DynoMetadata>("Torque"), Has
                .Property(nameof(IQuantity.Value)).EqualTo(200).And
                .Property(nameof(IQuantity.Unit)).EqualTo(TorqueUnit.PoundForceFoot));
            Assert.That(obj.GetQuantity<DynoData, DynoMeasurementAttribute, DynoMetadata>(d => d.Rpm), Has
                .Property(nameof(IQuantity.Value)).EqualTo(6000).And
                .Property(nameof(IQuantity.Unit)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute));
        });
    }
}
