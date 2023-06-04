using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet.Dataframes.Utils;

namespace UnitsNet.Dataframes.Reflection;

internal static class ReflectionExtensionsz
{
    private static readonly Lazy<Type[]> LazyQuantityValueCompatibleTypes = new(() => (
        from m in typeof(QuantityValue).GetMethods(BindingFlags.Public | BindingFlags.Static)
        where m.Name == "op_Implicit"
        select m.GetParameters().First().ParameterType).ToArray());

    private static readonly Lazy<ConcurrentDictionary<Type, ConstructorInfo>> LazyQuantityConstructorTable = new(() => new());

    public static IEnumerable<PropertyInfo> GetProperties(this Type type, bool inherit = true)
    {
        foreach (var property in type.GetProperties())
        {
            yield return property;
        }

        if (!inherit)
            yield break;

        foreach (var super in type.GetInterfaces())
        {
            foreach (var property in super.GetProperties())
            {
                yield return property;
            }
        }
    }

    public static PropertyInfo ExtractProperty<TDataframe, TPropertyValue>(this Expression<Func<TDataframe, TPropertyValue>> propertySelectorExpression)
    {
        var expression = propertySelectorExpression.Body;

        // Unwrap any casts in the expression tree
        while (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
            expression = unaryExpression.Operand;

        // Ensure the expression is a property accessor and get the PropertyInfo
        if (expression is not MemberExpression memberExpression || memberExpression.Member is not PropertyInfo property || property.GetGetMethod()?.IsPublic != true)
            throw new InvalidOperationException($"{{{propertySelectorExpression}}} is not a valid property accessor.");

        return property;
    }

    public static bool TryCreateQuantityInstance(this Type type, [NotNullWhen(true)] out IQuantity? instance)
    {
        var ctor = type.GetConstructor(Type.EmptyTypes);
        if (ctor is null || !typeof(IQuantity).IsAssignableFrom(type))
        {
            instance = default;
            return false;
        }
        instance = (IQuantity)ctor.Invoke(null);
        return true;
    }

    private static QuantityMetadata GetQuantityMetadata(this PropertyInfo property, IMetadataProvider<QuantityMetadata>? metadataProvider = null)
    {
        if (metadataProvider?.TryGetMetadata(property, out var providerMetadata) is true && providerMetadata.Unit is not null)
            return providerMetadata;
        else if (DataFrameMetadata.TryGetMetadata(property, out var dataframeMetadata) && dataframeMetadata.Unit is not null)
            return dataframeMetadata;
        else
            throw new InvalidOperationException($"Unit metadata does not exist for {property.DeclaringType.Name}.{property.Name}.");
    }

    public static bool TryGetStaticQuantityInfo(this Type type, [NotNullWhen(true)] out QuantityInfo? value)
    {
        var staticProperty = type.GetProperties(BindingFlags.Public | BindingFlags.Static).SingleOrDefault(p => typeof(QuantityInfo).IsAssignableFrom(p.PropertyType));
        var staticGetter = staticProperty?.GetGetMethod();
        if (staticGetter is null)
        {
            value = default;
            return false;
        }
        value = (QuantityInfo)staticGetter.Invoke(null, null);
        return true;
    }

    public static IQuantity GetQuantityFromProperty(this object dataframe, PropertyInfo property)
    {
        var value = dataframe.GetQuantityValueFromProperty(property);
        var quantityMetadata = property.GetQuantityMetadata(dataframe as IMetadataProvider<QuantityMetadata>);
        var unitMetadata = quantityMetadata.Unit!;
        var quantityTypeMetadata = unitMetadata.QuantityType;

        return value.AsQuantity(unitMetadata.UnitInfo.Value, quantityTypeMetadata.QuantityInfo.ValueType);
    }

    public static double GetQuantityValueFromProperty(this object dataframe, PropertyInfo property)
    {
        // Get property getter from cache, or get and add to cache
        var getter = EphemeralValueCache<PropertyInfo, MethodInfo>.Instance.GetOrAdd(property, p =>
        {
            var getter = p.GetGetMethod() ?? throw new InvalidOperationException($"{p.DeclaringType}.{p.Name} does not have a public getter.");
            if (!LazyQuantityValueCompatibleTypes.Value.Contains(getter.ReturnType))
                throw new InvalidOperationException($"{p.DeclaringType}.{p.Name} type of {getter.ReturnType} is not compatible with {typeof(QuantityValue)}.");

            return getter;
        });
        return getter is not null
            ? Convert.ToDouble(getter.Invoke(dataframe, new object[] { }))
            : default;
    }

    public static IQuantity AsQuantity(this double value, Enum unit, Type quantityType)
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
        var quantityCtor = LazyQuantityConstructorTable.Value.GetOrAdd(quantityType, t =>
        {
            var ctor = (
                from c in t.GetConstructors()
                let parameters = c.GetParameters()
                where parameters.Count() == 2
                where
                    parameters.Last().ParameterType == typeof(QuantityValue)
                    || LazyQuantityValueCompatibleTypes.Value.Contains(parameters.First().ParameterType)
                where parameters.Last().ParameterType == quantityInfo!.UnitType
                select c).SingleOrDefault();

            return ctor is null
                ? throw new InvalidOperationException($"Unable to create quantity. No constructor found compatible with {t.Name}({typeof(QuantityValue).Name}, {quantityInfo!.UnitType.Name})")
                : ctor;
        })!;
        return (IQuantity)quantityCtor.Invoke(new object[] { Convert.ChangeType(value, quantityCtor.GetParameters().First().ParameterType), unit });
    }

