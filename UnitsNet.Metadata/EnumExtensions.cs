﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using UnitsNet.Metadata.Reflection;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata;

public static class EnumExtensions
{
    public static bool TryGetUnitInfo(this Enum unit, Type? quantityType, [NotNullWhen(true)] out UnitInfo? unitInfo)
    {
        // Check cache
        if (EphemeralValueCache<Enum, UnitInfo>.GlobalInstance.TryGet(unit, out unitInfo))
            return true;

        // Check for a built-in unit type
        unitInfo = (
            from q in Quantity.Infos
            from u in q.UnitInfos
            where u.Value.Equals(unit)
            select u).SingleOrDefault();
        if (unitInfo is not null)
        {
            EphemeralValueCache<Enum, UnitInfo>.GlobalInstance.AddOrUpdate(unit, unitInfo);
            return true;
        }

        // Check if quantityType can be used to get a matching quantityInfo and unitInfo.
        if (unit.TryGetQuantityInfo(quantityType, out var quantityInfo))
            unitInfo = quantityInfo?.UnitInfos.SingleOrDefault(i => i.Value.Equals(unit));

        if (unitInfo is not null)
        {
            EphemeralValueCache<Enum, UnitInfo>.GlobalInstance.AddOrUpdate(unit, unitInfo);
            return true;
        }

        return false;
    }

    public static bool TryGetQuantityInfo(this Enum unit, Type? quantityType, [NotNullWhen(true)] out QuantityInfo? quantityInfo)
    {
        // Check cache
        if (EphemeralValueCache<Enum, QuantityInfo>.GlobalInstance.TryGet(unit, out quantityInfo))
            return true;

        // Check for a built-in quantity type
        quantityInfo = (
            from q in Quantity.Infos
            where q.UnitInfos.Any(u => u.Value.Equals(unit))
            select q).SingleOrDefault();
        if (quantityInfo is not null)
        {
            EphemeralValueCache<Enum, QuantityInfo>.GlobalInstance.AddOrUpdate(unit, quantityInfo);
            return true;
        }

        // Check for a static QuantityInfo property on quantityType and try to invoke the getter
        if (quantityType is not null && quantityType.TryGetStaticQuantityInfo(out quantityInfo) && quantityInfo!.UnitType == unit.GetType())
        {
            EphemeralValueCache<Enum, QuantityInfo>.GlobalInstance.AddOrUpdate(unit, quantityInfo);
            return true;
        }

        // Check for a default public constructor, try to construct an instance of quantityType, then use the QuantityInfo instance property
        if (quantityType is not null && quantityType.TryCreateQuantityInstance(out var instance) && instance.QuantityInfo.UnitType == unit.GetType())
        {
            quantityInfo = instance.QuantityInfo;
            EphemeralValueCache<Enum, QuantityInfo>.GlobalInstance.AddOrUpdate(unit, instance.QuantityInfo);
            return true;
        }

        return false;
    }
}