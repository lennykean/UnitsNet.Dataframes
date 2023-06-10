﻿using System;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Dataframes.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.DataframeExtensions;

[TestFixture]
public class ConvertQuantity
{
    public record ConvertQuantityTestData(double Value, double Converted, Enum Unit);

    public readonly static TestCaseData[] ConvertQuantityTestCases = new[]
    {
        new TestCaseData(
            new ConvertQuantityTestData(1, 10, LengthUnit.Decimeter),
            new ConvertQuantityTestData(2, 200, LengthUnit.Centimeter),
            new ConvertQuantityTestData(3, 3000, LengthUnit.Millimeter),
            new ConvertQuantityTestData(4, 4000, MassUnit.Gram),
            new ConvertQuantityTestData(0, 6000, VolumeUnit.CubicDecimeter)).SetName("{c} (to same unit system)"),
        new TestCaseData(
            new ConvertQuantityTestData(1, 39.37, LengthUnit.Inch),
            new ConvertQuantityTestData(2, 6.561, LengthUnit.Foot),
            new ConvertQuantityTestData(3, 3.28, LengthUnit.Yard),
            new ConvertQuantityTestData(4, 8.818, MassUnit.Pound),
            new ConvertQuantityTestData(0, 366143.894, VolumeUnit.CubicInch)).SetName("{c} (to different unit system)"),
        new TestCaseData(
            new ConvertQuantityTestData(1, 1, LengthUnit.Meter),
            new ConvertQuantityTestData(2, 2, LengthUnit.Meter),
            new ConvertQuantityTestData(3, 3, LengthUnit.Meter),
            new ConvertQuantityTestData(4, 4, MassUnit.Kilogram),
            new ConvertQuantityTestData(0, 6, VolumeUnit.CubicMeter)).SetName("{c} (to same unit)"),
    };

    [TestCaseSource(nameof(ConvertQuantityTestCases))]
    public void ToUnitTest(
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

    [TestCase(TestName = "{c} (to invalid unit)")]
    public void ToInvalidUnitTest()
    {
        var box = new Box
        {
            Width = 1,
            Height = 2
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => box.ConvertQuantity(b => b.Width, to: AngleUnit.Degree), Throws.InvalidOperationException.With.Message.Match("(.*) cannot be converted to (.*)"));
            Assert.That(() => box.ConvertQuantity("Height", to: SpeedUnit.Mach), Throws.InvalidOperationException.With.Message.Match("(.*) cannot be converted to (.*)"));
        });
    }

    [TestCase(TestName = "{c} (to disallowed unit)")]
    public void ToDisallowedUnitTest()
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
            Assert.That(() => box.ConvertQuantity(b => b.Volume, to: VolumeUnit.CubicFoot), Throws.InvalidOperationException.With.Message.Match("(.*) cannot be converted to (.*)"));
            Assert.That(() => box.ConvertQuantity("Height", to: MassUnit.Pound), Throws.InvalidOperationException.With.Message.Match("(.*) cannot be converted to (.*)"));
        });
    }

    [TestCase(TestName = "{c} (with invalid quantity)")]
    public void WithInvalidQuantityTest()
    {
        var blob = new Blob
        {
            Data = "1"
        };

        Assert.That(() => blob.ConvertQuantity("Data", to: InformationUnit.Gibibit), Throws.InvalidOperationException.With.Message.Match("Type of (.*) \\((.*)\\) is not a valid quantity type"));
    }

    [TestCase(TestName = "{c} (with missing property)")]
    public void WithMissingPropertyTest()
    {
        var blob = new Blob();

        Assert.That(() => blob.ConvertQuantity("FakeProperty", to: InformationUnit.Gibibit), Throws.InvalidOperationException.With.Message.Match("(.*) is not a property of (.*)"));
    }

    [TestCase(TestName = "{c} (with invalid attribute)")]
    public void WithInvalidAttributeTest()
    {
        var garbage = new Garbage();

        Assert.That(() => garbage.ConvertQuantity(r => r.Odor, to: PowerUnit.MechanicalHorsepower), Throws.ArgumentException.With.Message.EqualTo("Unit must be an enum value"));
    }

    [TestCase(TestName = "{c} (with custom unit)")]
    public void WithCustomUnitTest()
    {
        var employee = new Employee
        {
            Name = "Cubert",
            Coolness = 40
        };

        Assert.Multiple(() =>
        {
            Assert.That(employee.ConvertQuantity(e => e.Coolness, to: CoolnessUnit.Fonzie), Has
                .Property(nameof(IQuantity.Value)).EqualTo(40_000_000).And
                .Property(nameof(IQuantity.Unit)).EqualTo(CoolnessUnit.Fonzie));
            Assert.That(employee.ConvertQuantity("Coolness", to: CoolnessUnit.Fonzie), Has
                .Property(nameof(IQuantity.Value)).EqualTo(40_000_000).And
                .Property(nameof(IQuantity.Unit)).EqualTo(CoolnessUnit.Fonzie));
        });
    }

    [TestCase(TestName = "{c} (with invalid custom unit)")]
    public void WithInvalidCustomUnitTest()
    {
        var rubbish = new Rubbish
        {
            Coolness = 40
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => rubbish.ConvertQuantity(r => r.Coolness, to: CoolnessUnit.Fonzie), Throws.ArgumentException.With.Message.Match("(.*) is not a known unit value"));
            Assert.That(() => rubbish.ConvertQuantity("Coolness", to: CoolnessUnit.Fonzie), Throws.ArgumentException.With.Message.Match("(.*) is not a known unit value"));
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

        Assert.Multiple(() =>
        {
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

        Assert.Multiple(() =>
        {
            Assert.That(dynoDataframe.ConvertQuantity<DynoDataframe, DynoMeasurementAttribute, DynoMetadata>(d => d.Horsepower, to: PowerUnit.Kilowatt), Has
                .Property(nameof(IQuantity.Value)).EqualTo(223.7).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(PowerUnit.Kilowatt));
            Assert.That(dynoDataframe.ConvertQuantity<DynoDataframe, DynoMeasurementAttribute, DynoMetadata>("Torque", to: TorqueUnit.NewtonMeter), Has
                .Property(nameof(IQuantity.Value)).EqualTo(271.16).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(TorqueUnit.NewtonMeter));
        });
    }
}