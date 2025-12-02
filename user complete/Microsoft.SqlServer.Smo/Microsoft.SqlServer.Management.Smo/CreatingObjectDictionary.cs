using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class CreatingObjectDictionary
{
	private Dictionary<Urn, SqlSmoObject> objectsStored;

	private Server server;

	public CreatingObjectDictionary(Server server)
	{
		if (server == null)
		{
			throw new SmoException("server", new ArgumentNullException("server"));
		}
		this.server = server;
		objectsStored = new Dictionary<Urn, SqlSmoObject>();
	}

	public void Add(SqlSmoObject obj)
	{
		if ((obj.State == SqlSmoState.Creating || obj.IsDesignMode) && !objectsStored.ContainsKey(obj.Urn))
		{
			objectsStored.Add(obj.Urn, obj);
		}
	}

	public SqlSmoObject SmoObjectFromUrn(Urn urn)
	{
		if (objectsStored.ContainsKey(urn))
		{
			return objectsStored[urn];
		}
		return server.GetSmoObject(urn);
	}

	public bool ContainsKey(Urn urn)
	{
		return objectsStored.ContainsKey(urn);
	}
}
