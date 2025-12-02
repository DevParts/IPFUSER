using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Differencing.SPI;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.Impl;

internal class DefaultContainerSortingProvider : ContainerSortingProvider
{
	private static readonly IComparer<ISfcSimpleNode> URN_COMPARER = new UrnComparer();

	public override bool AreGraphsSupported(ISfcSimpleNode source, ISfcSimpleNode target)
	{
		return true;
	}

	public override IComparer<ISfcSimpleNode> GetComparer(ISfcSimpleList list, ISfcSimpleList list2)
	{
		return URN_COMPARER;
	}
}
