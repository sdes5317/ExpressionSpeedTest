// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;
using System.Reflection;

Console.WriteLine("It's a speed test for set ");

var person = new Person() { Name = "a" };
var info = person.GetType().GetProperty("Name");

Console.WriteLine($"----------------------{nameof(Helper.SetValue)}----------------------");
Helper.SetValue(info, person, "777");
Console.WriteLine(person.Name);

Console.WriteLine($"----------------------{nameof(Helper.SetValue2)}----------------------");
Helper.SetValue2(info, person, "777");
Console.WriteLine(person.Name);

Console.WriteLine($"----------------------{nameof(Helper.Add)}----------------------");
Console.WriteLine(Helper.Add(1, 7));

public class Helper
{
    public static void SetValue<T, K>(PropertyInfo propertyInfo, T propObject, K newValue)
    {
        var SetValueAction = GetSetMethod<T, K>(propertyInfo);
        SetValueAction(propObject, newValue);
    }

    private static Action<T, K> GetSetMethod<T, K>(PropertyInfo propertyInfo)
    {
        var a = Expression.Parameter(typeof(T), "a");
        var b = Expression.Parameter(typeof(K), "b");
        var call = Expression.Call(a, propertyInfo.GetSetMethod(), b);
        Console.WriteLine(call);
        return Expression.Lambda<Action<T, K>>(call, a, b).Compile();
    }
    public static void SetValue2<T, K>(PropertyInfo propertyInfo, T propObject, K newValue)
    {
        var SetValueAction = GetSetMethod2<T, K>(propertyInfo);
        SetValueAction(propObject, newValue);
    }
    private static Action<T, K> GetSetMethod2<T, K>(PropertyInfo propertyInfo)
    {
        var objectExpresssion = Expression.Parameter(typeof(T));
        var access = Expression.MakeMemberAccess(objectExpresssion, propertyInfo);
        var b = Expression.Parameter(typeof(K), "b");
        var assign = Expression.Assign(access, b);
        Console.WriteLine(assign);
        return Expression.Lambda<Action<T,K>>(assign, objectExpresssion, b).Compile();
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
public class Person
{
    public string Name { get; set; }
}
