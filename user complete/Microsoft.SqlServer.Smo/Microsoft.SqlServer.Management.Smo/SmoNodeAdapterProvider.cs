using System;
using Microsoft.SqlServer.Management.Sdk.Differencing.SPI;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SmoNodeAdapterProvider : SfcNodeAdapterProvider
{
	public override bool IsGraphSupported(object obj)
	{
		if (obj is SqlSmoObject)
		{
			return true;
		}
		return false;
	}

	public override ISfcSimpleNode GetGraphAdapter(object obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		return SfcSimpleNodeFactory.Factory.GetSimpleNode(obj, new SmoSimpleNodeAdapter());
	}
}
