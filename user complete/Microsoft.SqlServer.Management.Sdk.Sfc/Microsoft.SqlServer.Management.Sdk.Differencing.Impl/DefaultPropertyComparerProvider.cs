using Microsoft.SqlServer.Management.Sdk.Differencing.SPI;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.Impl;

internal class DefaultPropertyComparerProvider : PropertyComparerProvider
{
	public override bool AreGraphsSupported(ISfcSimpleNode left, ISfcSimpleNode right)
	{
		return true;
	}

	public override bool Compare(ISfcSimpleNode left, ISfcSimpleNode right, string propName)
	{
		object left2 = left.Properties[propName];
		object right2 = right.Properties[propName];
		return CompareUtil.CompareObjects(left2, right2);
	}
}
