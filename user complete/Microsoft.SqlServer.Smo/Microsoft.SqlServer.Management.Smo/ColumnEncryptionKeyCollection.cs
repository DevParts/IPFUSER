using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ColumnEncryptionKeyCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public ColumnEncryptionKey this[int index] => GetObjectByIndex(index) as ColumnEncryptionKey;

	public ColumnEncryptionKey this[string name] => GetObjectByName(name) as ColumnEncryptionKey;

	internal ColumnEncryptionKeyCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ColumnEncryptionKey[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ColumnEncryptionKey ItemById(int id)
	{
		return (ColumnEncryptionKey)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ColumnEncryptionKey);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ColumnEncryptionKey(this, key, state);
	}

	public void Add(ColumnEncryptionKey columnEncryptionKey)
	{
		AddImpl(columnEncryptionKey);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		return GetObjectByKey(new SimpleObjectKey(name));
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("Name");
		if (attribute == null || attribute.Length == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("Name", urn.Type));
		}
		return new SimpleObjectKey(attribute);
	}
}
