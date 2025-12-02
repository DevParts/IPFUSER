using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class SmoCollectionBase : AbstractCollectionBase, ICollection, IEnumerable
{
	private SmoInternalStorage internalStorage;

	private string m_lockReason;

	private bool acceptDuplicateNames;

	internal SmoInternalStorage InternalStorage
	{
		get
		{
			if (internalStorage == null)
			{
				InitInnerCollection();
			}
			return internalStorage;
		}
		set
		{
			internalStorage = value;
		}
	}

	internal bool IsCollectionLocked => null != m_lockReason;

	public bool IsSynchronized => InternalStorage.IsSynchronized;

	public object SyncRoot => InternalStorage.SyncRoot;

	public int Count
	{
		get
		{
			if (!initialized && base.ParentInstance.State == SqlSmoState.Existing)
			{
				InitializeChildCollection();
			}
			return InternalStorage.Count;
		}
	}

	internal override int NoFaultCount => InternalStorage.Count;

	internal bool AcceptDuplicateNames
	{
		get
		{
			return acceptDuplicateNames;
		}
		set
		{
			acceptDuplicateNames = value;
		}
	}

	internal SmoCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected abstract void InitInnerCollection();

	protected virtual Type GetCollectionElementType()
	{
		return null;
	}

	internal void LockCollection(string lockReason)
	{
		m_lockReason = lockReason;
	}

	internal void UnlockCollection()
	{
		m_lockReason = null;
	}

	internal void CheckCollectionLock()
	{
		if (base.ParentInstance.IsDesignMode || m_lockReason == null)
		{
			return;
		}
		throw new FailedOperationException(ExceptionTemplatesImpl.CollectionCannotBeModified + m_lockReason).SetHelpContext("CollectionCannotBeModified");
	}

	internal virtual SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return null;
	}

	internal override void ImplRemove(ObjectKeyBase key)
	{
		InternalStorage.Remove(key);
	}

	internal void Remove(ObjectKeyBase key)
	{
		RemoveObj(InternalStorage[key], key);
	}

	internal void RemoveObj(SqlSmoObject obj, ObjectKeyBase key)
	{
		CheckCollectionLock();
		if (obj != null)
		{
			if (obj.State == SqlSmoState.Creating || (obj is Column && obj.ParentColl.ParentInstance is View))
			{
				InternalStorage.Remove(key);
				obj.objectInSpace = true;
				return;
			}
			throw new InvalidSmoOperationException("Remove", obj.State);
		}
		if (!key.IsNull)
		{
			throw new MissingObjectException(ExceptionTemplatesImpl.ObjectDoesNotExist(GetCollectionElementType().Name, key.ToString()));
		}
		throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException());
	}

	internal SqlSmoObject GetNewObject(ObjectKeyBase key)
	{
		key.Validate(GetCollectionElementType());
		bool flag = base.ParentInstance == null || !base.ParentInstance.IsObjectInSpace();
		if ((InternalStorage.Contains(key) || (flag && InitializeChildObject(key) != null)) && !AcceptDuplicateNames)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotAddObject(GetCollectionElementType().ToString(), key.ToString()));
		}
		return GetCollectionElementInstance(key, SqlSmoState.Creating);
	}

	protected SqlSmoObject GetObjectByIndex(int index)
	{
		if (!initialized && base.ParentInstance.State == SqlSmoState.Existing)
		{
			InitializeChildCollection();
		}
		return InternalStorage.GetByIndex(index);
	}

	internal virtual SqlSmoObject GetObjectByKey(ObjectKeyBase key)
	{
		object obj = InternalStorage[key];
		bool flag = base.ParentInstance == null || !base.ParentInstance.IsObjectInSpace();
		if (obj == null && flag && !initialized && base.ParentInstance.State == SqlSmoState.Existing)
		{
			obj = InitializeChildObject(key);
		}
		return obj as SqlSmoObject;
	}

	public void ClearAndInitialize(string filterQuery, IEnumerable<string> extraFields)
	{
		InternalStorage.Clear();
		InitializeChildCollection(refresh: false, null, filterQuery, extraFields);
	}

	public void ResetCollection()
	{
		InternalStorage.Clear();
		UnlockCollection();
	}

	private void InitializeChildCollection(bool refresh, ScriptingPreferences sp)
	{
		InitializeChildCollection(refresh, sp, null, null);
	}

	protected void InitializeChildCollection()
	{
		InitializeChildCollection(refresh: false);
	}

	protected void InitializeChildCollection(bool refresh)
	{
		InitializeChildCollection(refresh, null);
	}

	private void InitializeChildCollection(bool refresh, ScriptingPreferences sp, string filterQuery, IEnumerable<string> extraFields)
	{
		if (base.ParentInstance.IsDesignMode)
		{
			initialized = true;
			return;
		}
		SmoInternalStorage smoInternalStorage = InternalStorage;
		InitInnerCollection();
		string text = null;
		text = ((!(GetCollectionElementType().GetBaseType() == typeof(Parameter))) ? (GetCollectionElementType().InvokeMember("UrnSuffix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string) : (GetCollectionElementType().GetBaseType().GetBaseType().InvokeMember("UrnSuffix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string));
		text += (string.IsNullOrEmpty(filterQuery) ? string.Empty : filterQuery);
		base.ParentInstance.InitChildLevel(text, sp ?? new ScriptingPreferences(), sp != null, extraFields);
		foreach (SqlSmoObject item in smoInternalStorage)
		{
			ObjectKeyBase key = item.key.Clone();
			SqlSmoObject sqlSmoObject2 = InternalStorage[key];
			if (sqlSmoObject2 != null)
			{
				InternalStorage[key] = item;
			}
			else if (item.State == SqlSmoState.Creating)
			{
				if (!refresh)
				{
					InternalStorage[key] = item;
				}
			}
			else
			{
				item.SetState(SqlSmoState.Dropped);
			}
		}
		initialized = true;
	}

	internal object InitializeChildObject(ObjectKeyBase key)
	{
		if (base.ParentInstance.IsDesignMode)
		{
			return null;
		}
		SqlSmoObject collectionElementInstance = GetCollectionElementInstance(key, SqlSmoState.Creating);
		if (collectionElementInstance.Initialize())
		{
			collectionElementInstance.SetState(SqlSmoState.Existing);
			AddExisting(collectionElementInstance);
			return collectionElementInstance;
		}
		return null;
	}

	internal bool Contains(ObjectKeyBase key)
	{
		return null != GetObjectByKey(key);
	}

	internal bool ContainsKey(ObjectKeyBase key)
	{
		return null != InternalStorage[key];
	}

	public void Refresh()
	{
		Refresh(refreshChildObjects: false);
	}

	public void Refresh(bool refreshChildObjects)
	{
		InitializeChildCollection(refresh: true);
		if (!refreshChildObjects)
		{
			return;
		}
		IEnumerator enumerator = GetEnumerator();
		if (enumerator != null)
		{
			while (enumerator.MoveNext())
			{
				((SqlSmoObject)enumerator.Current).Refresh();
			}
		}
	}

	internal void Clear()
	{
		MarkAllDropped();
		InternalStorage.Clear();
	}

	protected SqlSmoObject GetItemById(int id)
	{
		return GetItemById(id, "ID");
	}

	protected SqlSmoObject GetItemById(int id, string idPropName)
	{
		IEnumerator enumerator = ((IEnumerable)this).GetEnumerator();
		while (enumerator.MoveNext())
		{
			SqlSmoObject sqlSmoObject = (SqlSmoObject)enumerator.Current;
			Property property = sqlSmoObject.Properties.Get(idPropName);
			if (property.Value == null && sqlSmoObject.State != SqlSmoState.Creating)
			{
				sqlSmoObject.Initialize(allProperties: true);
			}
			if (property.Value != null && id == Convert.ToInt32(property.Value, SmoApplication.DefaultCulture))
			{
				return sqlSmoObject;
			}
		}
		return null;
	}

	internal void MarkAllDropped()
	{
		IEnumerator enumerator = GetEnumerator();
		while (enumerator != null && enumerator.MoveNext())
		{
			((SqlSmoObject)enumerator.Current).MarkDroppedInternal();
		}
	}

	internal IEnumerator GetEnumerator(ScriptingPreferences sp)
	{
		if (base.ParentInstance.State == SqlSmoState.Dropped)
		{
			return null;
		}
		if (!initialized && base.ParentInstance.State == SqlSmoState.Existing)
		{
			InitializeChildCollection(refresh: false, sp);
		}
		return InternalStorage.GetEnumerator();
	}

	public virtual IEnumerator GetEnumerator()
	{
		return GetEnumerator(null);
	}

	internal override SqlSmoObject NoFaultLookup(ObjectKeyBase key)
	{
		return InternalStorage[key];
	}

	protected override void ImplAddExisting(SqlSmoObject obj)
	{
		InternalStorage.Add(obj.key, obj);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		int num = index;
		foreach (SqlSmoObject item in InternalStorage)
		{
			array.SetValue(item, num++);
		}
	}

	internal bool CanHaveEmptyName(Urn urn)
	{
		if (urn.Value.IndexOf("/JobServer", StringComparison.Ordinal) > 0)
		{
			return true;
		}
		if (urn.Type == "Login" && urn.Parent.Type == "LinkedServer")
		{
			return true;
		}
		return false;
	}

	protected void ValidateParentObject(SqlSmoObject obj)
	{
		if (obj.ParentColl != this)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.WrongParent(obj.ToString()));
		}
	}
}
