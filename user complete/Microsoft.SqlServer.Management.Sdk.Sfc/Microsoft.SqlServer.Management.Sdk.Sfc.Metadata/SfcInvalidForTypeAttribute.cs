using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
public class SfcInvalidForTypeAttribute : Attribute
{
	private Type excludedType;

	public Type ExcludedType => excludedType;

	public SfcInvalidForTypeAttribute(Type excludedType)
	{
		this.excludedType = excludedType;
	}
}
