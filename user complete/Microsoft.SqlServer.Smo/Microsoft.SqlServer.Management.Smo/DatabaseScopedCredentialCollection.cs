using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabaseScopedCredentialCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public DatabaseScopedCredential this[int index] => GetObjectByIndex(index) as DatabaseScopedCredential;

	public DatabaseScopedCredential this[string name] => GetObjectByName(name) as DatabaseScopedCredential;

	internal DatabaseScopedCredentialCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(DatabaseScopedCredential[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public DatabaseScopedCredential ItemById(int id)
	{
		return (DatabaseScopedCredential)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(DatabaseScopedCredential);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new DatabaseScopedCredential(this, key, state);
	}

	public void Add(DatabaseScopedCredential databaseScopedCredential)
	{
		AddImpl(databaseScopedCredential);
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
