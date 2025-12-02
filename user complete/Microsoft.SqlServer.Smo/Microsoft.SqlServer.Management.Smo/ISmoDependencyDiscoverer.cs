using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public interface ISmoDependencyDiscoverer
{
	Server Server { get; set; }

	IEnumerable<Urn> Discover(IEnumerable<Urn> urns);
}
