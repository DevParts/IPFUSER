using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabaseRoleCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public DatabaseRole this[int index] => GetObjectByIndex(index) as DatabaseRole;

	public DatabaseRole this[string name] => GetObjectByName(name) as DatabaseRole;

	internal DatabaseRoleCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(DatabaseRole[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public DatabaseRole ItemById(int id)
	{
		return (DatabaseRole)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(DatabaseRole);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new DatabaseRole(this, key, state);
	}

	public void Add(DatabaseRole databaseRole)
	{
		AddImpl(databaseRole);
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
