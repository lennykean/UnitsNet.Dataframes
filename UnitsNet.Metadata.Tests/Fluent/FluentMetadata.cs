using NUnit.Framework;

using UnitsNet.Metadata.Fluent;
using UnitsNet.Metadata.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.Fluent;

[TestFixture]
public class FluentMetadata
{
    [TestCase(TestName = "{c} (with valid metadata)")]
    public void WithValidMetadata()
    {
        var metadataProvider = MetadataBuilder
            .CreateFor<Cylinder>()
            .With(c => c.Radius, asUnit: LengthUnit.Meter)
            .With("Height", asUnit: LengthUnit.Meter)
            .With(c => c.Volume, asUnit: VolumeUnit.CubicMeter)
            .Build(global: false);

        var obj = new Cylinder
        {
            Radius = .1,
            Height = .5
        };

        var metadata = metadataProvider.GetObjectMetadata(obj);

        Assert.Multiple(() =>
        {
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Radius))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Height))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Volume))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(VolumeUnit.CubicMeter));

            Assert.That(metadataProvider.GetQuantity(obj, c => c.Radius), Has
                .Property(nameof(IQuantity.Value)).EqualTo(0.1).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(metadataProvider.GetQuantity(obj, "Height"), Has
                .Property(nameof(IQuantity.Value)).EqualTo(.5).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(metadataProvider.GetQuantity(obj, c => c.Volume), Has
                .Property(nameof(IQuantity.Value)).EqualTo(0.016).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(VolumeUnit.CubicMeter));
        });
    }

    [TestCase(TestName = "{c} (with allowed conversions)")]
    public void WithAllowedConversions()
    {
        var metadataProvider = MetadataBuilder
            .CreateFor<Cylinder>()
            .With(c => c.Radius, asUnit: LengthUnit.Meter)
                .WithAllowedConversion(to: LengthUnit.Millimeter)
                .WithAllowedConversion(to: LengthUnit.Centimeter)
            .With(c => c.Height, asUnit: LengthUnit.Meter)
                .WithAllowedConversion(to: LengthUnit.Millimeter)
                .WithAllowedConversion(to: LengthUnit.Centimeter)
            .With(c => c.Volume, asUnit: VolumeUnit.CubicMeter)
                .WithAllowedConversion(to: VolumeUnit.CubicMillimeter)
                .WithAllowedConversion(to: VolumeUnit.CubicCentimeter)
            .Build(global: false);

        var obj = new Cylinder
        {
            Radius = .1,
            Height = .5
        };

        var metadata = metadataProvider.GetObjectMetadata(obj);

        Assert.Multiple(() =>
        {
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Radius))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Height))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Volume))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata.UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(VolumeUnit.CubicMeter));

            Assert.That(metadataProvider.GetQuantity(obj, c => c.Radius), Has
                .Property(nameof(IQuantity.Value)).EqualTo(0.1).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(metadataProvider.GetQuantity(obj, "Height"), Has
                .Property(nameof(IQuantity.Value)).EqualTo(.5).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(metadataProvider.GetQuantity(obj, c => c.Volume), Has
                .Property(nameof(IQuantity.Value)).EqualTo(0.016).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(VolumeUnit.CubicMeter));
        });
    }
}
