using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class UserDefinedFunctionParameterCollection : ParameterCollectionBase
{
	public UserDefinedFunction Parent => base.ParentInstance as UserDefinedFunction;

	public UserDefinedFunctionParameter this[int index] => GetObjectByIndex(index) as UserDefinedFunctionParameter;

	public UserDefinedFunctionParameter this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as UserDefinedFunctionParameter;

	internal UserDefinedFunctionParameterCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(UserDefinedFunctionParameter[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(UserDefinedFunctionParameter);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new UserDefinedFunctionParameter(this, key, state);
	}

	public void Add(UserDefinedFunctionParameter userDefinedFunctionParameter)
	{
		AddImpl(userDefinedFunctionParameter);
	}

	public void Add(UserDefinedFunctionParameter userDefinedFunctionParameter, string insertAtColumnName)
	{
		AddImpl(userDefinedFunctionParameter, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(UserDefinedFunctionParameter userDefinedFunctionParameter, int insertAtPosition)
	{
		AddImpl(userDefinedFunctionParameter, insertAtPosition);
	}

	public void Remove(UserDefinedFunctionParameter userDefinedFunctionParameter)
	{
		if (userDefinedFunctionParameter == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("userDefinedFunctionParameter"));
		}
		RemoveObj(userDefinedFunctionParameter, userDefinedFunctionParameter.key);
	}

	public UserDefinedFunctionParameter ItemById(int id)
	{
		return (UserDefinedFunctionParameter)GetItemById(id);
	}
}
