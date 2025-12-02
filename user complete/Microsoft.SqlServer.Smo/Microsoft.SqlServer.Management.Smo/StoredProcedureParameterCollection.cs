using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class StoredProcedureParameterCollection : ParameterCollectionBase
{
	public StoredProcedure Parent => base.ParentInstance as StoredProcedure;

	public StoredProcedureParameter this[int index] => GetObjectByIndex(index) as StoredProcedureParameter;

	public StoredProcedureParameter this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as StoredProcedureParameter;

	internal StoredProcedureParameterCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(StoredProcedureParameter[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(StoredProcedureParameter);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new StoredProcedureParameter(this, key, state);
	}

	public void Add(StoredProcedureParameter storedProcedureParameter)
	{
		AddImpl(storedProcedureParameter);
	}

	public void Add(StoredProcedureParameter storedProcedureParameter, string insertAtColumnName)
	{
		AddImpl(storedProcedureParameter, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(StoredProcedureParameter storedProcedureParameter, int insertAtPosition)
	{
		AddImpl(storedProcedureParameter, insertAtPosition);
	}

	public void Remove(StoredProcedureParameter storedProcedureParameter)
	{
		if (storedProcedureParameter == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("storedProcedureParameter"));
		}
		RemoveObj(storedProcedureParameter, storedProcedureParameter.key);
	}

	public StoredProcedureParameter ItemById(int id)
	{
		return (StoredProcedureParameter)GetItemById(id);
	}
}
