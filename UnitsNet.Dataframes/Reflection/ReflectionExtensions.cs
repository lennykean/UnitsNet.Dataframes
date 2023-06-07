using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Utils;

namespace UnitsNet.Dataframes.Reflection;

internal static class ReflectionExtensions
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

    private static TMetadata GetQuantityMetadata<TDataframe, TMetadataAttribute, TMetadata>(
        this PropertyInfo property,
        IDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata> metadataProvider,
        CultureInfo? culture = null)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
    {
        if (metadataProvider.TryGetMetadata(property, out var metadata, culture) is not true || metadata.Unit is null)
            throw new InvalidOperationException($"Unit metadata does not exist for {property.DeclaringType.Name}.{property.Name}.");

        metadata.Validate();

        return metadata;
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

    public static IQuantity GetQuantityFromProperty<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, PropertyInfo property, CultureInfo? culture = null)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
    {
        var metadataProvider = dataframe as IDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata>
            ?? DefaultDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata>.Instance;

        var value = dataframe.GetQuantityValueFromProperty(property);
        var quantityMetadata = property.GetQuantityMetadata(metadataProvider, culture);
        var unitMetadata = quantityMetadata.Unit!;
        var quantityTypeMetadata = unitMetadata.QuantityType;

        return value.AsQuantity(unitMetadata.UnitInfo.Value, quantityTypeMetadata.QuantityInfo.ValueType);
    }

    public static double GetQuantityValueFromProperty<TDataframe>(this TDataframe dataframe, PropertyInfo property)
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

    public static (UnitMetadata fromMetadata, UnitMetadata toMetadata) GetConversionMetadatas<TDataframe, TMetadataAttribute, TMetadata>(
        this PropertyInfo property, 
        Enum to,
        IDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata>? metadataProvider = null,
        CultureInfo? culture = null)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
    {
        metadataProvider ??= DefaultDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata>.Instance;

        var metadata = property.GetQuantityMetadata(metadataProvider, culture);
        var conversionMetadata = metadata.Conversions.FirstOrDefault(c => c.UnitInfo.Value.Equals(to))
            ?? throw new InvalidOperationException($"{property.DeclaringType.Name}.{property.Name} ({metadata.Unit!.UnitInfo.Value}) cannot be converted to {to}.");
        var toMetadata = UnitMetadata.FromUnitInfo(conversionMetadata.UnitInfo, conversionMetadata.QuantityType.QuantityInfo, culture);

        return (metadata.Unit!, toMetadata);
    }

    public static bool TryGetMappedProperty(this PropertyInfo property, Type type, [NotNullWhen(true)] out PropertyInfo? otherProperty)
    {
        otherProperty = property;
        if (property.DeclaringType == type)
            return true;

        ReadOnlyDictionary<PropertyInfo, PropertyInfo>? interfacePropertyMap;

        if (!type.IsInterface && property.DeclaringType.IsInterface)
            interfacePropertyMap = type.GetInterfacePropertyMap();
        else if (!property.DeclaringType.IsInterface && type.IsInterface)
            interfacePropertyMap = property.DeclaringType.GetInterfacePropertyMap();
        else
            return false;

        return interfacePropertyMap.TryGetValue(property, out otherProperty);
    }

    private static ReadOnlyDictionary<PropertyInfo, PropertyInfo> GetInterfacePropertyMap(this Type concreteType)
    {
        return EphemeralValueCache<Type, ReadOnlyDictionary<PropertyInfo, PropertyInfo>>.Instance.GetOrAdd(concreteType, t =>
        {
            return new(BuildInterfacePropertyMap(concreteType).ToDictionary(t => t.Item2, t => t.Item2));
        });
    }

    private static IEnumerable<(PropertyInfo, PropertyInfo)> BuildInterfacePropertyMap(Type type)
    {
        var properties = type.GetProperties((BindingFlags)(-1));

        foreach (var interfaceType in type.GetInterfaces())
        {
            var interfaceMap = type.GetInterfaceMap(interfaceType);
            var interfaceProperties = interfaceType.GetProperties((BindingFlags)(-1));

            for (var i = 0; i < interfaceMap.InterfaceMethods.Length; i++)
            {
                var interfaceProperty = interfaceProperties.SingleOrDefault(p => p.GetMethod == interfaceMap.InterfaceMethods[i]);
                var concreteProperty = properties.SingleOrDefault(p => p.GetMethod == interfaceMap.TargetMethods[i]);

                if (interfaceProperty is null || concreteProperty is null)
                    continue;

                yield return new(interfaceProperty, concreteProperty);
                yield return new(concreteProperty, interfaceProperty);
            }
        }
    }
}