namespace Microsoft.SqlServer.Management.Smo;

internal class PropertyDispatcher : PropertyStorageBase
{
	private IPropertyDataDispatch m_dispatch;

	internal PropertyDispatcher(int count, IPropertyDataDispatch dispatch)
		: base(count)
	{
		m_dispatch = dispatch;
	}

	internal override object GetValue(int index)
	{
		if (GetBit(index, BitIndex.Uninitialized))
		{
			return null;
		}
		return m_dispatch.GetPropertyValue(index);
	}

	internal override void SetValue(int index, object value)
	{
		if (value == null)
		{
			SetBit(index, BitIndex.Uninitialized, value: true);
			return;
		}
		m_dispatch.SetPropertyValue(index, value);
		SetBit(index, BitIndex.Uninitialized, value: false);
	}
}
