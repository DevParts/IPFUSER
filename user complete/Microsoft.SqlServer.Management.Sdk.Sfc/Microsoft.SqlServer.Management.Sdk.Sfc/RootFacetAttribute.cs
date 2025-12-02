using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class RootFacetAttribute : Attribute
{
	private Type rootType;

	public Type RootType => rootType;

	public RootFacetAttribute(Type rootType)
	{
		this.rootType = rootType;
	}
}
