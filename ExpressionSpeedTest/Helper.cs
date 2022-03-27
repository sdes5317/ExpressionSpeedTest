// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using Newtonsoft.Json;

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
    private static Action<object, object> _setMethodTemp;
    public static void SetValueByTempCache<TInstance, TNewPropertyValue>(PropertyInfo propertyInfo, TInstance propObject, TNewPropertyValue newValue)
    {
        if (_setMethodTemp is null)
        {
            _setMethodTemp += GetSetMethod3<TInstance, TNewPropertyValue>(propertyInfo);
        }
        _setMethodTemp(propObject, newValue);
    }
    private static dynamic _setMethodDynamic;
    public static void SetValueByCacheDynamic<TInstance, TNewPropertyValue>(PropertyInfo propertyInfo, TInstance propObject, TNewPropertyValue newValue)
    {
        if (_setMethodDynamic is null)
        {
            _setMethodDynamic += GetSetMethod2<TInstance, TNewPropertyValue>(propertyInfo);
        }
        _setMethodDynamic(propObject, newValue);
    }

    private static Action<object, object> GetSetMethod3<TInstance, TNewPropertyValue>(PropertyInfo propertyInfo)
    {
        var a = Expression.Parameter(typeof(object), "a");
        var b = Expression.Parameter(typeof(object), "b");
        var castA = Expression.Convert(a, typeof(TInstance));
        var castB = Expression.Convert(b, typeof(TNewPropertyValue));
        var call = Expression.Call(castA, propertyInfo.GetSetMethod(), castB);
        //Console.WriteLine(call);
        return Expression.Lambda<Action<object, object>>(call, a, b).Compile();
    }

    public static void SetValueByRefleaction<TInstance, TNewPropertyValue>(PropertyInfo propertyInfo, TInstance propObject, TNewPropertyValue newValue)
    {
        propertyInfo.SetValue(propObject, newValue);
    }
    private static dynamic _reflectionSetter;
    public static void SetValueByRefleactionCache<TInstance, TNewPropertyValue>(PropertyInfo propertyInfo, TInstance propObject, TNewPropertyValue newValue)
    {
        if (_reflectionSetter is null)
        {
            _reflectionSetter = propertyInfo.GetValueSetter<TInstance>();
        }
        _reflectionSetter(propObject, newValue);
    }

    public static int Add(int a, int b)
    {
        var parameterA = Expression.Parameter(typeof(int), "a");
        var parameterB = Expression.Parameter(typeof(int), "b");

        var body = Expression.AddAssign(parameterA, parameterB);
        Console.WriteLine(body);
        //var methodAdd = Expression.Lambda<Func<int, int, int>>(body, parameterA, parameterB).Compile();
        Expression<Func<int, int, int>> methodAddEx = (a, b) => a + b;
        var methodAdd = methodAddEx.Compile();

        return methodAdd(a, b);
    }

    private static IMapper _mapper;
    public static void SetValueByAutoMapper<TInstance, TNewInstance>(TInstance propObject, TNewInstance newValue)
    {
        if (_mapper is null)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TInstance, TNewInstance>();
            });
            _mapper = config.CreateMapper();
        }

        _mapper.Map(newValue, propObject);
    }
    public static void SetValueByAutoMapperNoCache<TInstance, TNewInstance>(TInstance propObject, TNewInstance newValue)
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TInstance, TNewInstance>();
        });
        _mapper = config.CreateMapper();

        _mapper.Map(newValue, propObject);
    }
    public static void SetValueByJsonDotNet<TInstance, TNewInstance>(TInstance propObject, TNewInstance newValue)
    {
        propObject = JsonConvert.DeserializeObject<TInstance>(JsonConvert.SerializeObject(newValue));
    }
}
