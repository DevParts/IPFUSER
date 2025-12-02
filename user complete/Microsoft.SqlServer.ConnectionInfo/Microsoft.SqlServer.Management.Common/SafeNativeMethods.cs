using System;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Common;

internal class SafeNativeMethods
{
	private static readonly string ConnectionInfoExtendedFullAssemblyName = "Microsoft.SqlServer.ConnectionInfoExtended.dll";

	private static readonly string ConnectionInfoExtendedAssemblyName = "ConnectionInfoExtended";

	private static MethodInfo GetMethodFromConnectionInfoExtended(string methodName)
	{
		try
		{
			string assemblyName = typeof(SafeNativeMethods).GetAssembly().FullName.ToLowerInvariant().Replace("connectioninfo", ConnectionInfoExtendedAssemblyName);
			Assembly assembly = NetCoreHelpers.LoadAssembly(assemblyName);
			Module module = assembly.GetModules()[0];
			Type type = module.GetType("Microsoft.SqlServer.Management.Common.SafeNativeMethodsExtended");
			return type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}
		catch (Exception ex)
		{
			throw new Exception(StringConnectionInfo.AssemblyLoadFailed(ConnectionInfoExtendedFullAssemblyName, ex.Message));
		}
	}

	internal static string GetLastErrorMessage(int errorCode)
	{
		MethodInfo methodFromConnectionInfoExtended = GetMethodFromConnectionInfoExtended("GetLastErrorMessage");
		return (string)methodFromConnectionInfoExtended.Invoke(null, new object[1] { errorCode });
	}

	internal static int GetUserToken(string user, string domain, string password, out IntPtr hToken)
	{
		hToken = IntPtr.Zero;
		MethodInfo methodFromConnectionInfoExtended = GetMethodFromConnectionInfoExtended("GetUserToken");
		object[] array = new object[methodFromConnectionInfoExtended.GetParameters().Length];
		array[0] = user;
		array[1] = domain;
		array[2] = password;
		int result = (int)methodFromConnectionInfoExtended.Invoke(null, array);
		hToken = (IntPtr)array[3];
		return result;
	}
}
