using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing;

public interface IDiffEntry
{
	DiffType ChangeType { get; }

	Urn Source { get; }

	Urn Target { get; }

	IDictionary<string, IPair<object>> Properties { get; }
}
