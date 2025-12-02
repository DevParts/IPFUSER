using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.SPI;

public abstract class SfcNodeAdapterProvider : Provider
{
	public abstract bool IsGraphSupported(object obj);

	public abstract ISfcSimpleNode GetGraphAdapter(object obj);
}
