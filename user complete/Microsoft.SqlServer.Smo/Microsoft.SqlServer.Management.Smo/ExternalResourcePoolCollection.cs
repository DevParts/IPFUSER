using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ExternalResourcePoolCollection : SimpleObjectCollectionBase
{
	public ResourceGovernor Parent => base.ParentInstance as ResourceGovernor;

	public ExternalResourcePool this[int index] => GetObjectByIndex(index) as ExternalResourcePool;

	public ExternalResourcePool this[string name] => GetObjectByName(name) as ExternalResourcePool;

	internal ExternalResourcePoolCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ExternalResourcePool[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ExternalResourcePool ItemById(int id)
	{
		return (ExternalResourcePool)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ExternalResourcePool);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ExternalResourcePool(this, key, state);
	}

	public void Add(ExternalResourcePool externalResourcePool)
	{
		AddImpl(externalResourcePool);
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
