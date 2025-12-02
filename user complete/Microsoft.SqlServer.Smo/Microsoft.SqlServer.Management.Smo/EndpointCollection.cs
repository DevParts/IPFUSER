using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class EndpointCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public Endpoint this[int index] => GetObjectByIndex(index) as Endpoint;

	public Endpoint this[string name] => GetObjectByName(name) as Endpoint;

	internal EndpointCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Endpoint[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Endpoint ItemById(int id)
	{
		return (Endpoint)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Endpoint);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Endpoint(this, key, state);
	}

	public void Add(Endpoint endpoint)
	{
		AddImpl(endpoint);
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

	public Endpoint[] EnumEndpoints(EndpointType endpointType)
	{
		ArrayList arrayList = new ArrayList();
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Endpoint endpoint = (Endpoint)enumerator.Current;
				if (endpointType == endpoint.EndpointType)
				{
					arrayList.Add(endpoint);
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		Endpoint[] array = new Endpoint[arrayList.Count];
		arrayList.CopyTo(array);
		return array;
	}
}
