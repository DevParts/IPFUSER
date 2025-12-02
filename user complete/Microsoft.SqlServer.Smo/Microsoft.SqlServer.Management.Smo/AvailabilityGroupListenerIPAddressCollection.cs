using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class AvailabilityGroupListenerIPAddressCollection : AvailabilityGroupListenerIPAddressCollectionBase
{
	public AvailabilityGroupListener Parent => base.ParentInstance as AvailabilityGroupListener;

	public AvailabilityGroupListenerIPAddress this[int index] => GetObjectByIndex(index) as AvailabilityGroupListenerIPAddress;

	public AvailabilityGroupListenerIPAddress this[string ipAddress, string subnetMask, string subnetIP] => GetObjectByKey(new AvailabilityGroupListenerIPAddressObjectKey(ipAddress, subnetMask, subnetIP)) as AvailabilityGroupListenerIPAddress;

	internal AvailabilityGroupListenerIPAddressCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(AvailabilityGroupListenerIPAddress[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public AvailabilityGroupListenerIPAddress ItemById(int id)
	{
		return (AvailabilityGroupListenerIPAddress)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(AvailabilityGroupListenerIPAddress);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new AvailabilityGroupListenerIPAddress(this, key, state);
	}

	public void Add(AvailabilityGroupListenerIPAddress AvailabilityGroupListenerIPAddress)
	{
		if (AvailabilityGroupListenerIPAddress == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("AvailabilityGroupListenerIPAddress"));
		}
		AddImpl(AvailabilityGroupListenerIPAddress);
	}
}
