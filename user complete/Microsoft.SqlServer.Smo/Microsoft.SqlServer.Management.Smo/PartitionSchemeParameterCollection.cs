using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class PartitionSchemeParameterCollection : ParameterCollectionBase
{
	public SqlSmoObject Parent => base.ParentInstance;

	public PartitionSchemeParameter this[int index] => GetObjectByIndex(index) as PartitionSchemeParameter;

	public PartitionSchemeParameter this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as PartitionSchemeParameter;

	internal PartitionSchemeParameterCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(PartitionSchemeParameter[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(PartitionSchemeParameter);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new PartitionSchemeParameter(this, key, state);
	}

	public void Add(PartitionSchemeParameter partitionSchemeParameter)
	{
		AddImpl(partitionSchemeParameter);
	}

	public void Add(PartitionSchemeParameter partitionSchemeParameter, string insertAtColumnName)
	{
		AddImpl(partitionSchemeParameter, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(PartitionSchemeParameter partitionSchemeParameter, int insertAtPosition)
	{
		AddImpl(partitionSchemeParameter, insertAtPosition);
	}

	public void Remove(PartitionSchemeParameter partitionSchemeParameter)
	{
		if (partitionSchemeParameter == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("partitionSchemeParameter"));
		}
		RemoveObj(partitionSchemeParameter, partitionSchemeParameter.key);
	}

	public PartitionSchemeParameter ItemById(int id)
	{
		return (PartitionSchemeParameter)GetItemById(id);
	}
}
