using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo.Broker;

public sealed class ServiceQueueCollection : SchemaCollectionBase
{
	public ServiceBroker Parent => base.ParentInstance as ServiceBroker;

	public ServiceQueue this[int index] => GetObjectByIndex(index) as ServiceQueue;

	public ServiceQueue this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as ServiceQueue;
		}
	}

	public ServiceQueue this[string name, string schema]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			if (schema == null)
			{
				throw new ArgumentNullException("schema cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as ServiceQueue;
		}
	}

	internal ServiceQueueCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ServiceQueue[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ServiceQueue ItemById(int id)
	{
		return (ServiceQueue)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ServiceQueue);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ServiceQueue(this, key, state);
	}

	public void Add(ServiceQueue serviceQueue)
	{
		if (serviceQueue == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("serviceQueue"));
		}
		AddImpl(serviceQueue);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("schema cannot be null");
		}
		return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema()));
	}
}
