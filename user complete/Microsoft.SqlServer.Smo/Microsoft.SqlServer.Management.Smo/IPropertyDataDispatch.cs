namespace Microsoft.SqlServer.Management.Smo;

internal interface IPropertyDataDispatch
{
	object GetPropertyValue(int index);

	void SetPropertyValue(int index, object value);
}
