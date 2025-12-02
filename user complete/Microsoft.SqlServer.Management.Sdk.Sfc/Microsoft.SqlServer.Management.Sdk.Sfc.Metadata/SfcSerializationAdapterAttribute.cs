using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class SfcSerializationAdapterAttribute : Attribute
{
	private Type sfcSerializationAdapterType;

	public Type SfcSerializationAdapterType => sfcSerializationAdapterType;

	public SfcSerializationAdapterAttribute(Type adapterType)
	{
		if (adapterType == null)
		{
			throw new ArgumentNullException("adapterType");
		}
		sfcSerializationAdapterType = adapterType;
	}
}
