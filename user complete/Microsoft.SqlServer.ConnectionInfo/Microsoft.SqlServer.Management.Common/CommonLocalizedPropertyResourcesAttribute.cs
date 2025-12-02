using System;

namespace Microsoft.SqlServer.Management.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
public class CommonLocalizedPropertyResourcesAttribute : Attribute
{
	private string resourcesName;

	private bool useDefaultKeys;

	public string ResourcesName => resourcesName;

	public bool UseDefaultKeys => useDefaultKeys;

	public CommonLocalizedPropertyResourcesAttribute(string resourcesName)
	{
		if (string.IsNullOrEmpty(resourcesName))
		{
			throw new ArgumentNullException("resourcesName");
		}
		this.resourcesName = resourcesName;
		useDefaultKeys = false;
	}

	public CommonLocalizedPropertyResourcesAttribute(string resourcesName, bool useDefaultKeys)
	{
		this.resourcesName = resourcesName;
		this.useDefaultKeys = useDefaultKeys;
	}

	public CommonLocalizedPropertyResourcesAttribute(Type resourceType)
	{
		if (resourceType == null)
		{
			throw new ArgumentNullException("resourceType");
		}
		resourcesName = resourceType.FullName;
	}
}
