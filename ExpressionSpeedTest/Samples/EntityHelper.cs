using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 實作1
/// </summary>
public static class EntityHelper
{
    #region Property 操作
    private static ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> s_properties = new ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();
    private static ConcurrentDictionary<int, Func<object, object>> s_getValueFuncs = new ConcurrentDictionary<int, Func<object, object>>();
    private static ConcurrentDictionary<int, Action<object, object>> s_setValueActions = new ConcurrentDictionary<int, Action<object, object>>();

    /// <summary>
    /// 取得所有屬性
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<PropertyInfo> GetProperties(Type type)
    {
        return GetProperties_Interal(type).Values.OrderBy(p => p.MetadataToken).ToList();
    }

    /// <summary>
    /// 取得所有Key屬性
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<PropertyInfo> GetKeyProperties(Type type)
    {
        return GetProperties_Interal(type).Values
                .Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute))
                .OrderBy(p => p.MetadataToken).ToList();
    }

    private static Dictionary<string, PropertyInfo> GetProperties_Interal(Type type)
    {
        if (s_properties.TryGetValue(type, out Dictionary<string, PropertyInfo> result))
        {
            return result;
        }
        else
        {
            var properties = type.GetProperties().ToDictionary(p => p.Name);
            s_properties.TryAdd(type, properties);
            return properties;
        }
    }

    /// <summary>
    /// 取得屬性值，若無法取得屬性值則回傳@default
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="propertyName"></param>
    /// <param name="default"></param>
    /// <returns></returns>
    public static TResult GetValue<TResult>(object source, string propertyName, TResult @default = default)
    {
        var properties = GetProperties_Interal(source.GetType());
        if (properties.TryGetValue(propertyName, out var propertyInfo))
        {
            return (TResult)GetValue(source, propertyInfo);
        }
        else
        {
            return @default;
        }
    }

    private static object GetValue(object source, PropertyInfo property)
    {
        if (s_getValueFuncs.TryGetValue(property.MetadataToken, out var getValueFunc))
        {
            return getValueFunc(source);
        }
        else
        {
            var func = CreateGetPropertyValueFunc(source.GetType(), property);
            s_getValueFuncs.TryAdd(property.MetadataToken, func);
            return func(source);
        }
    }

    private static Func<object, object> CreateGetPropertyValueFunc(Type sourceType, PropertyInfo property)
    {
        // 比Reflection速度快0.3倍左右
        // source => (object)((sourceType)source.property)
        var source = Expression.Parameter(typeof(object), "source");
        var castSource = Expression.Convert(source, sourceType);
        var getPropertyValue = Expression.Property(castSource, property);
        var castPropertyValue = Expression.Convert(getPropertyValue, typeof(object));
        return Expression.Lambda<Func<object, object>>(castPropertyValue, source).Compile();
    }

    /// <summary>
    /// 設定屬性值
    /// </summary>
    /// <param name="source"></param>
    /// <param name="propertyName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TrySetValue(object source, string propertyName, object value)
    {
        var sourceType = source.GetType();
        var properties = GetProperties_Interal(sourceType);
        if (properties.TryGetValue(propertyName, out var propertyInfo))
        {
            if (s_setValueActions.TryGetValue(propertyInfo.MetadataToken, out var setValueAction))
            {
                setValueAction(source, value);
                return true;
            }
            else
            {
                var action = CreateSetPropertyValueAction(sourceType, propertyInfo);
                s_setValueActions.TryAdd(propertyInfo.MetadataToken, action);
                action(source, value);
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    private static Action<object, object> CreateSetPropertyValueAction(Type sourceType, PropertyInfo property)
    {
        // 比Reflection速度快1倍左右
        // source => (sourceType)source.property = (propertyType)value
        var target = Expression.Parameter(typeof(object), "source");
        var propertyValue = Expression.Parameter(typeof(object), "value");
        var castTarget = Expression.Convert(target, sourceType);
        var castPropertyValue = Expression.Convert(propertyValue, property.PropertyType);
        var setPropertyValue = Expression.Call(castTarget, property.GetSetMethod(), castPropertyValue);
        return Expression.Lambda<Action<object, object>>(setPropertyValue, target, propertyValue).Compile();
    }
    #endregion

    #region SQL
    /// <summary>
    /// Get Insert SQL
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static string GetInsertSql<T>(string tableName = default)
    {
        if (tableName == default) tableName = typeof(T).Name;
        var propertites = GetProperties(typeof(T)).Select(p => p.Name);
        return $"INSERT INTO {tableName} ({string.Join(",", propertites)}) VALUES ({string.Join(",", propertites.Select(p => $"@{p}"))})";
    }

    /// <summary>
    /// Get Update SQL, key default = 有[Key]的所有屬性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableName"></param>
    /// <param name="keys"></param>
    /// <returns></returns>
    public static string GetUpdateSql<T>(string tableName = default, IEnumerable<string> keys = default)
    {
        if (tableName == default) tableName = typeof(T).Name;
        if (keys == default) keys = GetKeyProperties(typeof(T)).Select(p => p.Name);
        if (keys.Count() == 0) throw new ArgumentException("no keys");
        var propertites = GetProperties(typeof(T)).Select(p => p.Name);
        var setProperties = propertites.Except(keys);
        return $"UPDATE {tableName} " +
               $"SET {string.Join(", ", setProperties.Select(p => $"{p} = @{p}"))} " +
               $"WHERE {string.Join(" AND ", keys.Select(k => $"{k} = @{k}"))}";
    }

    /// <summary>
    /// Get Delete SQL, key default = 有[Key]的所有屬性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableName"></param>
    /// <param name="keys"></param>
    /// <returns></returns>
    public static string GetDeleteSql<T>(string tableName = default, IEnumerable<string> keys = default)
    {
        if (tableName == default) tableName = typeof(T).Name;
        if (keys == default) keys = GetKeyProperties(typeof(T)).Select(p => p.Name);
        if (keys.Count() == 0) throw new ArgumentException("no keys");
        return $"DELETE {tableName} WHERE {string.Join(" AND ", keys.Select(k => $"{k} = @{k}"))}";
    }

    #endregion

    public static DateTime ToDateTime(string dateTime)
    {
        return Convert.ToDateTime(dateTime);
    }

    public static DateTime? ToDateTimeOrNull(string dateTime)
    {
        if (string.IsNullOrWhiteSpace(dateTime)) return null;
        try
        {
            return Convert.ToDateTime(dateTime);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static DateTime ToYearMonth(string fyymm)
    {
        return ToDateTime(fyymm);
    }

    public static string ToDateTimeString(DateTime dateTime, string format)
    {
        if (dateTime != default)
        {
            return dateTime.ToString(format);
        }
        else
        {
            throw new ArgumentException($"Not Allow {dateTime}");
        }
    }


    public static string SubstringString(string source, int length)
    {
        if (source.Length > length)
        {
            return source.Substring(0, length);
        }
        else
        {
            return source.Substring(0, source.Length);
        }
    }
}

