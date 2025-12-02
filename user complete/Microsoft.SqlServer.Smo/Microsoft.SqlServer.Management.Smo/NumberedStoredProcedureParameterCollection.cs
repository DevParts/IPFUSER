using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class NumberedStoredProcedureParameterCollection : ParameterCollectionBase
{
	public NumberedStoredProcedure Parent => base.ParentInstance as NumberedStoredProcedure;

	public NumberedStoredProcedureParameter this[int index] => GetObjectByIndex(index) as NumberedStoredProcedureParameter;

	public NumberedStoredProcedureParameter this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as NumberedStoredProcedureParameter;

	internal NumberedStoredProcedureParameterCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(NumberedStoredProcedureParameter[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(NumberedStoredProcedureParameter);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new NumberedStoredProcedureParameter(this, key, state);
	}

	public void Add(NumberedStoredProcedureParameter numberedStoredProcedureParameter)
	{
		AddImpl(numberedStoredProcedureParameter);
	}

	public void Add(NumberedStoredProcedureParameter numberedStoredProcedureParameter, string insertAtColumnName)
	{
		AddImpl(numberedStoredProcedureParameter, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(NumberedStoredProcedureParameter numberedStoredProcedureParameter, int insertAtPosition)
	{
		AddImpl(numberedStoredProcedureParameter, insertAtPosition);
	}

	public void Remove(NumberedStoredProcedureParameter numberedStoredProcedureParameter)
	{
		if (numberedStoredProcedureParameter == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("numberedStoredProcedureParameter"));
		}
		RemoveObj(numberedStoredProcedureParameter, numberedStoredProcedureParameter.key);
	}

	public NumberedStoredProcedureParameter ItemById(int id)
	{
		return (NumberedStoredProcedureParameter)GetItemById(id);
	}
}
