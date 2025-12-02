using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class TriggerCollection : SimpleObjectCollectionBase
{
	public SqlSmoObject Parent => base.ParentInstance;

	public Trigger this[int index] => GetObjectByIndex(index) as Trigger;

	public Trigger this[string name] => GetObjectByName(name) as Trigger;

	internal TriggerCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Trigger[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Trigger ItemById(int id)
	{
		return (Trigger)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Trigger);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Trigger(this, key, state);
	}

	public void Remove(Trigger trigger)
	{
		if (trigger == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("trigger"));
		}
		RemoveObj(trigger, new SimpleObjectKey(trigger.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(Trigger trigger)
	{
		AddImpl(trigger);
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
