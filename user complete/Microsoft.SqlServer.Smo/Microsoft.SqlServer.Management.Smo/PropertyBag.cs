namespace Microsoft.SqlServer.Management.Smo;

internal class PropertyBag : PropertyStorageBase
{
	private object[] m_propertyValues;

	internal PropertyBag(int count)
		: base(count)
	{
		m_propertyValues = new object[count];
	}

	internal override object GetValue(int index)
	{
		return m_propertyValues[index];
	}

	internal override void SetValue(int index, object value)
	{
		m_propertyValues[index] = value;
	}
}
