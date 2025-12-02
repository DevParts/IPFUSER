namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class SfcDefaultStorage : ISfcPropertyStorageProvider
{
	private object[] propertiesStorage;

	private SfcInstance sfcObject;

	internal SfcDefaultStorage(SfcInstance sfcObject)
	{
		this.sfcObject = sfcObject;
	}

	public object GetPropertyValueImpl(string propertyName)
	{
		if (propertiesStorage == null)
		{
			BuildPropertiesStorage();
		}
		sfcObject.Properties.SetRetrieved(propertyName, val: true);
		return propertiesStorage[sfcObject.Properties.LookupID(propertyName)];
	}

	public void SetPropertyValueImpl(string propertyName, object value)
	{
		if (propertiesStorage == null)
		{
			BuildPropertiesStorage();
		}
		sfcObject.Properties.SetDirty(propertyName, val: true);
		propertiesStorage[sfcObject.Properties.LookupID(propertyName)] = value;
	}

	private void BuildPropertiesStorage()
	{
		propertiesStorage = new object[sfcObject.Metadata.InternalStorageSupportedCount];
	}
}
