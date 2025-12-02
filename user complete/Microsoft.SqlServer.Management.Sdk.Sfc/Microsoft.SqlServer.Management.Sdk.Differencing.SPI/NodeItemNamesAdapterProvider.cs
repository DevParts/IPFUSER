using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.SPI;

public abstract class NodeItemNamesAdapterProvider : Provider
{
	public abstract bool IsGraphSupported(ISfcSimpleNode node);

	public abstract IEnumerable<string> GetRelatedContainerNames(ISfcSimpleNode node);

	public abstract IEnumerable<string> GetRelatedObjectNames(ISfcSimpleNode node);

	public abstract IEnumerable<string> GetPropertyNames(ISfcSimpleNode node);

	public abstract bool IsContainerInNatrualOrder(ISfcSimpleNode node, string name);
}
