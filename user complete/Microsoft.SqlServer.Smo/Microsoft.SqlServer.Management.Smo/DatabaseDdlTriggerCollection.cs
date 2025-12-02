using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabaseDdlTriggerCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public DatabaseDdlTrigger this[int index] => GetObjectByIndex(index) as DatabaseDdlTrigger;

	public DatabaseDdlTrigger this[string name] => GetObjectByName(name) as DatabaseDdlTrigger;

	internal DatabaseDdlTriggerCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(DatabaseDdlTrigger[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public DatabaseDdlTrigger ItemById(int id)
	{
		return (DatabaseDdlTrigger)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(DatabaseDdlTrigger);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new DatabaseDdlTrigger(this, key, state);
	}

	public void Add(DatabaseDdlTrigger databaseDdlTrigger)
	{
		AddImpl(databaseDdlTrigger);
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
