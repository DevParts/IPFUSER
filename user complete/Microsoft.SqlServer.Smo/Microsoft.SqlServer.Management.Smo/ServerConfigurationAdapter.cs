using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ServerConfigurationAdapter : ServerAdapterBase, IDmfAdapter, IServerConfigurationFacet, IServerPerformanceFacet, IDmfFacet
{
	public ServerConfigurationAdapter(Server obj)
		: base(obj)
	{
	}

	public override void Refresh()
	{
		base.Server.Configuration.Refresh();
	}

	public override void Alter()
	{
		base.Server.Configuration.Alter(overrideValueChecking: true);
	}
}
