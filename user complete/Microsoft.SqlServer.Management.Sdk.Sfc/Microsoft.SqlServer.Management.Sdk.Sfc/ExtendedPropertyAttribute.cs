using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[AttributeUsage(AttributeTargets.Property)]
public class ExtendedPropertyAttribute : Attribute
{
	private string parentPropertyName;

	public string ParentPropertyName => parentPropertyName;

	public bool HasParent => !string.IsNullOrEmpty(parentPropertyName);

	public ExtendedPropertyAttribute()
	{
	}

	public ExtendedPropertyAttribute(string parentPropertyName)
	{
		this.parentPropertyName = parentPropertyName;
	}
}
