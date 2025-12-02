using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcPropertyProvider : ISfcNotifyPropertyMetadataChanged, INotifyPropertyChanged
{
	ISfcPropertySet GetPropertySet();
}
