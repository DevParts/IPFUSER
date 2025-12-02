using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SearchPropertyCollection : SimpleObjectCollectionBase
{
	internal class SimpleObjectCaseSensitiveComparer : IComparer
	{
		int IComparer.Compare(object obj1, object obj2)
		{
			return NetCoreHelpers.StringCompare((obj1 as SimpleObjectKey).Name, (obj2 as SimpleObjectKey).Name, ignoreCase: false, SmoApplication.DefaultCulture);
		}
	}

	public SearchPropertyList Parent => base.ParentInstance as SearchPropertyList;

	public SearchProperty this[int index] => GetObjectByIndex(index) as SearchProperty;

	public SearchProperty this[string name] => GetObjectByName(name) as SearchProperty;

	internal SearchPropertyCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(SearchProperty[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public SearchProperty ItemById(int id)
	{
		return (SearchProperty)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(SearchProperty);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new SearchProperty(this, key, state);
	}

	public void Add(SearchProperty searchProperty)
	{
		AddImpl(searchProperty);
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

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoSortedList(new SimpleObjectCaseSensitiveComparer());
	}
}
