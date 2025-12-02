using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DatabaseAdapter : DatabaseAdapterBase, IDmfAdapter, IDmfFacet
{
	public DatabaseAdapter(Database obj)
		: base(obj)
	{
	}
}
