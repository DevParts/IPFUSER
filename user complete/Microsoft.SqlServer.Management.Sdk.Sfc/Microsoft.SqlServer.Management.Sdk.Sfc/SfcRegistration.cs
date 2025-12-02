using System;
using System.IO;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public sealed class SfcRegistration
{
	public static SfcDomainInfoCollection Domains => DomainRegistrationEncapsulation.Domains;

	private SfcRegistration()
	{
	}

	public static object CreateObject(string fullTypeName)
	{
		Type objectTypeFromFullName = GetObjectTypeFromFullName(fullTypeName);
		return Activator.CreateInstance(objectTypeFromFullName, nonPublic: true);
	}

	public static Type GetObjectTypeFromFullName(string fullTypeName)
	{
		return GetObjectTypeFromFullName(fullTypeName, ignoreCase: false);
	}

	public static Type GetObjectTypeFromFullName(string fullTypeName, bool ignoreCase)
	{
		Assembly assembly = LocateSfcExtension(GetRegisteredDomainForType(fullTypeName), throwOnUnregisteredDomain: true);
		return assembly.GetType(fullTypeName, throwOnError: true, ignoreCase);
	}

	public static Type TryGetObjectTypeFromFullName(string fullTypeName)
	{
		Type type = Type.GetType(fullTypeName);
		if (type != null)
		{
			return type;
		}
		Assembly assembly = LocateSfcExtension(GetRegisteredDomainForType(fullTypeName, throwOnUnregisteredDomain: false), throwOnUnregisteredDomain: false);
		if (assembly == null)
		{
			return null;
		}
		return assembly.GetType(fullTypeName, throwOnError: true, ignoreCase: false);
	}

	private static Assembly LocateSfcExtension(string domainName)
	{
		return LocateSfcExtension(domainName, throwOnUnregisteredDomain: true);
	}

	private static Assembly LocateSfcExtension(string domainName, bool throwOnUnregisteredDomain)
	{
		Assembly assembly = null;
		foreach (SfcDomainInfo domain in Domains)
		{
			if (string.Compare(domain.Name.ToUpperInvariant(), domainName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				assembly = (domain.IsAssemblyInGAC ? SmoManagementUtil.LoadAssembly(domain.AssemblyStrongName) : SmoManagementUtil.LoadAssemblyFromFile(new FileInfo(domain.AssemblyStrongName).FullName));
			}
		}
		if (assembly == null && throwOnUnregisteredDomain)
		{
			throw new SfcUnregisteredXmlDomainException(SfcStrings.UnregisteredXmlSfcDomain(domainName));
		}
		return assembly;
	}

	public static string GetRegisteredDomainForType(string fullTypeName)
	{
		return GetRegisteredDomainForType(fullTypeName, throwOnUnregisteredDomain: true);
	}

	public static string GetRegisteredDomainForType(string fullTypeName, bool throwOnUnregisteredDomain)
	{
		foreach (SfcDomainInfo domain in Domains)
		{
			if (fullTypeName.Contains(domain.DomainNamespace))
			{
				return domain.Name.ToUpperInvariant();
			}
		}
		if (throwOnUnregisteredDomain)
		{
			throw new SfcUnregisteredXmlTypeException(SfcStrings.UnregisteredSfcXmlType(string.Empty, fullTypeName), null);
		}
		return null;
	}

	public static SfcDomainInfo GetRegisteredDomainForType(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		return Domains[GetRegisteredDomainForType(type.FullName)];
	}

	internal static string GetFullAssemblyName(string assemblyName)
	{
		if (assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || assemblyName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
		{
			assemblyName = Path.GetFileNameWithoutExtension(assemblyName);
		}
		Assembly executingAssembly = SmoManagementUtil.GetExecutingAssembly();
		string fullName = executingAssembly.FullName;
		return fullName.Replace(SmoManagementUtil.GetAssemblyName(executingAssembly), assemblyName);
	}

	internal static void AddTestDomainsToDomainsList()
	{
		DomainRegistrationEncapsulation.AddTestDomainsToDomainsList();
	}
}
