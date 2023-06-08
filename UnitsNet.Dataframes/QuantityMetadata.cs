using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes;

public class QuantityMetadata : DataframeMetadata<QuantityAttribute, QuantityMetadata>.IDataframeMetadata
{
    private static readonly HashSet<Type> NumericTypes = new()
    {
        typeof(byte),
        typeof(sbyte),
        typeof(ushort),
        typeof(uint),
        typeof(ulong),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(decimal),
        typeof(double),
        typeof(float)
    };

    public QuantityMetadata(PropertyInfo property, UnitMetadata? unit, IList<UnitMetadataBasic> conversions)
    {
        Property = property;
        Unit = unit;
        Conversions = new(conversions);
    }

    [JsonIgnore, IgnoreDataMember]
    public PropertyInfo Property { get; }
    public string FieldName => Property.Name;
    public UnitMetadata? Unit { get; }
    public ReadOnlyCollection<UnitMetadataBasic> Conversions { get; }

    internal protected virtual void Validate()
    {
        if (!NumericTypes.Contains(Property.PropertyType))
            throw new InvalidOperationException($"Type of {Property.DeclaringType.Name}.{Property.Name} ({Property.PropertyType}) is not a valid quantity type");
    }

    QuantityMetadata DataframeMetadata<QuantityAttribute, QuantityMetadata>.IDataframeMetadata.Clone(
        PropertyInfo? overrideProperty,
        IEnumerable<UnitMetadataBasic>? overrideConversions,
        UnitMetadata? overrideUnit,
        CultureInfo? overrideCulture)
    {
        return new QuantityMetadata(overrideProperty ?? Property, overrideUnit ?? Unit, (overrideConversions ?? Conversions).ToList());
    }

    void DataframeMetadata<QuantityAttribute, QuantityMetadata>.IDataframeMetadata.Validate()
    {
        Validate();
    }
}