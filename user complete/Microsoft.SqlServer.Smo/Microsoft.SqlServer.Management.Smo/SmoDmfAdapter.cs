using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class SmoDmfAdapter
{
	private static readonly PropertyInfo SmoSchemaPropertyInfo = typeof(ScriptSchemaObjectBase).GetProperty("Schema");

	private static readonly PropertyInfo SmoNamePropertyInfo = typeof(NamedSmoObject).GetProperty("Name");

	internal static PropertyInfo[] GetTypeProperties(Type type)
	{
		PropertyInfo propertyInfo = null;
		List<PropertyInfo> list = new List<PropertyInfo>();
		Type nestedType = type.GetNestedType("PropertyMetadataProvider", BindingFlags.NonPublic);
		if (null != nestedType)
		{
			StaticMetadata[] array = (StaticMetadata[])nestedType.GetField("staticMetadata", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			StaticMetadata[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				StaticMetadata staticMetadata = array2[i];
				if (!(staticMetadata.Name == "PolicyHealthState"))
				{
					propertyInfo = type.GetProperty(staticMetadata.Name);
					if (null != propertyInfo && propertyInfo.GetCustomAttributes(typeof(DmfIgnorePropertyAttribute), inherit: false).Length == 0)
					{
						list.Add(propertyInfo);
					}
				}
			}
		}
		if (type.IsSubclassOf(typeof(ScriptSchemaObjectBase)))
		{
			list.Add(SmoSchemaPropertyInfo);
		}
		if (type.IsSubclassOf(typeof(NamedSmoObject)))
		{
			list.Add(SmoNamePropertyInfo);
		}
		return list.ToArray();
	}

	internal static PropertyInfo[] GetTypeFilterProperties(string skeleton)
	{
		return GetTypeFilterProperties(SqlSmoObject.GetTypeFromUrnSkeleton(new Urn(skeleton)));
	}

	internal static PropertyInfo[] GetTypeFilterProperties(Type type)
	{
		if (null == type)
		{
			return null;
		}
		List<PropertyInfo> list = new List<PropertyInfo>();
		PropertyInfo property = type.GetProperty("Parent", BindingFlags.Instance | BindingFlags.Public);
		if (type.IsSubclassOf(typeof(NamedSmoObject)) && null != property && property.PropertyType == typeof(Server))
		{
			list.Add(SmoNamePropertyInfo);
		}
		return list.ToArray();
	}
}
