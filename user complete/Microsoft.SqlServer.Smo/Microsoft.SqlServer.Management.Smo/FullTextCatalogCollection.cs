using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class FullTextCatalogCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public FullTextCatalog this[int index] => GetObjectByIndex(index) as FullTextCatalog;

	public FullTextCatalog this[string name] => GetObjectByName(name) as FullTextCatalog;

	internal FullTextCatalogCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(FullTextCatalog[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public FullTextCatalog ItemById(int id)
	{
		return (FullTextCatalog)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(FullTextCatalog);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new FullTextCatalog(this, key, state);
	}

	public void Add(FullTextCatalog fullTextCatalog)
	{
		AddImpl(fullTextCatalog);
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
