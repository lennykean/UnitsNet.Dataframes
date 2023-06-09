using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Dataframes.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.DataframeExtensions;

[TestFixture]
public class GetDataframeMetadata
{
    [TestCase(TestName = "{c} (gets metadata)")]
    public void GetMetadataTest()
    {
        var box = new Box();

        var metadata = box.GetDataframeMetadata();

        Assert.Multiple(() =>
        {
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

    [TestCase(TestName = "{c} (throws exception on invalid datatype)")]
    public void InvalidDataTypeTest()
    {
        var blob = new Blob
        {
            Data = "1"
        };

        Assert.That(() => blob.GetDataframeMetadata(), Throws.InvalidOperationException.With.Message.Match("Type of (.*) \\((.*)\\) is not a valid quantity type"));
    }

    [TestCase(TestName = "{c} (throws exception on invalid attribute)")]
    public void InvalidAttributeTest()
    {
        var garbage = new Garbage();

        Assert.That(() => garbage.GetDataframeMetadata(), Throws.ArgumentException.With.Message.EqualTo("Unit must be an enum value"));
    }
}
