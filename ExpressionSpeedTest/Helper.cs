// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;
using System.Reflection;
/// <summary>
/// 練習實作Expression
/// </summary>
public class Helper
{
    public static object GetPropertyValue<TInstance>(PropertyInfo propertyInfo, TInstance propObject)
    {
        var getVlaueFunc = GetMethod<TInstance>(propertyInfo);

        return getVlaueFunc(propObject);
    }

    private static Func<TInstance, object> GetMethod<TInstance>(PropertyInfo propertyInfo)
    {
        var instance = Expression.Parameter(typeof(TInstance));
        var access = Expression.MakeMemberAccess(instance, propertyInfo);
        Console.WriteLine(access);
        return Expression.Lambda<Func<TInstance, object>>(access, instance).Compile();
    }

    public static void SetValue<TInstance, TNewPropertyValue>(PropertyInfo propertyInfo, TInstance propObject, TNewPropertyValue newValue)
    {
        var SetValueAction = GetSetMethod<TInstance, TNewPropertyValue>(propertyInfo);
        SetValueAction(propObject, newValue);
    }

    private static Action<TInstance, TNewPropertyValue> GetSetMethod<TInstance, TNewPropertyValue>(PropertyInfo propertyInfo)
    {
        var a = Expression.Parameter(typeof(TInstance), "a");
        var b = Expression.Parameter(typeof(TNewPropertyValue), "b");
        var call = Expression.Call(a, propertyInfo.GetSetMethod(), b);
        //Console.WriteLine(call);
        return Expression.Lambda<Action<TInstance, TNewPropertyValue>>(call, a, b).Compile();
    }
    public static void SetValue2<TInstance, TNewPropertyValue>(PropertyInfo propertyInfo, TInstance propObject, TNewPropertyValue newValue)
    {
        var SetValueAction = GetSetMethod2<TInstance, TNewPropertyValue>(propertyInfo);
        SetValueAction(propObject, newValue);
    }
    private static Action<TInstance, TNewPropertyValue> GetSetMethod2<TInstance, TNewPropertyValue>(PropertyInfo propertyInfo)
    {
        var instance = Expression.Parameter(typeof(TInstance));
        var propertyAccess = Expression.MakeMemberAccess(instance, propertyInfo);
        var b = Expression.Parameter(typeof(TNewPropertyValue), "b");
        var assign = Expression.Assign(propertyAccess, b);
        //Console.WriteLine(assign);
        return Expression.Lambda<Action<TInstance, TNewPropertyValue>>(assign, instance, b).Compile();
    }

    public static int Add(int a, int b)
    {
        var parameterA = Expression.Parameter(typeof(int), "a");
        var parameterB = Expression.Parameter(typeof(int), "b");

        var body = Expression.AddAssign(parameterA, parameterB);
        Console.WriteLine(body);
        var methodAdd = Expression.Lambda<Func<int, int, int>>(body, parameterA, parameterB).Compile();

        return methodAdd(a, b);
    }

}
