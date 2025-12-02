using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class TargetServerGroupCollection : SimpleObjectCollectionBase
{
	public JobServer Parent => base.ParentInstance as JobServer;

	public TargetServerGroup this[int index] => GetObjectByIndex(index) as TargetServerGroup;

	public TargetServerGroup this[string name] => GetObjectByName(name) as TargetServerGroup;

	internal TargetServerGroupCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(TargetServerGroup[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public TargetServerGroup ItemById(int id)
	{
		return (TargetServerGroup)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(TargetServerGroup);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new TargetServerGroup(this, key, state);
	}

	public void Add(TargetServerGroup targetServerGroup)
	{
		AddImpl(targetServerGroup);
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
