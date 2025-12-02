namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcDiscoverObject
{
	void Discover(ISfcDependencyDiscoveryObjectSink sink);
}
