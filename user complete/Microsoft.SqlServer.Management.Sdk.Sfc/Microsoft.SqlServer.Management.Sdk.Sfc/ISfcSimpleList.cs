using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcSimpleList : IEnumerable<ISfcSimpleNode>, IEnumerable
{
	IEnumerable ListReference { get; }
}
