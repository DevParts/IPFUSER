using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabaseAuditSpecificationCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public DatabaseAuditSpecification this[int index] => GetObjectByIndex(index) as DatabaseAuditSpecification;

	public DatabaseAuditSpecification this[string name] => GetObjectByName(name) as DatabaseAuditSpecification;

	internal DatabaseAuditSpecificationCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(DatabaseAuditSpecification[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public DatabaseAuditSpecification ItemById(int id)
	{
		return (DatabaseAuditSpecification)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(DatabaseAuditSpecification);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new DatabaseAuditSpecification(this, key, state);
	}

	public void Add(DatabaseAuditSpecification databaseAuditSpecification)
	{
		AddImpl(databaseAuditSpecification);
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
