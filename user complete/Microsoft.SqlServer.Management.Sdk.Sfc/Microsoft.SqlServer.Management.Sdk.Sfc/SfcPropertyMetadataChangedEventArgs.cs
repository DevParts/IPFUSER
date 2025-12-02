using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcPropertyMetadataChangedEventArgs : PropertyChangedEventArgs
{
	public SfcPropertyMetadataChangedEventArgs(string propertyName)
		: base(propertyName)
	{
	}
}
