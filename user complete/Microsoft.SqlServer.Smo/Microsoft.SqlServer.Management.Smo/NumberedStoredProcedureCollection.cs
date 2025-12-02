using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class NumberedStoredProcedureCollection : NumberedObjectCollectionBase
{
	public StoredProcedure Parent => base.ParentInstance as StoredProcedure;

	public NumberedStoredProcedure this[int index] => GetObjectByIndex(index) as NumberedStoredProcedure;

	internal NumberedStoredProcedureCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(NumberedStoredProcedure);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new NumberedStoredProcedure(this, key, state);
	}

	public NumberedStoredProcedure GetProcedureByNumber(short number)
	{
		return GetObjectByKey(new NumberedObjectKey(number)) as NumberedStoredProcedure;
	}

	public void CopyTo(NumberedStoredProcedure[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}
}
