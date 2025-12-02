using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class AvailabilityDatabaseCollection : SimpleObjectCollectionBase
{
	public AvailabilityGroup Parent => base.ParentInstance as AvailabilityGroup;

	public AvailabilityDatabase this[int index] => GetObjectByIndex(index) as AvailabilityDatabase;

	public AvailabilityDatabase this[string name] => GetObjectByName(name) as AvailabilityDatabase;

	internal AvailabilityDatabaseCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(AvailabilityDatabase[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public AvailabilityDatabase ItemById(int id)
	{
		return (AvailabilityDatabase)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(AvailabilityDatabase);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new AvailabilityDatabase(this, key, state);
	}

	public void Remove(AvailabilityDatabase AvailabilityDatabase)
	{
		if (AvailabilityDatabase == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("AvailabilityDatabase"));
		}
		RemoveObj(AvailabilityDatabase, new SimpleObjectKey(AvailabilityDatabase.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(AvailabilityDatabase AvailabilityDatabase)
	{
		AddImpl(AvailabilityDatabase);
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
