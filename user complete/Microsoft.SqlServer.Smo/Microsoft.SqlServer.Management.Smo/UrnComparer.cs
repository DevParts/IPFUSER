using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class UrnComparer : IComparer
{
	private Server server;

	internal UrnComparer(Server srv)
	{
		server = srv;
	}

	public int Compare(object x, object y)
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x != null && y == null)
		{
			return 1;
		}
		if (x == null && y != null)
		{
			return -1;
		}
		return server.CompareUrn((Urn)x, (Urn)y);
	}
}
