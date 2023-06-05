using System;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests;

[TestFixture]
public class DataframeExtensionsTests
{
    [TestCase(TestName = $"{nameof(DataframeExtensions.GetQuantity)}")]
    public void GetQuantityTest()
    {
        var box = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4,
            Items = 5
        };

        var width = box.GetQuantity<Box, Length>("Width");
        var height = box.GetQuantity<Box, Length>(b => b.Height);
        var depth = box.GetQuantity("Depth");
        var weight = box.GetQuantity(b => b.Weight);
        var volume = box.GetQuantity(b => b.Volume);
        var items = box.GetQuantity(b => b.Items);

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
    
    [TestCase(TestName = $"{nameof(DataframeExtensions.GetQuantity)} (missing metadata)")]
    public void GetQuantityMissingMetadataTest()
    {
        var box = new Box
        {
            SerialNumber = 1,
            Priority = 2,
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => box.GetQuantity(b => b.SerialNumber),
                Throws.InvalidOperationException.With.Message.EqualTo("Unit metadata does not exist for Box.SerialNumber."));
            Assert.That(() => box.GetQuantity("Priority"),
                Throws.InvalidOperationException.With.Message.EqualTo("Unit metadata does not exist for Box.Priority."));
        });
    }

    [TestCase(TestName = $"{nameof(DataframeExtensions.GetQuantity)} (invalid datatype)")]
    public void GetQuantityInvalidDatatypeTest()
    {
        var box = new Box
        {
            SerialNumber = 1,
            Priority = 2,
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => box.GetQuantity("Data"),
                Throws.InvalidOperationException.With.Message.EqualTo("UnitsNet.Dataframes.Tests.Box.Data type of System.String is not compatible with UnitsNet.QuantityValue."));
        });
    }

    public record ConvertQuantityTestData(double Value, double Converted, Enum Unit);
    public readonly static TestCaseData[] ConvertQuantityTestCases = new[]
    {
        new TestCaseData(
            new ConvertQuantityTestData(1, 1, LengthUnit.Meter),
            new ConvertQuantityTestData(2, 2, LengthUnit.Meter),
            new ConvertQuantityTestData(3, 3, LengthUnit.Meter),
            new ConvertQuantityTestData(4, 4, MassUnit.Kilogram),
            new ConvertQuantityTestData(0, 6, VolumeUnit.CubicMeter)).SetName($"{nameof(DataframeExtensions.ConvertQuantity)} (self conversions)"),
        new TestCaseData(
            new ConvertQuantityTestData(1, 10, LengthUnit.Decimeter),
            new ConvertQuantityTestData(2, 200, LengthUnit.Centimeter),
            new ConvertQuantityTestData(3, 3000, LengthUnit.Millimeter),
            new ConvertQuantityTestData(4, 4000, MassUnit.Gram),
            new ConvertQuantityTestData(0, 6000, VolumeUnit.CubicDecimeter)).SetName($"{nameof(DataframeExtensions.ConvertQuantity)} (unit conversions)")
    };

    [TestCaseSource(nameof(ConvertQuantityTestCases))]
    public void ConvertQuantityTest(
        ConvertQuantityTestData width,
        ConvertQuantityTestData height,
        ConvertQuantityTestData depth,
        ConvertQuantityTestData weight,
        ConvertQuantityTestData volume)
    {
        var box = new Box
        {
            Width = width.Value,
            Height = height.Value,
            Depth = depth.Value,
            Weight = weight.Value
        };

        var widthQuantity = box.ConvertQuantity<Box, Length>("Width", to: width.Unit);
        var heightQuantity = box.ConvertQuantity<Box, Length>(b => b.Height, to: height.Unit);
        var depthQuantity = box.ConvertQuantity("Depth", to: depth.Unit);
        var volumeQuantity = box.ConvertQuantity(b => b.Volume, to: volume.Unit);
        var weightQuantity = box.ConvertQuantity(b => b.Weight, to: weight.Unit);

        Assert.Multiple(() =>
        {
            Assert.That(widthQuantity, Has
                .Property(nameof(IQuantity.Value)).EqualTo(width.Converted).Within(0.001).And
                .Property(nameof(IQuantity.Unit)).EqualTo(width.Unit));
            Assert.That(heightQuantity, Has
                .Property(nameof(IQuantity.Value)).EqualTo(height.Converted).Within(0.001).And
                .Property(nameof(IQuantity.Unit)).EqualTo(height.Unit));
            Assert.That(depthQuantity, Has
                .Property(nameof(IQuantity.Value)).EqualTo(depth.Converted).Within(0.001).And
                .Property(nameof(IQuantity.Unit)).EqualTo(depth.Unit));
            Assert.That(volumeQuantity, Has
                .Property(nameof(IQuantity.Value)).EqualTo(volume.Converted).Within(0.001).And
                .Property(nameof(IQuantity.Unit)).EqualTo(volume.Unit));
            Assert.That(weightQuantity, Has
                .Property(nameof(IQuantity.Value)).EqualTo(weight.Converted).Within(0.001).And
                .Property(nameof(IQuantity.Unit)).EqualTo(weight.Unit));
        });
    }

    [TestCase(TestName = $"{nameof(DataframeExtensions.ConvertQuantity)} (invalid unit conversion)")]
    public void ConvertQuantityInvalidUnitConversionTest()
    {
        var box = new Box
        {
            Width = 1,
            Height = 2
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => box.ConvertQuantity(b => b.Width, to: AngleUnit.Degree),
                Throws.InvalidOperationException.With.Message.EqualTo("Box.Width (Meter) cannot be converted to Degree."));
            Assert.That(() => box.ConvertQuantity("Height", to: SpeedUnit.Mach),
                Throws.InvalidOperationException.With.Message.EqualTo("Box.Height (Meter) cannot be converted to Mach."));
        });
    }

    [TestCase(TestName = $"{nameof(DataframeExtensions.ConvertQuantity)} (disallowed unit conversion)")]
    public void ConvertQuantityDisallowedUnitConversionTest()
    {
        var box = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => box.ConvertQuantity(b => b.Volume, to: VolumeUnit.CubicFoot),
                Throws.InvalidOperationException.With.Message.EqualTo("Box.Volume (CubicMeter) cannot be converted to CubicFoot."));
            Assert.That(() => box.ConvertQuantity("Height", to: MassUnit.Pound),
                Throws.InvalidOperationException.With.Message.EqualTo("Box.Height (Meter) cannot be converted to Pound."));
        });
    }
}