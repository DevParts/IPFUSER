using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class PartitionFunctionCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public PartitionFunction this[int index] => GetObjectByIndex(index) as PartitionFunction;

	public PartitionFunction this[string name] => GetObjectByName(name) as PartitionFunction;

	internal PartitionFunctionCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(PartitionFunction[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public PartitionFunction ItemById(int id)
	{
		return (PartitionFunction)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(PartitionFunction);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new PartitionFunction(this, key, state);
	}

	public void Add(PartitionFunction partitionFunction)
	{
		AddImpl(partitionFunction);
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
