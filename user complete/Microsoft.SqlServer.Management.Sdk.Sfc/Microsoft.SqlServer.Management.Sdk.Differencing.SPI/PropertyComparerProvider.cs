using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.SPI;

public abstract class PropertyComparerProvider : Provider
{
	public abstract bool AreGraphsSupported(ISfcSimpleNode source, ISfcSimpleNode target);

	public abstract bool Compare(ISfcSimpleNode source, ISfcSimpleNode target, string propName);
}
