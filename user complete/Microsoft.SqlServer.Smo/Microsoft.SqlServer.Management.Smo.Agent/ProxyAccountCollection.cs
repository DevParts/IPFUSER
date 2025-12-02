using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class ProxyAccountCollection : SimpleObjectCollectionBase
{
	public JobServer Parent => base.ParentInstance as JobServer;

	public ProxyAccount this[int index] => GetObjectByIndex(index) as ProxyAccount;

	public ProxyAccount this[string name] => GetObjectByName(name) as ProxyAccount;

	internal ProxyAccountCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ProxyAccount[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ProxyAccount ItemById(int id)
	{
		return (ProxyAccount)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ProxyAccount);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ProxyAccount(this, key, state);
	}

	public void Add(ProxyAccount proxyAccount)
	{
		AddImpl(proxyAccount);
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
