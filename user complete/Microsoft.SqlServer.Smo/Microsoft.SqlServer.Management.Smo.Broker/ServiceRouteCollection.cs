using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

public sealed class ServiceRouteCollection : SimpleObjectCollectionBase
{
	public ServiceBroker Parent => base.ParentInstance as ServiceBroker;

	public ServiceRoute this[int index] => GetObjectByIndex(index) as ServiceRoute;

	public ServiceRoute this[string name] => GetObjectByName(name) as ServiceRoute;

	internal ServiceRouteCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ServiceRoute[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ServiceRoute ItemById(int id)
	{
		return (ServiceRoute)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ServiceRoute);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ServiceRoute(this, key, state);
	}

	public void Add(ServiceRoute serviceRoute)
	{
		AddImpl(serviceRoute);
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
