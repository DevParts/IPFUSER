using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

internal class AttributeUtilities
{
	public static object[] GetValuesOfProperties(object instance, string[] properties)
	{
		object[] array = new object[properties.Length];
		int num = 0;
		foreach (string propertyName in properties)
		{
			object obj = GetValueOfProperty(instance, SplitNames(propertyName));
			if (obj != null && obj.GetType() == typeof(string))
			{
				obj = SfcSecureString.EscapeSquote((string)obj);
			}
			array[num++] = obj;
		}
		return array;
	}

	public static object GetValueOfProperty(object instance, string name)
	{
		PropertyInfo pInfo = null;
		if (!SfcMetadataDiscovery.TryGetCachedPropertyInfo(instance.GetType().TypeHandle, name, out pInfo))
		{
			try
			{
				pInfo = instance.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
			}
			catch (AmbiguousMatchException)
			{
				pInfo = instance.GetType().GetProperty(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			}
		}
		return pInfo.GetValue(instance, null);
	}

	public static object GetValueOfProperty(object instance, List<string> names)
	{
		object obj = instance;
		foreach (string name in names)
		{
			obj = GetValueOfProperty(obj, name);
		}
		return obj;
	}

	public static List<string> SplitNames(string propertyName)
	{
		List<string> list = new List<string>();
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		bool flag = false;
		while (num < propertyName.Length)
		{
			char c = propertyName[num++];
			switch (c)
			{
			case '\\':
				if (flag)
				{
					stringBuilder.Append(c);
					flag = false;
				}
				else
				{
					flag = true;
				}
				break;
			case '.':
				if (flag)
				{
					stringBuilder.Append(c);
					flag = false;
				}
				else
				{
					list.Add(stringBuilder.ToString());
					stringBuilder = new StringBuilder();
				}
				break;
			default:
				if (flag)
				{
					stringBuilder.Append('\\');
					stringBuilder.Append(c);
					flag = false;
				}
				else
				{
					stringBuilder.Append(c);
				}
				break;
			}
		}
		if (flag)
		{
			stringBuilder.Append('\\');
		}
		if (stringBuilder.Length > 0)
		{
			list.Add(stringBuilder.ToString());
		}
		return list;
	}
}
