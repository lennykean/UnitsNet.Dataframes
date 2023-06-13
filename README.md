# UnitsNet.Metadata Documentation

UnitsNet.Metadata is an extension of [UnitsNet](https://github.com/angularsen/UnitsNet) that is designed to streamline the handling of units and quantities in applications. It allows for the provision of metadata through annotations, fluent syntax, and dynamic proxies. This enables developers to manage units in a declarative manner, simplifying interactions with quantities by seamlessly performing conversions and retrieving quantities.

## Features

- 🏷️ **Annotations**: Decorate class properties with units and allowable conversions using attributes.
- 🔗 **Fluent Syntax**: Configure metadata for properties and enable conversions using a fluent interface.
- 🔄 **Conversion Methods**: Easily convert between units using extension methods.
- 🛡️ **Dynamic Proxies**: Create proxies of your objects which can automatically handle unit conversions.
- 📄 **Metadata Objects**: Generate metadata based on annotations or fluent syntax, which includes unit information, allowed conversions, and quantity types.
- ⚙️ **Custom Units and Attributes**: Full support for custom units, and extended data annotations.

## Getting Started

### Installation

Install UnitsNet.Metadata through NuGet:

```sh
dotnet add package UnitsNet.Metadata
```

### Annotations

Decorate your class properties with the unit types using annotations.

```csharp
using UnitsNet.Metadata.Annotations;
using UnitsNet.Units;

public class Box
{
    [Quantity(LengthUnit.Meter)]
    public double Width { get; set; }

    [Quantity(LengthUnit.Meter)]
    public double Height { get; set; }

    [Quantity(MassUnit.Kilogram)]
    [AllowUnitConversion(MassUnit.Gram)]
    public double Weight { get; set; }
}
```

#### Allowed Conversions Attribute

By default, all units of the same quantity type are considered as "allowed conversions". However, you can use the `AllowedConversions` attribute to specify a subset of units to be used as allowed conversions for a particular property.

## Retrieving Quantities

The `GetQuantity` method retrieves quantities as `IQuantity` objects. This can be helpful for obtaining the properties of an object in their original unit of measurement.

```csharp
using UnitsNet.Metadata;

var obj = new Box { Width = 2.5, Height = 3.5, Weight = 4.5 };

IQuantity width = obj.GetQuantity("Width");        // Retrieves 2.5 in meters
IQuantity height = obj.GetQuantity(b => b.Height); // Retrieves 3.5 in meters
IQuantity weight = obj.GetQuantity(b => b.Weight); // Retrieves 4.5 in kilograms
```

In this example, `GetQuantity` returns `IQuantity` objects representing the dimensions and weight of the box in the units they were initially set (meters for dimensions and kilograms for weight).

## Conversion Methods

UnitsNet.Metadata extension methods facilitate conversion between different units. These methods enable converting quantities associated with properties and also setting property values in different units, which are then automatically converted to the declared unit.

### Convert Between Units

Use the `ConvertQuantity` method to convert properties into different units.

```csharp
using UnitsNet.Metadata;
using UnitsNet.Units;

var obj = new Box { Width = 2.5, Height = 3.5, Weight = 4.5 };

double widthInCentimeters = obj.ConvertQuantity("Width", LengthUnit.Centimeter);        // 250
double heightInCentimeters = obj.ConvertQuantity(b => b.Height, LengthUnit.Centimeter); // 350
double weightInGrams = obj.ConvertQuantity(b => b.Weight, MassUnit.Gram);               // 4500
```

### Set Properties in Different Units

The `SetQuantity` method setting property values using different units. The values will be automatically converted to the declared unit of the property.

```csharp
using UnitsNet.Metadata;
using UnitsNet.Units;

var obj = new Box();

obj.SetQuantity("Width", Length.FromCentimeter(45));        // obj.Weight == 4.5
obj.SetQuantity(b => b.Height, Length.FromCentimeter(625)); // obj.Height == 6.25
obj.SetQuantity(b => b.Weight, Mass.FromGrams(7000));       // obj.Weight == 7
```

In the above example, Width and Height are set in centimeters and are automatically converted to meters, while Weight is set in grams and converted to kilograms.

## Fluent Syntax

For a more flexible way to configure metadata, UnitsNet.Metadata.Fluent is provided. To start using it, install the package through NuGet:

```sh
dotnet add package UnitsNet.Metadata.Fluent
```

The fluent interface allows configuring metadata for properties, specifying the unit they represent, and also defining what conversions are allowed.

Here’s an example that demonstrates how to use the fluent syntax to configure metadata:

```csharp
using UnitsNet.Metadata;
using UnitsNet.Metadata.Fluent;
using UnitsNet.Units;

var metadataProvider = MetadataBuilder
    .CreateFor<Box>()
    .With("Width", asUnit: LengthUnit.Meter)
        .WithAllowedConversion(to: LengthUnit.Millimeter)
        .WithAllowedConversion(to: LengthUnit.Centimeter)
    .With(b => b.Height, asUnit: LengthUnit.Meter)
        .WithAllowedConversion(to: LengthUnit.Millimeter)
        .WithAllowedConversion(to: LengthUnit.Centimeter)
    .Build(global: false);

var box = new Box();
var metadata = metadataProvider.GetObjectMetadata(box);
```

In this example, metadata for the properties `Width` and `Height` of the `Box` class is configured. The properties are annotated to indicate that they are measured in meters, and also specify that they can be converted to millimeters and centimeters.

The `Build` method has a parameter named global, which indicates whether the metadata should be applied globally (similar to the annotation syntax) or if it should be stored only in the returned `metadataProvider`. In this example, the metadata is local and only accessible through the `metadataProvider`. To apply the metadata globally, pass `true` for the global parameter.

## Dynamic Proxies

Dynamic proxies are objects that act as a wrapper around your original objects and can automatically handle unit conversions. This is particularly useful when you need to interact with objects in different units without modifying the original object's values. The `UnitsNet.Metadata.DynamicProxy` package enables the creation of these proxies.

To use dynamic proxies, first, you need to install the package through NuGet:

```sh
dotnet add package UnitsNet.Metadata.DynamicProxy
```

Here’s an example that demonstrates how to use dynamic proxies.

```csharp
using UnitsNet.Metadata.DynamicProxy;
using UnitsNet.Units;

var boxes = new[]
{
    new Box { Width = 1, Height = 2, Weight = 4 };
    new Box { Width = 2, Height = 4, Weight = 8 };
    new Box { Width = 4, Height = 8, Weight = 16 };
};

// Create dynamic proxies
var proxies = boxes.AsDynamicQuantities()
    .WithConversion(b => b.Width, LengthUnit.Centimeter)
    .WithConversion(b => b.Height, LengthUnit.Centimeter)
    .WithConversion(b => b.Weight, MassUnit.Gram)
    .Build();

// Select the first proxy object
var proxy = proxies.First();

// The properties of the proxy will be in the converted units
double width = proxy.Width;   // 100, as it is converted to centimeters
double height = proxy.Height; // 200, as it is converted to centimeters

// Set values in the proxy, and it will be back-converted
proxy.Width = 200;  // obj.Width will now be 2 meters
proxy.Height = 400; // obj.Height will now be 4 meters
```

In this example, we create a dynamic proxy for `Box`, and configure it so that `Width` and `Height` are automatically converted to centimeters, and `Weight` is converted to grams.

When accessing these properties through the proxy, the values are automatically converted to the units specified. When setting values through the proxy, they are back-converted to the original units of the underlying object.

This enables working different units transparently, without having to manually convert values back and forth.

## Metadata Objects

Once you have configured the metadata, you can retrieve a complete description using the 'GetObjectMetadata' method.

```csharp
using UnitsNet.Metadata;

var box = new Box();
var metadata = box.GetObjectMetadata();
```

This method call will return the following metadata:

```json
{
  "Width": {
    "FieldName": "Width",
    "Unit": {
      "QuantityType": {
        "BaseUnit": {
          "QuantityType": {
            "Name": "Length",
            "DisplayName": "length"
          },
          "Name": "Meter",
          "Value": 21,
          "DisplayName": "meters",
          "Abbreviation": "m"
        },
        "Name": "Length",
        "DisplayName": "length"
      },
      "Name": "Meter",
      "Value": 21,
      "DisplayName": "meters",
      "Abbreviation": "m"
    },
    "Conversions": [ ... /* all default length conversions */ ]
  },
  "Height": {
    "FieldName": "Height",
    ...
  },
  "Weight": {
    "FieldName": "Weight",
    ...
    "Conversions": [ // only the conversions explicitly defined
      {
        "QuantityType": {
          "Name": "Mass",
          "DisplayName": "mass"
        },
        "Name": "Kilogram",
        "Value": 8,
        "DisplayName": "kilograms",
        "Abbreviation": "kg"
      },
      {
        "QuantityType": {
          "Name": "Mass",
          "DisplayName": "mass"
        },
        "Name": "Gram",
        "Value": 6,
        "DisplayName": "grams",
        "Abbreviation": "g"
      }
    ]
  }
}
```

In the example above, each field of the `Box` class (like `Width`, `Height`, and `Weight`) is annotated with metadata that defines the `Unit` it is measured in, the `FieldName` (which is the name of the field itself), and the allowed `Conversions`. The `Conversions` field lists the units that the value of the field can be converted into. For example, for the `Weight` field, only conversions to "Kilogram" and "Gram" are explicitly defined, therefore only these conversions are listed.

License
UnitsNet.Metadata is released under the [MIT License](LICENSE).
