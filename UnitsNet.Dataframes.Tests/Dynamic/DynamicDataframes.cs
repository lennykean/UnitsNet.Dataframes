using System.Linq;

using FizzWare.NBuilder;

using NUnit.Framework;

using UnitsNet.Dataframes.Dynamic;
using UnitsNet.Dataframes.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.Dynamic;

[TestFixture]
public class DynamicDataframes
{
    [TestCase(TestName = "{c} (dynamically creates metadata)")]
    public void DynamicDataframesDynamicallyConvertsUnits() 
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
    public void DynamicDataframesDynamicallyConvertsProperties()
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
            Assert.That(dynamicBox, Has.Property(nameof(Box.Weight)).EqualTo(4000));;
            Assert.That(dynamicBox, Has.Property(nameof(Box.Volume)).EqualTo(6000));
        });
    }
}
