using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class PartitionSchemeCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public PartitionScheme this[int index] => GetObjectByIndex(index) as PartitionScheme;

	public PartitionScheme this[string name] => GetObjectByName(name) as PartitionScheme;

	internal PartitionSchemeCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(PartitionScheme[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public PartitionScheme ItemById(int id)
	{
		return (PartitionScheme)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(PartitionScheme);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new PartitionScheme(this, key, state);
	}

	public void Add(PartitionScheme partitionScheme)
	{
		AddImpl(partitionScheme);
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
