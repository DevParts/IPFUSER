using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ServerAdapter : ServerAdapterBase, IDmfAdapter, IServerAuditFacet, IDmfFacet
{
	public ServerAdapter(Server obj)
		: base(obj)
	{
	}
}
