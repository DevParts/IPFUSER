using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ColumnEncryptionKeyValueCollection : ColumnEncryptionKeyValueCollectionBase
{
	public ColumnEncryptionKey Parent => base.ParentInstance as ColumnEncryptionKey;

	public ColumnEncryptionKeyValue this[int index] => GetObjectByIndex(index) as ColumnEncryptionKeyValue;

	internal ColumnEncryptionKeyValueCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public ColumnEncryptionKeyValue GetItemByColumnMasterKeyID(int ColumnMasterKeyID)
	{
		return base.InternalStorage[new ColumnEncryptionKeyValueObjectKey(ColumnMasterKeyID)] as ColumnEncryptionKeyValue;
	}

	public void Add(ColumnEncryptionKeyValue columnEncryptionKeyValue)
	{
		base.InternalStorage.Add(new ColumnEncryptionKeyValueObjectKey(columnEncryptionKeyValue.ColumnMasterKeyID), columnEncryptionKeyValue);
	}

	public void CopyTo(ColumnEncryptionKeyValue[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ColumnEncryptionKeyValue);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ColumnEncryptionKeyValue(this, key, state);
	}
}
