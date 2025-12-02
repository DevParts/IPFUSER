using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo.RegisteredServers;

[Obsolete("Instead use namespace Microsoft.SqlServer.Management.RegisteredServers")]
public abstract class RegSvrCollectionBase
{
	private RegSvrSmoObject parentInstance;

	protected internal SortedList innerColl;

	protected internal bool initialized;

	public bool IsSynchronized => innerColl.IsSynchronized;

	public object SyncRoot => innerColl.SyncRoot;

	internal RegSvrSmoObject ParentInstance => parentInstance;

	internal RegSvrCollectionBase(RegSvrSmoObject parentInstance)
	{
		this.parentInstance = parentInstance;
		initialized = false;
		innerColl = new SortedList(System.StringComparer.Ordinal);
	}

	internal void AddInternal(RegSvrSmoObject regSvrObj)
	{
		innerColl.Add(regSvrObj.Name, regSvrObj);
	}

	internal void RemoveInternal(string key)
	{
		innerColl.Remove(key);
	}
}
