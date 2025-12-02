using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLog.Common;
using NLog.Config;

namespace NLog.Internal;

/// <summary>
/// Scans (breadth-first) the object graph following all the edges whose are
/// instances have <see cref="T:NLog.Config.NLogConfigurationItemAttribute" /> attached and returns
/// all objects implementing a specified interfaces.
/// </summary>
internal static class ObjectGraphScanner
{
	/// <summary>
	/// Finds the objects which have attached <see cref="T:NLog.Config.NLogConfigurationItemAttribute" /> which are reachable
	/// from any of the given root objects when traversing the object graph over public properties.
	/// </summary>
	/// <typeparam name="T">Type of the objects to return.</typeparam>
	/// <param name="configFactory">Configuration Reflection Helper</param>
	/// <param name="aggressiveSearch">Also search the properties of the wanted objects.</param>
	/// <param name="rootObjects">The root objects.</param>
	/// <returns>Ordered list of objects implementing T.</returns>
	public static List<T> FindReachableObjects<T>(ConfigurationItemFactory configFactory, bool aggressiveSearch, params object[] rootObjects) where T : class
	{
		if (InternalLogger.IsTraceEnabled)
		{
			InternalLogger.Trace("FindReachableObject<{0}>:", typeof(T));
		}
		List<T> result = new List<T>();
		HashSet<object> visitedObjects = new HashSet<object>(SingleItemOptimizedHashSet<object>.ReferenceEqualityComparer.Default);
		foreach (object obj in rootObjects)
		{
			if (PropertyHelper.IsConfigurationItemType(configFactory, obj.GetType()))
			{
				ScanProperties(configFactory, aggressiveSearch, obj, result, 0, visitedObjects);
			}
		}
		return result;
	}

	private static void ScanProperties<T>(ConfigurationItemFactory configFactory, bool aggressiveSearch, object? targetObject, List<T> result, int level, HashSet<object> visitedObjects) where T : class
	{
		if (targetObject == null || visitedObjects.Contains(targetObject))
		{
			return;
		}
		if (targetObject is T item)
		{
			result.Add(item);
			if (!aggressiveSearch)
			{
				return;
			}
		}
		Type type = targetObject.GetType();
		if (InternalLogger.IsTraceEnabled)
		{
			InternalLogger.Trace("{0}Scanning {1} '{2}'", new string(' ', level), type.Name, targetObject);
		}
		foreach (KeyValuePair<string, PropertyInfo> allConfigItemProperty in PropertyHelper.GetAllConfigItemProperties(configFactory, type))
		{
			if (string.IsNullOrEmpty(allConfigItemProperty.Key))
			{
				continue;
			}
			PropertyInfo value = allConfigItemProperty.Value;
			if (PropertyHelper.IsConfigurationItemType(configFactory, value.PropertyType))
			{
				object obj = ScanPropertyValue(targetObject, type, value);
				if (obj != null)
				{
					visitedObjects.Add(targetObject);
					ScanPropertyForObject(configFactory, aggressiveSearch, obj, value, result, level, visitedObjects);
				}
			}
		}
	}

	private static object? ScanPropertyValue(object targetObject, Type type, PropertyInfo propInfo)
	{
		try
		{
			return propInfo.GetValue(targetObject, null);
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "Failed scanning property: {0}.{1}", type, propInfo.Name);
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			return null;
		}
	}

	private static void ScanPropertyForObject<T>(ConfigurationItemFactory configFactory, bool aggressiveSearch, object propValue, PropertyInfo prop, List<T> result, int level, HashSet<object> visitedObjects) where T : class
	{
		if (InternalLogger.IsTraceEnabled)
		{
			InternalLogger.Trace("{0}Scanning Property {1} '{2}' {3}", new string(' ', level + 1), prop.Name, propValue, prop.PropertyType);
		}
		if (propValue is IEnumerable enumerable)
		{
			IList list = ConvertEnumerableToList(enumerable, visitedObjects);
			if (list.Count > 0)
			{
				ScanPropertiesList(configFactory, aggressiveSearch, list, result, level + 1, visitedObjects);
			}
		}
		else
		{
			ScanProperties(configFactory, aggressiveSearch, propValue, result, level + 1, visitedObjects);
		}
	}

	private static void ScanPropertiesList<T>(ConfigurationItemFactory configFactory, bool aggressiveSearch, IList list, List<T> result, int level, HashSet<object> visitedObjects) where T : class
	{
		object obj = list[0];
		if (obj == null || PropertyHelper.IsConfigurationItemType(configFactory, obj.GetType()))
		{
			ScanProperties(configFactory, aggressiveSearch, obj, result, level, visitedObjects);
			for (int i = 1; i < list.Count; i++)
			{
				object targetObject = list[i];
				ScanProperties(configFactory, aggressiveSearch, targetObject, result, level, visitedObjects);
			}
		}
	}

	private static IList ConvertEnumerableToList(IEnumerable enumerable, HashSet<object> visitedObjects)
	{
		if (enumerable is ICollection { Count: 0 })
		{
			return ArrayHelper.Empty<object>();
		}
		if (visitedObjects.Contains(enumerable))
		{
			return ArrayHelper.Empty<object>();
		}
		visitedObjects.Add(enumerable);
		if (enumerable is IList list)
		{
			if (!list.IsReadOnly && !(list is Array))
			{
				List<object> list2 = new List<object>(list.Count);
				lock (list.SyncRoot)
				{
					for (int i = 0; i < list.Count; i++)
					{
						list2.Add(list[i]);
					}
					return list2;
				}
			}
			return list;
		}
		return enumerable.Cast<object>().ToList();
	}
}
