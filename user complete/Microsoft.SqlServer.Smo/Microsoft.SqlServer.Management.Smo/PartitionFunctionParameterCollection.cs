using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class PartitionFunctionParameterCollection : ParameterCollectionBase
{
	public PartitionFunction Parent => base.ParentInstance as PartitionFunction;

	public PartitionFunctionParameter this[int index] => GetObjectByIndex(index) as PartitionFunctionParameter;

	internal PartitionFunctionParameterCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(PartitionFunctionParameter[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(PartitionFunctionParameter);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new PartitionFunctionParameter(this, key, state);
	}

	public void Add(PartitionFunctionParameter partitionFunctionParameter)
	{
		AddImpl(partitionFunctionParameter);
	}

	public void Add(PartitionFunctionParameter partitionFunctionParameter, string insertAtColumnName)
	{
		AddImpl(partitionFunctionParameter, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(PartitionFunctionParameter partitionFunctionParameter, int insertAtPosition)
	{
		AddImpl(partitionFunctionParameter, insertAtPosition);
	}

	public void Remove(PartitionFunctionParameter partitionFunctionParameter)
	{
		if (partitionFunctionParameter == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("partitionFunctionParameter"));
		}
		RemoveObj(partitionFunctionParameter, partitionFunctionParameter.key);
	}

	public PartitionFunctionParameter ItemById(int id)
	{
		return (PartitionFunctionParameter)GetItemById(id);
	}
}
