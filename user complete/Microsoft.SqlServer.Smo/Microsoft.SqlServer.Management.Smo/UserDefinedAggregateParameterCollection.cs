using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class UserDefinedAggregateParameterCollection : ParameterCollectionBase
{
	public UserDefinedAggregate Parent => base.ParentInstance as UserDefinedAggregate;

	public UserDefinedAggregateParameter this[int index] => GetObjectByIndex(index) as UserDefinedAggregateParameter;

	public UserDefinedAggregateParameter this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as UserDefinedAggregateParameter;

	internal UserDefinedAggregateParameterCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(UserDefinedAggregateParameter[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(UserDefinedAggregateParameter);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new UserDefinedAggregateParameter(this, key, state);
	}

	public void Add(UserDefinedAggregateParameter userDefinedAggregateParameter)
	{
		AddImpl(userDefinedAggregateParameter);
	}

	public void Add(UserDefinedAggregateParameter userDefinedAggregateParameter, string insertAtColumnName)
	{
		AddImpl(userDefinedAggregateParameter, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(UserDefinedAggregateParameter userDefinedAggregateParameter, int insertAtPosition)
	{
		AddImpl(userDefinedAggregateParameter, insertAtPosition);
	}

	public void Remove(UserDefinedAggregateParameter userDefinedAggregateParameter)
	{
		if (userDefinedAggregateParameter == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("userDefinedAggregateParameter"));
		}
		RemoveObj(userDefinedAggregateParameter, userDefinedAggregateParameter.key);
	}

	public UserDefinedAggregateParameter ItemById(int id)
	{
		return (UserDefinedAggregateParameter)GetItemById(id);
	}
}
