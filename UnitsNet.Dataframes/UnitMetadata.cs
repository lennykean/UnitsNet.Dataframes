using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Humanizer;

using UnitsNet.Dataframes.Reflection;
using UnitsNet.Dataframes.Utils;

namespace UnitsNet.Dataframes;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class UnitMetadataBase
{
    private protected UnitMetadataBase(UnitInfo unitInfo, string name, int value, string displayName, string abbriviation)
    {
        UnitInfo = unitInfo;
        Name = name;
        Value = value;
        DisplayName = displayName;
        Abbriviation = abbriviation;
    }

    [JsonIgnore, IgnoreDataMember]
    public UnitInfo UnitInfo { get; }

    public string Name { get; }
    public int Value { get; }
    public string DisplayName { get; }
    public string Abbriviation { get; }

    private protected static string GetDisplayName(UnitInfo unitInfo)
    {
        return unitInfo.PluralName.Humanize(LetterCasing.LowerCase);
    }

    private protected static string GetAbbriviation(UnitInfo unitInfo, QuantityInfo quantityInfo, CultureInfo? culture)
    {
        return UnitAbbreviationsCache.Default.GetUnitAbbreviations(quantityInfo.UnitType, Convert.ToInt32(unitInfo.Value), culture).FirstOrDefault() ?? "";
    }
}

public sealed class UnitMetadataBasic : UnitMetadataBase
{
    public UnitMetadataBasic(UnitInfo unitInfo, string name, int value, string displayName, string abbriviation, QuantityTypeMetadataBasic quantityType)
        : base(unitInfo, name, value, displayName, abbriviation)
    {
        QuantityType = quantityType;
    }

    public QuantityTypeMetadataBasic QuantityType { get; }

    public static UnitMetadataBasic? FromUnitInfo(UnitInfo unitInfo, QuantityInfo quantityInfo, CultureInfo? culture = null)
    {
        return EphemeralValueCache<Enum, UnitMetadataBasic?>.Instance.GetOrAdd(unitInfo.Value, _ =>
        {
            return QuantityTypeMetadataBasic.FromQuantityInfo(quantityInfo) switch
            {
                var quantityType when quantityType is not null
                    => new(unitInfo, unitInfo.Name, Convert.ToInt32(unitInfo.Value), GetDisplayName(unitInfo), GetAbbriviation(unitInfo, quantityInfo, culture), quantityType),
                _
                    => null
            };
        });
    }
}

public sealed class UnitMetadata : UnitMetadataBase
{
    public UnitMetadata(UnitInfo unitInfo, string name, int value, string displayName, string abbriviation, QuantityTypeMetadata quantityType)
        : base(unitInfo, name, value, displayName, abbriviation)
    {
        QuantityType = quantityType;
    }

    public QuantityTypeMetadata QuantityType { get; }

    public bool TryConvertQuantity(double value, UnitMetadata to, out IQuantity convertedQuantity)
    {
        convertedQuantity = value.AsQuantity(UnitInfo.Value, QuantityType.QuantityInfo.ValueType);
        if (UnitInfo.Value == to.UnitInfo.Value)
            return true;

        var conversionFunctions = EphemeralKeyCache<UnitMetadata, ConcurrentDictionary<UnitMetadata, ConversionFunction>>.Instance.GetOrAdd(this, _ => new());
        if (!conversionFunctions.TryGetValue(to, out var conversionFunction))
        {
            if (!UnitConverter.Default.TryGetConversionFunction(
                    QuantityType.QuantityInfo.ValueType, UnitInfo.Value,
                    to.QuantityType.QuantityInfo.ValueType,
                    to.UnitInfo.Value,
                    out conversionFunction))
            {
                var baseUnit = QuantityType.BaseUnit;
                if (baseUnit is null || baseUnit != to.QuantityType.BaseUnit)
                    return false;

                if (!UnitConverter.Default.TryGetConversionFunction(
                        QuantityType.QuantityInfo.ValueType,
                        UnitInfo.Value,
                        baseUnit.QuantityType.QuantityInfo.ValueType,
                        baseUnit.UnitInfo.Value,
                        out var baseUnitConversionFunction))
                {
                    return false;
                }
                if (!UnitConverter.Default.TryGetConversionFunction(
                        baseUnit.QuantityType.QuantityInfo.ValueType,
                        baseUnit.UnitInfo.Value,
                        to.QuantityType.QuantityInfo.ValueType,
                        to.UnitInfo.Value,
                        out var finalUnitConversionFunction))
                {
                    return false;
                }
                conversionFunction = quantity => finalUnitConversionFunction(baseUnitConversionFunction(quantity));
            }
            conversionFunctions.AddOrUpdate(to, conversionFunction, (_, _) => conversionFunction);
        }
        convertedQuantity = conversionFunction.Invoke(convertedQuantity);
        return true;
    }

    public static UnitMetadata FromUnitInfo(UnitInfo unitInfo, QuantityInfo quantityInfo, CultureInfo? culture = null)
    {
        return EphemeralValueCache<Enum, UnitMetadata>.Instance.GetOrAdd(unitInfo.Value, _ =>
        {
            var quantityType = QuantityTypeMetadata.FromQuantityInfo(quantityInfo, culture);
            return new(unitInfo, unitInfo.Name, Convert.ToInt32(unitInfo.Value), GetDisplayName(unitInfo), GetAbbriviation(unitInfo, quantityInfo, culture), quantityType);
        });
    }
}