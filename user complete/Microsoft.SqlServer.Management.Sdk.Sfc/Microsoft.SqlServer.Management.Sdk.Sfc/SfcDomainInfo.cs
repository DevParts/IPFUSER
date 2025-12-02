using System;
using System.IO;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public sealed class SfcDomainInfo
{
	private string name;

	private Type rootType;

	private string rootTypeFullName;

	private string namespaceQualifier;

	private string assemblyStrongName;

	private string domainNamespace;

	private string psDriveName;

	private bool assemblyInGAC = true;

	public string Name
	{
		get
		{
			return name;
		}
		internal set
		{
			name = value;
		}
	}

	public bool IsAssemblyInGAC => assemblyInGAC;

	public string DomainNamespace => domainNamespace;

	public Type RootType
	{
		get
		{
			if (rootType == null)
			{
				LateLoadRootType();
			}
			return rootType;
		}
		internal set
		{
			rootType = value;
		}
	}

	public string RootTypeFullName
	{
		get
		{
			return rootTypeFullName;
		}
		internal set
		{
			rootTypeFullName = value;
		}
	}

	public string AssemblyStrongName
	{
		get
		{
			return assemblyStrongName;
		}
		internal set
		{
			assemblyStrongName = value;
		}
	}

	public string NamespaceQualifier
	{
		get
		{
			return namespaceQualifier;
		}
		internal set
		{
			namespaceQualifier = value;
		}
	}

	public string PSDriveName
	{
		get
		{
			return psDriveName;
		}
		internal set
		{
			psDriveName = value;
		}
	}

	internal SfcDomainInfo(string namespaceQualifier, string rootTypeFullName, string assemblyName, string psDriveName)
		: this(namespaceQualifier, rootTypeFullName, assemblyName, psDriveName, assemblyInGAC: true)
	{
	}

	internal SfcDomainInfo(string namespaceQualifier, string rootTypeFullName, string assemblyName, string psDriveName, bool assemblyInGAC)
	{
		this.rootTypeFullName = rootTypeFullName;
		if (assemblyInGAC)
		{
			assemblyStrongName = SfcRegistration.GetFullAssemblyName(assemblyName);
		}
		else
		{
			assemblyStrongName = assemblyName;
			if (!assemblyStrongName.ToUpperInvariant().EndsWith(".DLL", StringComparison.Ordinal))
			{
				assemblyStrongName += ".dll";
			}
		}
		name = rootTypeFullName.Substring(rootTypeFullName.LastIndexOf(".", StringComparison.Ordinal) + 1);
		this.namespaceQualifier = namespaceQualifier;
		domainNamespace = rootTypeFullName.Substring(0, rootTypeFullName.LastIndexOf(".", StringComparison.Ordinal));
		this.psDriveName = psDriveName;
		this.assemblyInGAC = assemblyInGAC;
	}

	private void LateLoadRootType()
	{
		Assembly assembly = null;
		assembly = (IsAssemblyInGAC ? SmoManagementUtil.LoadAssembly(assemblyStrongName) : SmoManagementUtil.LoadAssemblyFromFile(new FileInfo(assemblyStrongName).FullName));
		rootType = assembly.GetType(rootTypeFullName);
	}

	public int GetLogicalVersion(object instance)
	{
		if (instance is SfcInstance)
		{
			SfcInstance sfcInstance = instance as SfcInstance;
			ISfcDomain domain = sfcInstance.KeyChain.Domain;
			return domain.GetLogicalVersion();
		}
		if (instance is IAlienObject)
		{
			return ((IAlienObject)instance).GetDomainRoot().GetLogicalVersion();
		}
		throw new SfcSerializationException(SfcStrings.DomainRootUnknown(instance.GetType().FullName));
	}
}
