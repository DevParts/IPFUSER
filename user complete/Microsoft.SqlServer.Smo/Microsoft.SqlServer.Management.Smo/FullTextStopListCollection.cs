using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class FullTextStopListCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public FullTextStopList this[int index] => GetObjectByIndex(index) as FullTextStopList;

	public FullTextStopList this[string name] => GetObjectByName(name) as FullTextStopList;

	internal FullTextStopListCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(FullTextStopList[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public FullTextStopList ItemById(int id)
	{
		return (FullTextStopList)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(FullTextStopList);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new FullTextStopList(this, key, state);
	}

	public void Add(FullTextStopList fullTextStopList)
	{
		AddImpl(fullTextStopList);
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
