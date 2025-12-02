using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class DependencyComparer : IComparer
{
	private Server server;

	internal DependencyComparer(Server srv)
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
		Dependency dependency = (Dependency)x;
		Dependency dependency2 = (Dependency)y;
		return server.CompareUrn(dependency.Urn, dependency2.Urn);
	}
}
