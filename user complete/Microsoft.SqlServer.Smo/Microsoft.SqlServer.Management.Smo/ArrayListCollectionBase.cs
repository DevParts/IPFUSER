using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class ArrayListCollectionBase : SmoCollectionBase
{
	internal ArrayList InternalList => ((SmoArrayList)base.InternalStorage).innerCollection;

	internal ArrayListCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	private void FixIDs(int startIdx)
	{
		int num = startIdx;
		for (int i = startIdx; i < InternalList.Count; i++)
		{
			Property property = ((SqlSmoObject)InternalList[i]).Properties.Get("ID");
			if (!property.Retrieved || Convert.ToInt32(property.Value, SmoApplication.DefaultCulture) != 0)
			{
				property.SetRetrieved(retrieved: true);
				if (property.Type.Equals(typeof(short)))
				{
					property.SetValue((short)(++num));
				}
				else if (property.Type.Equals(typeof(byte)))
				{
					property.SetValue((byte)(++num));
				}
				else
				{
					property.SetValue(++num);
				}
			}
		}
	}

	protected void AddImpl(SqlSmoObject obj, int insertAtPosition)
	{
		CheckCollectionLock();
		if (obj == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException());
		}
		if (obj.ParentColl == null)
		{
			obj.SetParentImpl(base.ParentInstance);
		}
		obj.CheckPendingState();
		ValidateParentObject(obj);
		InternalList.Insert(insertAtPosition, obj);
		obj.objectInSpace = false;
		obj.key.Writable = true;
		if (!base.AcceptDuplicateNames)
		{
			FixIDs(insertAtPosition);
		}
	}

	internal void AddImpl(SqlSmoObject obj, ObjectKeyBase insertAtKey)
	{
		CheckCollectionLock();
		if (obj == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException());
		}
		int num = base.InternalStorage.LookUp(insertAtKey);
		if (-1 == num)
		{
			throw new SmoException(ExceptionTemplatesImpl.ColumnBeforeNotExisting(insertAtKey.ToString()));
		}
		AddImpl(obj, num);
	}

	internal void AddImpl(SqlSmoObject obj)
	{
		try
		{
			if (obj == null)
			{
				throw new ArgumentNullException();
			}
			if (!(obj is Column))
			{
				int num = base.InternalStorage.LookUp(obj.key);
				if (-1 != num)
				{
					throw new SmoException(ExceptionTemplatesImpl.CannotAddObject(obj.GetType().Name, obj.ToString()));
				}
			}
			AddImpl(obj, base.InternalStorage.Count);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, ex);
		}
	}
}
