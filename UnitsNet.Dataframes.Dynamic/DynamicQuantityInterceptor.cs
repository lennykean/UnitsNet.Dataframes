using System.Reflection;

using Castle.DynamicProxy;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes.Dynamic;

internal class DynamicQuantityInterceptor<TDataframe, TMetadataAttribute, TMetadata> : IInterceptor
    where TDataframe : class
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
{
    public DynamicQuantityInterceptor(DynamicDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata> metadataProvider)
    {
        MetadataProvider = metadataProvider;
    }

    public DynamicDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata> MetadataProvider { get; }

    public void Intercept(IInvocation invocation)
    {
        invocation.Proceed();

        if (typeof(IDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata>).IsAssignableFrom(invocation.TargetType))
            return;

        var concreteMethod = invocation.GetConcreteMethodInvocationTarget();
        var concreteProperty = concreteMethod.DeclaringType!.GetProperties((BindingFlags)(-1)).SingleOrDefault(p => p.GetMethod == concreteMethod || p.SetMethod == concreteMethod);
        if (concreteProperty is null || !MetadataProvider.TryGetBaseMetadata(concreteProperty, out var concreteMetadata) || concreteMetadata.Unit is null)
            return;

        var property = invocation.Method.DeclaringType?.GetProperties((BindingFlags)(-1)).SingleOrDefault(p => p.GetMethod == invocation.Method || p.SetMethod == invocation.Method);
        if (property is null || !MetadataProvider.TryGetMetadata(property, out var metadata) || metadata.Unit is null)
            return;

        if (concreteMetadata.Unit.UnitInfo.Value == metadata.Unit.UnitInfo.Value)
            return;

        if (concreteProperty.GetMethod == concreteMethod)
        {
            var quantity = ConvertQuantity(from: concreteMetadata.Unit, to: metadata.Unit, value: invocation.ReturnValue);
            invocation.ReturnValue = Convert.ChangeType(quantity.Value, property.PropertyType);
        }
        else
        {
            var quantity = ConvertQuantity(from: metadata.Unit, to: concreteMetadata.Unit, value: invocation.Arguments[0]);
            concreteProperty.SetValue(invocation.InvocationTarget, Convert.ChangeType(quantity.Value, property.PropertyType));
        }
    }

    private static IQuantity ConvertQuantity(UnitMetadata from, UnitMetadata to, object value)
    {
        if (!from.TryConvertQuantity(Convert.ToDouble(value), to, out var quantity))
            throw new InvalidOperationException($"Invalid unit conversion from {from.UnitInfo.Value} to {to.UnitInfo.Value}.");

        return quantity;
    }
}