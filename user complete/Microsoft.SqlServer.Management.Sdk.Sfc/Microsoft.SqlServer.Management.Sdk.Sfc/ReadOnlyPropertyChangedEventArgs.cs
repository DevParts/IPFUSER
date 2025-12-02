using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class ReadOnlyPropertyChangedEventArgs : EventArgs
{
	private string propertyName;

	public string PropertyName
	{
		get
		{
			return propertyName;
		}
		set
		{
			propertyName = value;
		}
	}

	public ReadOnlyPropertyChangedEventArgs(string propertyName)
	{
		this.propertyName = propertyName;
	}
}
