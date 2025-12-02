using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal static class NetCoreExtension
{
	public static bool IsPrimitive(this Type type)
	{
		return type.IsPrimitive;
	}

	public static bool IsEnum(this Type type)
	{
		return type.IsEnum;
	}

	public static bool IsAssignableFrom(this Type type, Type c)
	{
		return type.IsAssignableFrom(c);
	}

	public static Assembly Assembly(this Type type)
	{
		return type.Assembly;
	}

	public static Type GetInterface(this Type type, string name)
	{
		return type.GetInterface(name);
	}

	public static PropertyInfo[] GetProperties(this Type type)
	{
		return type.GetProperties();
	}

	public static string Copy(this string value)
	{
		return string.Copy(value);
	}

	public static int Compare(this string strA, string strB, bool ignoreCase, CultureInfo culture)
	{
		return culture.CompareInfo.Compare(strA, strB, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
	}

	public static Delegate CreateDelegate(this MethodInfo methodInfo, Type type)
	{
		return Delegate.CreateDelegate(type, methodInfo);
	}

	public static Assembly GetAssembly(this Type type)
	{
		return System.Reflection.Assembly.GetAssembly(type);
	}
}
