using System;
using System.Collections;
using Microsoft.SqlServer.Management.Smo.RegisteredServers;

namespace Microsoft.SqlServer.Management.Smo;

[Obsolete("Instead use namespace Microsoft.SqlServer.Management.RegisteredServers")]
public class SqlServerRegistrations
{
	private static volatile ServerType m_ServerTypeSingleton;

	public static ServerGroupCollection ServerGroups => Instance.ServerGroups;

	public static RegisteredServerCollection RegisteredServers => Instance.RegisteredServers;

	private static ServerType Instance
	{
		get
		{
			if (m_ServerTypeSingleton == null)
			{
				lock (typeof(SqlServerRegistrations))
				{
					if (m_ServerTypeSingleton == null)
					{
						m_ServerTypeSingleton = new ServerType("<root>");
					}
				}
			}
			return m_ServerTypeSingleton;
		}
	}

	public static RegisteredServer[] EnumRegisteredServers()
	{
		ArrayList arrayList = new ArrayList();
		foreach (RegisteredServer registeredServer in RegisteredServers)
		{
			arrayList.Add(registeredServer);
		}
		GetRegisteredServers(arrayList, ServerGroups);
		RegisteredServer[] array = new RegisteredServer[arrayList.Count];
		arrayList.CopyTo(array);
		return array;
	}

	public static void Refresh()
	{
		Instance.Refresh();
	}

	private static void GetRegisteredServers(ArrayList rs, ServerGroupCollection sgs)
	{
		foreach (ServerGroup sg in sgs)
		{
			foreach (RegisteredServer registeredServer in sg.RegisteredServers)
			{
				rs.Add(registeredServer);
			}
			GetRegisteredServers(rs, sg.ServerGroups);
		}
	}
}
