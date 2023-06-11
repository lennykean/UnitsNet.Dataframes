using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Humanizer;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class QuantityTypeMetadataBase
{
    private protected QuantityTypeMetadataBase(QuantityInfo quantityInfo, string name, string displayName)
    {
        QuantityInfo = quantityInfo;
        Name = name;
        DisplayName = displayName;
    }

    [JsonIgnore, IgnoreDataMember]
    public QuantityInfo QuantityInfo { get; }

    public string Name { get; }
    public string DisplayName { get; }

    private protected static string GetDisplayName(QuantityInfo quantityInfo)
    {
        return quantityInfo.Name.Humanize(LetterCasing.LowerCase);
    }
}

public sealed class QuantityTypeMetadataBasic : QuantityTypeMetadataBase
{
    public QuantityTypeMetadataBasic(QuantityInfo quantityInfo, string name, string displayName) : base(quantityInfo, name, displayName)
    {
    }

    public static QuantityTypeMetadataBasic? FromQuantityInfo(QuantityInfo quantityInfo)
    {
        return EphemeralValueCache<Type, QuantityTypeMetadataBasic>.Instance.GetOrAdd(quantityInfo.ValueType, _ =>
        {
            var name = quantityInfo.Name;
            var displayName = GetDisplayName(quantityInfo);

            return new(quantityInfo, name, displayName);
        });
    }
}

public sealed class QuantityTypeMetadata : QuantityTypeMetadataBase
{
    public QuantityTypeMetadata(QuantityInfo quantityInfo, string name, string displayName, UnitMetadataBasic? baseUnit) : base(quantityInfo, name, displayName)
    {
        BaseUnit = baseUnit;
    }

    public UnitMetadataBasic? BaseUnit { get; }

    public static QuantityTypeMetadata FromQuantityInfo(QuantityInfo quantityInfo, CultureInfo? culture = null)
    {
        return EphemeralValueCache<Type, QuantityTypeMetadata>.Instance.GetOrAdd(quantityInfo.ValueType, _ =>
        {
            var name = quantityInfo.Name;
            var displayName = GetDisplayName(quantityInfo);
            var baseUnit = UnitMetadataBasic.FromUnitInfo(quantityInfo.BaseUnitInfo, quantityInfo, culture);

            return new(quantityInfo, name, displayName, baseUnit);
        });
    }
}