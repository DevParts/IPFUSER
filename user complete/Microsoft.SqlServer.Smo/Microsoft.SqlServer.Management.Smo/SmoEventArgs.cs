using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class SmoEventArgs : EventArgs
{
	private Urn urn;

	public Urn Urn => urn;

	public SmoEventArgs(Urn urn)
	{
		this.urn = urn;
	}
}
