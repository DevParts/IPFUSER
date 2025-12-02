using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.SPI;

public abstract class AvailablePropertyValueProvider : Provider
{
	public abstract bool IsGraphSupported(ISfcSimpleNode node);

	public abstract bool IsValueAvailable(ISfcSimpleNode node, string propName);
}
