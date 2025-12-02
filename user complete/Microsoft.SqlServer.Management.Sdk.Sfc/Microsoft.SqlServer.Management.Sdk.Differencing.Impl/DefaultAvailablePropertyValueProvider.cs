using Microsoft.SqlServer.Management.Sdk.Differencing.SPI;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.Impl;

internal class DefaultAvailablePropertyValueProvider : AvailablePropertyValueProvider
{
	public override bool IsGraphSupported(ISfcSimpleNode source)
	{
		return true;
	}

	public override bool IsValueAvailable(ISfcSimpleNode source, string propName)
	{
		return true;
	}
}
