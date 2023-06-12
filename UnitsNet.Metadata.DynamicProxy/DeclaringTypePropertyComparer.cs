using System.Reflection;

namespace UnitsNet.Metadata.DynamicProxy;

internal class DeclaringTypePropertyComparer : IEqualityComparer<PropertyInfo>
{
    public bool Equals(PropertyInfo x, PropertyInfo y)
    {
        return x.Name == y.Name && x.PropertyType == y.PropertyType && x.DeclaringType == x.DeclaringType;
    }

    public int GetHashCode(PropertyInfo obj)
    {
        return HashCode.Combine(obj.Name, obj.PropertyType, obj.DeclaringType);
    }
}