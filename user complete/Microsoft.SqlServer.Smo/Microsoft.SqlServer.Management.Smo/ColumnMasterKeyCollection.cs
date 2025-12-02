using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ColumnMasterKeyCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public ColumnMasterKey this[int index] => GetObjectByIndex(index) as ColumnMasterKey;

	public ColumnMasterKey this[string name] => GetObjectByName(name) as ColumnMasterKey;

	internal ColumnMasterKeyCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ColumnMasterKey[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ColumnMasterKey ItemById(int id)
	{
		return (ColumnMasterKey)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ColumnMasterKey);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ColumnMasterKey(this, key, state);
	}

	public void Add(ColumnMasterKey columnMasterKey)
	{
		AddImpl(columnMasterKey);
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
