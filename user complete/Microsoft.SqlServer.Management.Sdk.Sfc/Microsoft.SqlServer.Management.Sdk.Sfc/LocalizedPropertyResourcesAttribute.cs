using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
public class LocalizedPropertyResourcesAttribute : Attribute
{
	private string resourcesName;

	private bool useDefaultKeys;

	public string ResourcesName => resourcesName;

	public bool UseDefaultKeys => useDefaultKeys;

	public LocalizedPropertyResourcesAttribute(string resourcesName)
	{
		if (string.IsNullOrEmpty(resourcesName))
		{
			throw new ArgumentNullException("resourcesName");
		}
		TraceHelper.Assert(resourcesName != null, "unexpected null resourcesName parameter");
		TraceHelper.Assert(0 < resourcesName.Length, "unexpected empty resourcesName");
		this.resourcesName = resourcesName;
		useDefaultKeys = false;
	}

	public LocalizedPropertyResourcesAttribute(string resourcesName, bool useDefaultKeys)
	{
		TraceHelper.Assert(resourcesName != null, "unexpected null resourcesName parameter");
		TraceHelper.Assert(0 < resourcesName.Length, "unexpected empty resourcesName");
		this.resourcesName = resourcesName;
		this.useDefaultKeys = useDefaultKeys;
	}

	public LocalizedPropertyResourcesAttribute(Type resourceType)
	{
		if (resourceType == null)
		{
			throw new ArgumentNullException("resourceType");
		}
		TraceHelper.Assert(resourceType != null, "unexpected null resourceType parameter");
		resourcesName = resourceType.FullName;
	}
}
