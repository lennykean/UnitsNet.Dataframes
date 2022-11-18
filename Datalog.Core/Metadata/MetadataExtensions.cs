using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public static class MetadataExtensions
    {
        public static IQuantity GetQuantity<T>(this T obj, Expression<Func<T, QuantityValue>> propertySelectorExpression)
        {
            var expression = propertySelectorExpression.Body;

            while (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
                expression = unaryExpression.Operand;

            if (expression is not MemberExpression memberExpression || memberExpression.Member is not PropertyInfo property)
                throw new InvalidOperationException($"{{{propertySelectorExpression}}} is not a valid property accessor.");

            var metadata = ObjectMetadata<T>.GetUnitMetadata(property);
            if (metadata?.UnitInfo == null)
                throw new InvalidOperationException($"Unit metadata does not exist for property {property.DeclaringType}.{property.Name}.");

            var propertySelector = propertySelectorExpression.Compile();

            return Quantity.From(propertySelector(obj), metadata.UnitInfo.Value);
        }

        public static IQuantity GetQuantity<T>(this T obj, string propertyName)
        {
            var metadata = ObjectMetadata<T>.GetUnitMetadata(propertyName);
            if (metadata?.UnitInfo == null)
                throw new InvalidOperationException($"Unit metadata does not exist for property {propertyName}.");

            return Quantity.From(Convert.ToDouble(typeof(T).GetProperty(propertyName).GetValue(obj)), metadata.UnitInfo.Value);
        }
    }
}