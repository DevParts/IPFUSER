using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class IndexCollection : SimpleObjectCollectionBase
{
	public SqlSmoObject Parent => base.ParentInstance;

	public Index this[int index] => GetObjectByIndex(index) as Index;

	public Index this[string name] => GetObjectByName(name) as Index;

	internal IndexCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Index[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Index ItemById(int id)
	{
		return (Index)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Index);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Index(this, key, state);
	}

	public void Remove(Index index)
	{
		if (index == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("index"));
		}
		RemoveObj(index, new SimpleObjectKey(index.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(Index index)
	{
		AddImpl(index);
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
