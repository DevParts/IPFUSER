using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

public sealed class ServiceContractMappingCollection : SimpleObjectCollectionBase
{
	public BrokerService Parent => base.ParentInstance as BrokerService;

	public ServiceContractMapping this[int index] => GetObjectByIndex(index) as ServiceContractMapping;

	public ServiceContractMapping this[string name] => GetObjectByName(name) as ServiceContractMapping;

	internal ServiceContractMappingCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ServiceContractMapping[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ServiceContractMapping);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ServiceContractMapping(this, key, state);
	}

	public void Remove(ServiceContractMapping serviceContractMapping)
	{
		if (serviceContractMapping == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("serviceContractMapping"));
		}
		RemoveObj(serviceContractMapping, new SimpleObjectKey(serviceContractMapping.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(ServiceContractMapping serviceContractMapping)
	{
		AddImpl(serviceContractMapping);
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
