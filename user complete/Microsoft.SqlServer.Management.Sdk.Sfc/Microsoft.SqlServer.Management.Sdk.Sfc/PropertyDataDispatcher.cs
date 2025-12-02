namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal sealed class PropertyDataDispatcher
{
	private SfcInstance m_owner;

	internal PropertyDataDispatcher(SfcInstance owner)
	{
		m_owner = owner;
	}

	internal SfcInstance GetParent()
	{
		return m_owner;
	}

	internal object GetPropertyValue(string propertyName)
	{
		return m_owner.GetPropertyValueImpl(propertyName);
	}

	internal void SetPropertyValue(string propertyName, object value)
	{
		m_owner.SetPropertyValueImpl(propertyName, value);
	}

	internal void InitializeState()
	{
		m_owner.InitializeUIPropertyState();
	}
}
