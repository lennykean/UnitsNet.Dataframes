using System.Reflection;

using Castle.DynamicProxy;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes.Dynamic;

internal class DynamicQuantityInterceptor<TDataframe, TMetadataAttribute, TMetadata> : IInterceptor
    where TDataframe : class
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
{
    public DynamicQuantityInterceptor(DynamicDataframeMetadataProvider<TMetadataAttribute, TMetadata> metadataProvider)
    {
        MetadataProvider = metadataProvider;
    }

    public DynamicDataframeMetadataProvider<TMetadataAttribute, TMetadata> MetadataProvider { get; }

    public void Intercept(IInvocation invocation)
    {
        invocation.Proceed();

        if (typeof(IDataframeMetadataProvider<TMetadataAttribute, TMetadata>).IsAssignableFrom(invocation.TargetType))
            return;

        var targetMethod = invocation.GetConcreteMethodInvocationTarget();
        var targetProperty = targetMethod.ReflectedType.GetProperties().SingleOrDefault(p => p.GetMethod == targetMethod || p.SetMethod == targetMethod);
        if (targetProperty is null || !MetadataProvider.TryGetBaseMetadata(targetProperty, out var targetMetadata) || targetMetadata.Unit is null)
            return;

        var method = invocation.Method;
        var property = method.ReflectedType.GetProperties().SingleOrDefault(p => p.GetMethod == method || p.SetMethod == method);
        if (property is null || !MetadataProvider.TryGetMetadata(property, out var metadata) || metadata.Unit is null)
            return;

        if (targetMetadata.Unit.UnitInfo.Value == metadata.Unit.UnitInfo.Value)
            return;

        if (targetProperty.GetMethod == targetMethod)
        {
            var quantity = ConvertQuantity(from: targetMetadata.Unit, to: metadata.Unit, value: invocation.ReturnValue);
            invocation.ReturnValue = Convert.ChangeType(quantity.Value, property.PropertyType);
        }
        else
        {
            var quantity = ConvertQuantity(from: metadata.Unit, to: targetMetadata.Unit, value: invocation.Arguments[0]);
            targetProperty.SetValue(invocation.InvocationTarget, Convert.ChangeType(quantity.Value, property.PropertyType));
        }
    }

    private static IQuantity ConvertQuantity(UnitMetadata from, UnitMetadata to, object value)
    {
        if (!from.TryConvertQuantity(Convert.ToDouble(value), to, out var quantity))
            throw new InvalidOperationException($"Invalid unit conversion from {from.UnitInfo.Value} to {to.UnitInfo.Value}.");

        return quantity;
    }
}