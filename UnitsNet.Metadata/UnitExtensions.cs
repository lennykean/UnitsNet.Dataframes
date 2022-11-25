using System;
using System.Linq;
using System.Reflection;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    public static class UnitExtensions
    {
        private static readonly Lazy<Type[]> LazyQuantityValueCompatibleTypes = new(() => (
            from m in typeof(QuantityValue).GetMethods(BindingFlags.Public | BindingFlags.Static)
            where m.Name == "op_Implicit"
            select m.GetParameters().First().ParameterType).ToArray());

        public static bool TryGetUnitInfo(this Enum unit, Type? quantityType, out UnitInfo? unitInfo)
        {
            // Check cache
            if (SimpleCache<Enum, UnitInfo>.Instance.TryGet(unit, out unitInfo))
            {
                return true;
            }

            // Check for a built-in unit type
            unitInfo = (
                from q in Quantity.Infos
                from u in q.UnitInfos
                where u.Value.Equals(unit)
                select u).SingleOrDefault();
            if (unitInfo is not null)
            {
                SimpleCache<Enum, UnitInfo>.Instance.TryAdd(unit, unitInfo);
                return true;
            }

            // Check if quantityType can be used to get a matching quantityInfo and unitInfo.
            if (unit.TryGetQuantityInfo(quantityType, out var quantityInfo))
                unitInfo = quantityInfo?.UnitInfos.SingleOrDefault(i => i.Value.Equals(unit));
            if (unitInfo is not null)
            {
                SimpleCache<Enum, UnitInfo>.Instance.TryAdd(unit, unitInfo);
                return true;
            }

            return false;
        }

        public static bool TryGetQuantityInfo(this Enum unit, Type? quantityType, out QuantityInfo? quantityInfo)
        {
            // Check cache
            if (SimpleCache<Enum, QuantityInfo>.Instance.TryGet(unit, out quantityInfo))
            {
                return true;
            }

            // Check for a built-in quantity type
            quantityInfo = (
                from q in Quantity.Infos
                where q.UnitInfos.Any(u => u.Value.Equals(unit))
                select q).SingleOrDefault();
            if (quantityInfo is not null)
            {
                SimpleCache<Enum, QuantityInfo>.Instance.TryAdd(unit, quantityInfo);
                return true;
            }

            // Check for a static QuantityInfo property on quantityType and try to invoke the getter
            var staticInfoProperty = quantityType?.GetProperties(BindingFlags.Public | BindingFlags.Static).SingleOrDefault(p => typeof(QuantityInfo).IsAssignableFrom(p.PropertyType));
            var staticInfoGetter = staticInfoProperty?.GetGetMethod();
            quantityInfo = staticInfoGetter?.Invoke(null, null) as QuantityInfo;
            if (quantityInfo?.UnitType == unit.GetType())
            {
                SimpleCache<Enum, QuantityInfo>.Instance.TryAdd(unit, quantityInfo);
                return true;
            }

            // Check for a default public constructor, try to construct an instance of quantityType, and use the QuantityInfo instance property
            var instance = quantityType?.GetConstructor(Type.EmptyTypes)?.Invoke(null) as IQuantity;
            quantityInfo = instance?.QuantityInfo;
            if (quantityInfo?.UnitType == unit.GetType())
            {
                SimpleCache<Enum, QuantityInfo>.Instance.TryAdd(unit, quantityInfo);
                return true;
            }

            return false;
        }

        internal static IQuantity CreateQuantity(this Enum unit, Type quantityType, double value)
        {
            // Get quantity metadata
            if (!unit.TryGetQuantityInfo(quantityType, out var quantityInfo))
                throw new ArgumentException($"{unit.GetType().Name} is not a known unit type.");
            if (!unit.TryGetUnitInfo(quantityType, out var unitInfo))
                throw new ArgumentException($"{unit.GetType().Name}.{unit} is not a known unit value.");

            // Try to create a quantity for a build-in unit type
            if (Quantity.TryFrom(value, unit, out var quantity))
                return quantity!;

            // Get quantity constructor for a custom unit type from cache, or get and add to cache
            var quantityCtor = SimpleCache<Type, (ConstructorInfo ctor, Type valueParamType)>.Instance.GetOrAdd(quantityType, t =>
            {
                var ctor = (
                    from c in t.GetConstructors()
                    let parameters = c.GetParameters()
                    where parameters.Count() == 2
                    where parameters.Last().ParameterType == typeof(QuantityValue) || LazyQuantityValueCompatibleTypes.Value.Contains(parameters.First().ParameterType)
                    where parameters.Last().ParameterType == quantityInfo!.UnitType
                    select c).SingleOrDefault();
                if (ctor is null)
                    throw new InvalidOperationException($"Unable to create quantity. No constructor found compatible with {t.Name}({typeof(QuantityValue).Name}, {quantityInfo!.UnitType.Name})");

                return (ctor, ctor.GetParameters().First().ParameterType);
            });            
            return (IQuantity)quantityCtor.ctor.Invoke(new object[] { Convert.ChangeType(value, quantityCtor.valueParamType), unit });
        }
    }
}