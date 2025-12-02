namespace Microsoft.SqlServer.Management.Smo;

internal abstract class PropertyStorageBase : BitStorage
{
	internal PropertyStorageBase(int count)
		: base(count)
	{
	}

	internal bool IsNull(int index)
	{
		return GetValue(index) == null;
	}

	internal bool IsDirty(int index)
	{
		return GetBit(index, BitIndex.Dirty);
	}

	internal void SetDirty(int index, bool val)
	{
		SetBit(index, BitIndex.Dirty, val);
	}

	internal bool IsRetrieved(int index)
	{
		return GetBit(index, BitIndex.Retrieved);
	}

	internal void SetRetrieved(int index, bool val)
	{
		SetBit(index, BitIndex.Retrieved, val);
	}

	internal bool IsEnabled(int index)
	{
		return GetBit(index, BitIndex.Enabled);
	}

	internal void SetEnabled(int index, bool val)
	{
		SetBit(index, BitIndex.Enabled, val);
	}

	internal abstract object GetValue(int index);

	internal abstract void SetValue(int index, object value);
}
