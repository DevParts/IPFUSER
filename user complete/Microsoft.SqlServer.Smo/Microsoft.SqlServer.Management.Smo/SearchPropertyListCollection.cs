using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SearchPropertyListCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public SearchPropertyList this[int index] => GetObjectByIndex(index) as SearchPropertyList;

	public SearchPropertyList this[string name] => GetObjectByName(name) as SearchPropertyList;

	internal SearchPropertyListCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(SearchPropertyList[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public SearchPropertyList ItemById(int id)
	{
		return (SearchPropertyList)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(SearchPropertyList);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new SearchPropertyList(this, key, state);
	}

	public void Add(SearchPropertyList searchPropertyList)
	{
		AddImpl(searchPropertyList);
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
