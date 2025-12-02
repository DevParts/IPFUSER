using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcEventArgs : EventArgs
{
	private Urn urn;

	private SfcInstance instance;

	public Urn Urn => urn;

	public SfcInstance Instance => instance;

	public SfcEventArgs(Urn urn, SfcInstance instance)
	{
		this.urn = urn;
		this.instance = instance;
	}
}
