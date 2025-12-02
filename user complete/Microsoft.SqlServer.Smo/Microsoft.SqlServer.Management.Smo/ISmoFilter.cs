using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal interface ISmoFilter
{
	Server Server { get; set; }

	IEnumerable<Urn> Filter(IEnumerable<Urn> urns);
}
