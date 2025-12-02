using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class FilterPropertyEventArgs : EventArgs
{
	private string propertyName;

	private SfcInstance instance;

	public string PropertyName => propertyName;

	public SfcInstance Instance => instance;

	public FilterPropertyEventArgs(SfcInstance instance, string propertyName)
	{
		this.instance = instance;
		this.propertyName = propertyName;
	}
}
