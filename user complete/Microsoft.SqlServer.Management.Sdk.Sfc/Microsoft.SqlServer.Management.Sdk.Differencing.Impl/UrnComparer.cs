using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.Impl;

internal class UrnComparer : IComparer<ISfcSimpleNode>
{
	int IComparer<ISfcSimpleNode>.Compare(ISfcSimpleNode x, ISfcSimpleNode y)
	{
		return CompareUtil.CompareUrnLeaves(x.Urn, y.Urn);
	}
}
