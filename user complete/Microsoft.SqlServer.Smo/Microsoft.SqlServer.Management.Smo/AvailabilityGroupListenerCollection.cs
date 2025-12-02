using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class AvailabilityGroupListenerCollection : SimpleObjectCollectionBase
{
	public AvailabilityGroup Parent => base.ParentInstance as AvailabilityGroup;

	public AvailabilityGroupListener this[int index] => GetObjectByIndex(index) as AvailabilityGroupListener;

	public AvailabilityGroupListener this[string name] => GetObjectByName(name) as AvailabilityGroupListener;

	internal AvailabilityGroupListenerCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(AvailabilityGroupListener[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public AvailabilityGroupListener ItemById(int id)
	{
		return (AvailabilityGroupListener)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(AvailabilityGroupListener);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new AvailabilityGroupListener(this, key, state);
	}

	public void Add(AvailabilityGroupListener AvailabilityGroupListener)
	{
		AddImpl(AvailabilityGroupListener);
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
