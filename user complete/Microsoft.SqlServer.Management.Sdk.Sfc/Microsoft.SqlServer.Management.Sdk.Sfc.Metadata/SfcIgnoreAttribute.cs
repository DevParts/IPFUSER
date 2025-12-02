using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class SfcIgnoreAttribute : SfcRelationshipAttribute
{
	public SfcIgnoreAttribute()
		: base(SfcRelationship.Ignore)
	{
	}
}
