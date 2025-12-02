namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IEnumDependencies
{
	DependencyChainCollection EnumDependencies(object ci, DependencyRequest rd);
}
