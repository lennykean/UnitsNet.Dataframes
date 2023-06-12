﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata.Reflection;

internal static class ReflectionExtensions
{
    private static readonly Lazy<Type[]> LazyQuantityValueCompatibleTypes = new(() => (
        from m in typeof(QuantityValue).GetMethods(BindingFlags.Public | BindingFlags.Static)
        where m.Name == "op_Implicit"
        select m.GetParameters().First().ParameterType).ToArray());

    private static readonly Lazy<ConcurrentDictionary<Type, ConstructorInfo>> LazyQuantityConstructorTable = new(() => new());

    public static string ExtractPropertyName<TObject, TPropertyValue>(this Expression<Func<TObject, TPropertyValue>> propertySelectorExpression)
    {
        var expression = propertySelectorExpression.Body;

        while (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
            expression = unaryExpression.Operand;

        if (expression is not MemberExpression memberExpression || memberExpression.Member is not PropertyInfo property || property.GetGetMethod()?.IsPublic != true)
            throw new InvalidOperationException($"{{{propertySelectorExpression}}} is not a valid property accessor.");

        return property.Name;
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

    private static TMetadata GetQuantityMetadata<TMetadataAttribute, TMetadata>(
        this PropertyInfo property,
        IMetadataProvider<TMetadataAttribute, TMetadata> metadataProvider,
        CultureInfo? culture = null)
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
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

    public static double GetQuantityValueFromProperty<TObject>(this TObject obj, PropertyInfo property)
    {
        var getter = EphemeralValueCache<(Type, Type, string), MethodInfo>.Instance.GetOrAdd((property.DeclaringType, property.PropertyType, property.Name), p =>
        {
            var getter = property.GetGetMethod() ?? throw new InvalidOperationException($"{property.DeclaringType}.{property.Name} does not have a public getter.");
            if (!LazyQuantityValueCompatibleTypes.Value.Contains(getter.ReturnType))
                throw new InvalidOperationException($"{property.DeclaringType}.{property.Name} type of {getter.ReturnType} is not compatible with {typeof(QuantityValue)}.");

            return getter;
        });
        return getter is not null
            ? Convert.ToDouble(getter.Invoke(obj, new object[] { }))
            : default;
    }

    public static IQuantity AsQuantity(this double value, Enum unit, Type quantityType)
    {
        if (!unit.TryGetQuantityInfo(quantityType, out var quantityInfo))
            throw new ArgumentException($"{unit.GetType().Name} is not a known unit type.");
        if (!unit.TryGetUnitInfo(quantityType, out var unitInfo))
            throw new ArgumentException($"{unit.GetType().Name}.{unit} is not a known unit value.");

        if (Quantity.TryFrom(value, unit, out var quantity))
            return quantity!;

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

    public static (UnitMetadata fromMetadata, UnitMetadata toMetadata) GetConversionMetadatas<TMetadataAttribute, TMetadata>(
        this PropertyInfo property,
        Enum to,
        IMetadataProvider<TMetadataAttribute, TMetadata>? metadataProvider = null,
        CultureInfo? culture = null)
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        metadataProvider ??= GlobalMetadataProvider<TMetadataAttribute, TMetadata>.Instance;

        var metadata = property.GetQuantityMetadata(metadataProvider, culture);
        var conversionMetadata = metadata.Conversions.FirstOrDefault(c => c.UnitInfo.Value.Equals(to))
            ?? throw new InvalidOperationException($"{property.DeclaringType.Name}.{property.Name} ({metadata.Unit!.UnitInfo.Value}) cannot be converted to {to}.");
        var toMetadata = UnitMetadata.FromUnitInfo(conversionMetadata.UnitInfo, conversionMetadata.QuantityType.QuantityInfo, culture);

        return (metadata.Unit!, toMetadata);
    }

    public static bool TryGetInterfaceProperty(this PropertyInfo concreteProperty, Type interfaceType, [NotNullWhen(true)] out PropertyInfo? interfaceProperty)
    {
        interfaceProperty = default;
        if (!interfaceType.IsInterface)
            return false;

        var interfacePropertyMap = concreteProperty.DeclaringType.GetInterfacePropertyMap();

        return interfacePropertyMap.TryGetValue(concreteProperty, out interfaceProperty);
    }

    public static bool TryGetVirtualProperty(this PropertyInfo concreteProperty, Type superType, [NotNullWhen(true)] out PropertyInfo? virtualProperty)
    {
        var superTypeProperties = superType.GetProperties();
        var virtualProperties = superTypeProperties.Where(p => (p.GetAccessors().All(p => p.IsVirtual) || p.GetAccessors().All(p => p.IsAbstract)) && !p.GetAccessors().Any(p => p.IsFinal));

        virtualProperty = virtualProperties.FirstOrDefault(p => p.Name == concreteProperty.Name);
        if (virtualProperty is null)
            return false;

        return true;
    }

    private static IReadOnlyDictionary<PropertyInfo, PropertyInfo> GetInterfacePropertyMap(this Type concreteType)
    {
        return EphemeralValueCache<Type, IReadOnlyDictionary<PropertyInfo, PropertyInfo>>.Instance.GetOrAdd(concreteType, type =>
        {
            return new Dictionary<PropertyInfo, PropertyInfo>(BuildInterfacePropertyMap(type));
        });
    }

    private static IEnumerable<KeyValuePair<PropertyInfo, PropertyInfo>> BuildInterfacePropertyMap(Type concreteType)
    {
        var concreteProperties = concreteType.GetProperties();

        foreach (var interfaceType in concreteType.GetInterfaces())
        {
            var map = concreteType.GetInterfaceMap(interfaceType);
            var interfaceProperties = interfaceType.GetProperties();

            foreach (var interfaceProperty in interfaceProperties)
            {
                for (var i = 0; i < map.InterfaceMethods.Length; i++)
                {
                    var interfaceMethod = map.InterfaceMethods[i];
                    if (!interfaceProperty.GetAccessors().Contains(interfaceMethod))
                        continue;

                    var concreteMethod = map.TargetMethods[i];
                    var concreteProperty = concreteProperties.SingleOrDefault(p => p.GetAccessors().Contains(concreteMethod));
                    if (concreteProperty is not null)
                    {
                        yield return new(concreteProperty, interfaceProperty);
                        break;
                    }
                }
            }
        }
    }
}