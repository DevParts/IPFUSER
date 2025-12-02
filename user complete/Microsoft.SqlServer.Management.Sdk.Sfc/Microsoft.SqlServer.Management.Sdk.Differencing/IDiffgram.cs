using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Differencing;

public interface IDiffgram : IEnumerable<IDiffEntry>, IEnumerable
{
	object SourceRoot { get; }

	object TargetRoot { get; }
}
