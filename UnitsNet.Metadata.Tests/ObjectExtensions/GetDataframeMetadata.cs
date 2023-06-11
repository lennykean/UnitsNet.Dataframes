using System.Collections.Generic;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Metadata.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.ObjectExtensions;

[TestFixture]
public class GetDataframeMetadata
{
    [TestCase(TestName = "{c} (with valid metadata)")]
    public void WithValidMetadataTest()
    {
        var box = new Box();

        var metadata = box.GetDataframeMetadata();
        var collectionMetadata = new List<Box> { box }.GetDataframeMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.Count.EqualTo(6));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Width))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Height))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Depth))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Weight))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(MassUnit.Kilogram));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Items))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(ScalarUnit.Amount));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Volume))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(VolumeUnit.CubicMeter));
        });
    }

    [TestCase(TestName = "{c} (with invalid quantity)")]
    public void WithInvalidQuantityTest()
    {
        var blob = new Blob
        {
            Data = "1"
        };

        Assert.That(() => blob.GetDataframeMetadata(), Throws.InvalidOperationException.With.Message.Match("Type of (.*) \\((.*)\\) is not a valid quantity type"));
    }

    [TestCase(TestName = "{c} (with invalid attribute)")]
    public void WithInvalidAttributeTest()
    {
        var garbage = new Garbage();

        Assert.That(() => garbage.GetDataframeMetadata(), Throws.ArgumentException.With.Message.EqualTo("Unit must be an enum value"));
    }

    [TestCase(TestName = "{c} (with custom unit)")]
    public void WithCustomUnitTest()
    {
        var employee = new Employee();

        var metadata = employee.GetDataframeMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(metadata, Has.Count.EqualTo(1));
            Assert.That(metadata, Has.ItemAt(nameof(employee.Coolness))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(CoolnessUnit.MegaFonzie));
        });
    }

    [TestCase(TestName = "{c} (with invalid custom unit)")]
    public void WithInvlidCustomUnitTest()
    {
        var rubbish = new Rubbish
        {
            Coolness = 40
        };

        Assert.That(() => rubbish.GetDataframeMetadata(), Throws.ArgumentException.With.Message.Match("(.*) is not a known unit value"));
    }

    [TestCase(TestName = "{c} (with custom attribute)")]
    public void WithCustomAttributeTest()
    {
        var dynoDataframe = new DynoDataframe();

        var metadata = dynoDataframe.GetDataframeMetadata();
        var collectionMetadata = new[] { dynoDataframe }.GetDataframeMetadata();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.Count.EqualTo(3));
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Horsepower))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.MechanicalHorsepower));
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Torque))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TorqueUnit.PoundForceFoot));
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Rpm))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute));
        });
    }

    [TestCase(TestName = "{c} (typed with custom attribute)")]
    public void TypedWithCustomAttributeTest()
    {
        var dynoDataframe = new DynoDataframe();

        var metadata = dynoDataframe.GetDataframeMetadata<DynoDataframe, DynoMeasurementAttribute, DynoMetadata>();
        var collectionMetadata = new List<DynoDataframe> { dynoDataframe }.GetDataframeMetadata<IEnumerable<DynoDataframe>, DynoMeasurementAttribute, DynoMetadata>();

        Assert.Multiple(() =>
        {
            CollectionAssert.AreEquivalent(metadata, collectionMetadata);
            Assert.That(metadata, Has.Count.EqualTo(3));
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Horsepower))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.MechanicalHorsepower).And
                .ItemAt(nameof(DynoDataframe.Horsepower)).Property(nameof(DynoMetadata.DisplayName)).EqualTo("Engine Horsepower"));
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Torque))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TorqueUnit.PoundForceFoot).And
                .ItemAt(nameof(DynoDataframe.Torque)).Property(nameof(DynoMetadata.DisplayName)).EqualTo("Engine Torque"));
            Assert.That(metadata, Has.ItemAt(nameof(DynoDataframe.Rpm))
                .Property(nameof(DynoMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute).And
                .ItemAt(nameof(DynoDataframe.Rpm)).Property(nameof(DynoMetadata.DisplayName)).EqualTo("Engine Speed"));
        });
    }
}
