using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Soap;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace Microsoft.SqlServer.Smo.UnSafeInternals;

internal class ManagementUtil
{
	private static readonly byte[] msPublicKey = new byte[160]
	{
		0, 36, 0, 0, 4, 128, 0, 0, 148, 0,
		0, 0, 6, 2, 0, 0, 0, 36, 0, 0,
		82, 83, 65, 49, 0, 4, 0, 0, 1, 0,
		1, 0, 39, 39, 54, 173, 110, 95, 149, 134,
		186, 194, 213, 49, 234, 188, 58, 204, 102, 108,
		47, 142, 200, 121, 250, 148, 248, 247, 176, 50,
		125, 47, 242, 237, 82, 52, 72, 248, 60, 61,
		92, 93, 210, 223, 199, 188, 153, 197, 40, 107,
		44, 18, 81, 23, 191, 92, 190, 36, 43, 157,
		65, 117, 7, 50, 178, 189, 255, 230, 73, 198,
		239, 184, 229, 82, 109, 82, 111, 221, 19, 0,
		149, 236, 219, 123, 242, 16, 128, 156, 108, 218,
		216, 130, 79, 170, 154, 192, 49, 10, 195, 203,
		162, 170, 5, 35, 86, 123, 45, 250, 127, 226,
		80, 179, 15, 172, 189, 98, 212, 236, 153, 185,
		74, 196, 124, 125, 59, 40, 241, 246, 228, 200
	};

	[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
	internal static Assembly LoadAssembly(string fileName)
	{
		return Assembly.Load(fileName);
	}

	[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
	internal static Assembly LoadAssemblyFromFile(string fileName)
	{
		return Assembly.LoadFile(fileName);
	}

	[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
	internal static Stream LoadResourceFromAssembly(Assembly assembly, string resourceFileName)
	{
		return assembly.GetManifestResourceStream(resourceFileName);
	}

	[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
	internal static string GetAssemblyName(Assembly assembly)
	{
		return assembly.GetName().Name;
	}

	[ReflectionPermission(SecurityAction.Assert, Unrestricted = true)]
	[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
	internal static object CreateInstance(Assembly assembly, string objectType)
	{
		return assembly.CreateInstance(objectType, ignoreCase: false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, CultureInfo.InvariantCulture, null);
	}

	[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
	internal static bool CallerIsMicrosoftAssembly(Assembly currentAssembly)
	{
		if (currentAssembly == null)
		{
			return false;
		}
		StackTrace stackTrace = new StackTrace();
		bool flag = false;
		StackFrame[] frames = stackTrace.GetFrames();
		foreach (StackFrame stackFrame in frames)
		{
			flag = false;
			Assembly assembly = stackFrame.GetMethod().Module.Assembly;
			IEnumerator assemblyEnumerator = assembly.Evidence.GetAssemblyEnumerator();
			while (assemblyEnumerator.MoveNext())
			{
				if (assemblyEnumerator.Current is StrongName strongName && strongName.PublicKey.Equals(new StrongNamePublicKeyBlob(msPublicKey)))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				IEnumerator hostEnumerator = assembly.Evidence.GetHostEnumerator();
				while (hostEnumerator.MoveNext())
				{
					if (hostEnumerator.Current is StrongName strongName2 && strongName2.PublicKey.Equals(new StrongNamePublicKeyBlob(msPublicKey)))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
	internal static void SerializeWithSoapFormatter(MemoryStream memoryStream, Exception pfe)
	{
		new SoapFormatter().Serialize(memoryStream, pfe);
	}

	[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
	internal static void EnterMonitor(object lockObject)
	{
		Monitor.Enter(lockObject);
	}

	[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
	internal static void ExitMonitor(object lockObject)
	{
		Monitor.Exit(lockObject);
	}

	[ReflectionPermission(SecurityAction.Assert, Unrestricted = true)]
	[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
	internal static TypeConverter GetTypeConverter(Type t)
	{
		return TypeDescriptor.GetConverter(t);
	}
}
