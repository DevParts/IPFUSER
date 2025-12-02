namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcPropertyStorageProvider
{
	object GetPropertyValueImpl(string propertyName);

	void SetPropertyValueImpl(string propertyName, object value);
}
