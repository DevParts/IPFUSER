using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcNotifyPropertyMetadataChanged
{
	event EventHandler<SfcPropertyMetadataChangedEventArgs> PropertyMetadataChanged;
}
