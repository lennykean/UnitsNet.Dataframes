using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Dataframes.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.DataframeExtensions;

[TestFixture]
public class GetDataframeMetadata
{
    [TestCase(TestName = "{c} (gets metadata)")]
    public void GetDataframeMetadataTests()
    {
        var box = new Box();

        var metadata = box.GetDataframeMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(metadata, Has.Count.EqualTo(7));
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
            Assert.That(metadata, Has.ItemAt(nameof(Box.Data))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(InformationUnit.Bit));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Volume))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(VolumeUnit.CubicMeter));
        });
    }
}
