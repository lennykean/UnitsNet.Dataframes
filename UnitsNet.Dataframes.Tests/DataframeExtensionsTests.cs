using System;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests;

#pragma warning disable NUnit1001
[TestFixture]
public class DataframeExtensionsTests
{
    [Test]
    public void GetQuantityTest()
    {
        var box = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4
        };
 
        var width = box.GetQuantity<Box, Length>("Width");
        var height = box.GetQuantity<Box, Length>(b => b.Height);
        var depth = box.GetQuantity("Depth");
        var weight = box.GetQuantity(b => b.Weight);
        var volume = box.GetQuantity(b => b.Volume);

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
            Assert.That(volume, Has
                .Property(nameof(IQuantity.Value)).EqualTo(6).And
                .Property(nameof(IQuantity.Unit)).EqualTo(VolumeUnit.CubicMeter));
        });
    }

    [TestCase(
        1, 1, LengthUnit.Meter,
        2, 2, LengthUnit.Meter,
        3, 3, LengthUnit.Meter,
        4, 4, MassUnit.Kilogram,
        6, VolumeUnit.CubicMeter,
        null, null,
        TestName = "{m} (no unit conversions)")]
    [TestCase(
        1, 10, LengthUnit.Decimeter,
        2, 200, LengthUnit.Centimeter,
        3, 3000, LengthUnit.Millimeter,
        4, 4000, MassUnit.Gram,
        6000, VolumeUnit.CubicDecimeter,
        null, null,
        TestName = "{m} (unit conversions)")]
    [TestCase(
        1, 10, AngleUnit.Degree,
        2, 200, LengthUnit.Centimeter,
        3, 3000, LengthUnit.Millimeter,
        4, 4000, MassUnit.Gram,
        6000, VolumeUnit.CubicDecimeter,
        typeof(InvalidOperationException), "Box.Width (Meter) cannot be converted to Degree.",
        TestName = "{m} (invalid unit conversion)")]
    [TestCase(
        1, 1, LengthUnit.Meter,
        2, 2, LengthUnit.Meter,
        3, 3, LengthUnit.Meter,
        1, 2.2, MassUnit.Pound,
        6, VolumeUnit.CubicMeter,
        typeof(InvalidOperationException), "Box.Weight (Kilogram) cannot be converted to Pound.",
        TestName = "{m} (disallowed unit conversion)")]
    public void ConvertQuantityTest(
        double width, double widthConverted, Enum widthUnit,
        double height, double heightConverted, Enum heightUnit,
        double depth, double depthConverted, Enum depthUnit,
        double weight, double weightConverted, Enum weightUnit,
        double volumeConverted, Enum volumeUnit,
        Type? expectedException, string? expectedExceptionMessage)
    {

        Assert.Multiple(() =>
        {
            Exception? actualException = null;
            try
            {
                var box = new Box
                {
                    Width = width,
                    Height = height,
                    Depth = depth,
                    Weight = weight
                };
                var widthQuantity = box.ConvertQuantity<Box, Length>("Width", to: widthUnit);
                var heightQuantity = box.ConvertQuantity<Box, Length>(b => b.Height, to: heightUnit);
                var depthQuantity = box.ConvertQuantity("Depth", to: depthUnit);
                var volumeQuantity = box.ConvertQuantity(b => b.Volume, to: volumeUnit);
                var weightQuantity = box.ConvertQuantity(b => b.Weight, to: weightUnit);

                Assert.That(widthQuantity, Has
                    .Property(nameof(IQuantity.Value)).EqualTo(widthConverted).Within(0.001).And
                    .Property(nameof(IQuantity.Unit)).EqualTo(widthUnit));
                Assert.That(heightQuantity, Has
                    .Property(nameof(IQuantity.Value)).EqualTo(heightConverted).Within(0.001).And
                    .Property(nameof(IQuantity.Unit)).EqualTo(heightUnit));
                Assert.That(depthQuantity, Has
                    .Property(nameof(IQuantity.Value)).EqualTo(depthConverted).Within(0.001).And
                    .Property(nameof(IQuantity.Unit)).EqualTo(depthUnit));
                Assert.That(volumeQuantity, Has
                    .Property(nameof(IQuantity.Value)).EqualTo(volumeConverted).Within(0.001).And
                    .Property(nameof(IQuantity.Unit)).EqualTo(volumeUnit));
                Assert.That(weightQuantity, Has
                    .Property(nameof(IQuantity.Value)).EqualTo(weightConverted).Within(0.001).And
                    .Property(nameof(IQuantity.Unit)).EqualTo(weightUnit));
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            finally
            {
                if (expectedException is not null)
                    Assert.That(actualException, Is.TypeOf(expectedException));
                if (expectedExceptionMessage is not null)
                    Assert.That(actualException?.Message, Is.EqualTo(expectedExceptionMessage));
            }
        });
    }
}
#pragma warning restore NUnit1001