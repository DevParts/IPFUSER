using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal interface IDatabasePrefetch
{
	CreatingObjectDictionary creatingDictionary { get; set; }

	IEnumerable<Urn> PrefetchObjects(IEnumerable<Urn> input);
}
