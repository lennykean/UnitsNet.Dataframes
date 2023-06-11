using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata;

public interface IMetadataProvider<TMetadataAttribue, TMetadata>
    where TMetadataAttribue : QuantityAttribute, IMetadataAttribute<TMetadataAttribue, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null);

    void ValidateMetadata(PropertyInfo property);

    public virtual IEnumerable<TMetadata> GetAllMetadata(Type dataframeType, CultureInfo? culture = null)
    {
        return EphemeralValueCache<Type, IEnumerable<TMetadata>>.Instance.GetOrAdd(dataframeType, type =>
        {
            IEnumerable<TMetadata> get(Type t) => (
                from property in t.GetProperties()
                let m = (hasMetadata: TryGetMetadata(property, out var metadata, culture), metadata)
                where m.hasMetadata
                select m.metadata)
                .ToList();

            var metadata = get(type);

            if (!metadata.Any())
            {
                var elementType = type switch
                {
                    { IsArray: true } => type.GetElementType(),
                    { IsInterface: true } when type.GetGenericTypeDefinition() == typeof(IEnumerable<>) => type.GenericTypeArguments[0],
                    { IsInterface: true } => (
                        from interfaceType in type.GetInterfaces()
                        where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                        select interfaceType.GenericTypeArguments[0])
                        .SingleOrDefault(),
                    _ => (
                        from interfaceType in type.GetInterfaces()
                        where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                        let interfaceMap = type.GetInterfaceMap(interfaceType)
                        from interfaceMethod in interfaceMap.InterfaceMethods.Select((method, index) => (method, index))
                        where interfaceMethod.method.Name == "GetEnumerator" && !interfaceMethod.method.GetParameters().Any()
                        select interfaceMap.TargetMethods[interfaceMethod.index].DeclaringType.GenericTypeArguments[0])
                        .SingleOrDefault()
                };
                if (elementType != null)
                    metadata = get(elementType);
            }
            return metadata;
        });
    }

    public virtual void ValidateAllMetadata(Type dataframeType)
    {
        foreach (var metadata in GetAllMetadata(dataframeType))
            ValidateMetadata(metadata.Property);
    }
}
