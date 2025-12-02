namespace Microsoft.SqlServer.Management.Sdk.Differencing;

public interface IPair<T>
{
	T Source { get; }

	T Target { get; }
}
