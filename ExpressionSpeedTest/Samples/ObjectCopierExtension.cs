// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// 實作2
/// </summary>
public static class ObjectCopierExtension
{
    public static Func<T, object> GetValueGetter<T>(this PropertyInfo propertyInfo)
    {
        var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
        var property = Expression.Property(instance, propertyInfo);
        var convert = Expression.TypeAs(property, typeof(object));
        return (Func<T, object>)Expression.Lambda(convert, instance).Compile();
    }

    public static Action<T, object> GetValueSetter<T>(this PropertyInfo propertyInfo)
    {
        var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
        var argument = Expression.Parameter(typeof(object), "a");
        var setterCall = Expression.Call(
            instance,
            propertyInfo.GetSetMethod(),
            Expression.Convert(argument, propertyInfo.PropertyType));
        return (Action<T, object>)Expression.Lambda(setterCall, instance, argument).Compile();
    }
    public static R ExpressionGetValue<T, R>(this PropertyInfo property, T reflectedType)
    {
        var getter = property.GetValueGetter<T>();

        return (R)getter(reflectedType);
    }
    public static void ExpressionSetValue<T>(this PropertyInfo property, T reflectedType, object newValue)
    {
        var setter = property.GetValueSetter<T>();

        setter(reflectedType, newValue);
    }
}