    public static (UnitMetadata fromMetadata, UnitMetadata toMetadata) GetConversionMetadata(this PropertyInfo property, Enum to, IMetadataProvider<QuantityMetadata>? metadataProvider = null)
    {
        var quantityMetadata = property.GetQuantityMetadata(metadataProvider);
        var conversionMetadata = quantityMetadata.Conversions.FirstOrDefault(c => c.UnitInfo.Value.Equals(to));
        if (conversionMetadata is null)
            throw new InvalidOperationException($"Conversion to {conversionMetadata} is not allowed on {property.DeclaringType.Name}.{property.Name}.");

        var toMetadata = UnitMetadata.FromUnitInfo(conversionMetadata.UnitInfo, conversionMetadata.QuantityType.QuantityInfo);

        return (quantityMetadata.Unit!, toMetadata);
    }

    public static ReadOnlyDictionary<PropertyInfo, PropertyInfo> GetBidirectionalInterfacePropertyMap(this Type concreteType)
    {
        return new(EphemeralValueCache<Type, Dictionary<PropertyInfo, PropertyInfo>>.Instance.GetOrAdd(concreteType, t =>
        {
            return new(GetInterfacePropertyMapKeyValues(concreteType));
        }));
    }

    private static IEnumerable<KeyValuePair<PropertyInfo, PropertyInfo>> GetInterfacePropertyMapKeyValues(Type concreteType)
    {
        foreach (var interfaceType in concreteType.GetInterfaces())
        {
            var interfaceMap = concreteType.GetInterfaceMap(interfaceType);
            var allInterfaceProperties = interfaceType.GetProperties((BindingFlags)(-1));
            var allConcreteProperties = concreteType.GetProperties((BindingFlags)(-1));

            for (var i = 0; i < interfaceMap.InterfaceMethods.Length; i++)
            {
                var interfaceProperty = allInterfaceProperties.SingleOrDefault(p => p.GetMethod == interfaceMap.InterfaceMethods[i]);
                var concreteProperty = allConcreteProperties.SingleOrDefault(p => p.GetMethod == interfaceMap.TargetMethods[i]);

                if (interfaceProperty is null || concreteProperty is null)
                    continue;

                yield return new(interfaceProperty, concreteProperty);
                yield return new(concreteProperty, interfaceProperty);
            }
        }
    }
}