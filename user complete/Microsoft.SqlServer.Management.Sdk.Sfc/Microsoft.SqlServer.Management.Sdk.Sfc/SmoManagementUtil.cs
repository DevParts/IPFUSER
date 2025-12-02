using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Microsoft.SqlServer.Smo.UnSafeInternals;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal static class SmoManagementUtil
{
	internal static void EnterMonitor(object lockObject)
	{
		ManagementUtil.EnterMonitor(lockObject);
	}

	internal static void ExitMonitor(object lockObject)
	{
		ManagementUtil.ExitMonitor(lockObject);
	}

	internal static object CreateInstance(Assembly assembly, string objectType)
	{
		return ManagementUtil.CreateInstance(assembly, objectType);
	}

	internal static Assembly LoadAssembly(string assemblyName)
	{
		return Assembly.Load(assemblyName);
	}

	internal static Assembly LoadAssemblyFromFile(string fileName)
	{
		return ManagementUtil.LoadAssemblyFromFile(fileName);
	}

	internal static Stream LoadResourceFromAssembly(Assembly assembly, string resourceFileName)
	{
		return ManagementUtil.LoadResourceFromAssembly(assembly, resourceFileName);
	}

	internal static string GetAssemblyName(Assembly assembly)
	{
		return ManagementUtil.GetAssemblyName(assembly);
	}

	internal static Assembly GetExecutingAssembly()
	{
		return Assembly.GetExecutingAssembly();
	}

	internal static TypeConverter GetTypeConverter(Type t)
	{
		return ManagementUtil.GetTypeConverter(t);
	}
}
