using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class PhysicalFacetAttribute : Attribute
{
	private bool isReadOnly;

	public bool IsReadOnly => isReadOnly;

	public PhysicalFacetAttribute()
	{
		isReadOnly = false;
	}

	public PhysicalFacetAttribute(PhysicalFacetOptions options)
	{
		isReadOnly = 0 != (options & PhysicalFacetOptions.ReadOnly);
	}
}
