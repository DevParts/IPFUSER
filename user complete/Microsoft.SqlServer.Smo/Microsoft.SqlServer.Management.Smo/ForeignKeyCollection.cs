using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ForeignKeyCollection : SimpleObjectCollectionBase
{
	public Table Parent => base.ParentInstance as Table;

	public ForeignKey this[int index] => GetObjectByIndex(index) as ForeignKey;

	public ForeignKey this[string name] => GetObjectByName(name) as ForeignKey;

	internal ForeignKeyCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ForeignKey[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ForeignKey ItemById(int id)
	{
		return (ForeignKey)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ForeignKey);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ForeignKey(this, key, state);
	}

	public void Remove(ForeignKey foreignKey)
	{
		if (foreignKey == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("foreignKey"));
		}
		RemoveObj(foreignKey, new SimpleObjectKey(foreignKey.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(ForeignKey foreignKey)
	{
		AddImpl(foreignKey);
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
