using System;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class SortedListCollectionBase : SmoCollectionBase
{
	internal SortedListCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected void AddImpl(SqlSmoObject obj)
	{
		if (obj == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException());
		}
		CheckCollectionLock();
		SqlSmoObject objectByKey = GetObjectByKey(obj.key);
		if (objectByKey != null)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotAddObject(obj.GetType().Name, obj.key.ToString()));
		}
		if (obj.ParentColl == null)
		{
			obj.SetParentImpl(base.ParentInstance);
		}
		obj.CheckPendingState();
		ValidateParentObject(obj);
		ImplAddExisting(obj);
	}

	protected override void ImplAddExisting(SqlSmoObject obj)
	{
		base.InternalStorage.Add(obj.key, obj);
		obj.objectInSpace = false;
		obj.ParentColl = this;
	}
}
