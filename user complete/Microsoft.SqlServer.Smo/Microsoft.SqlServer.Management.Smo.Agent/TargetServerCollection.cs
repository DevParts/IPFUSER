using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class TargetServerCollection : SimpleObjectCollectionBase
{
	public JobServer Parent => base.ParentInstance as JobServer;

	public TargetServer this[int index] => GetObjectByIndex(index) as TargetServer;

	public TargetServer this[string name] => GetObjectByName(name) as TargetServer;

	internal TargetServerCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(TargetServer[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public TargetServer ItemById(int id)
	{
		return (TargetServer)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(TargetServer);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new TargetServer(this, key, state);
	}

	public void Add(TargetServer server)
	{
		AddImpl(server);
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
