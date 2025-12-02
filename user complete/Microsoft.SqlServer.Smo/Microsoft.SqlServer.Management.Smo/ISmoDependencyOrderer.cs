using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal interface ISmoDependencyOrderer
{
	Server Server { get; set; }

	List<Urn> Order(IEnumerable<Urn> urns);
}
