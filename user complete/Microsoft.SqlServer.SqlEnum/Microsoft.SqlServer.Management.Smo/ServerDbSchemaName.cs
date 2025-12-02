using System;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

internal class ServerDbSchemaName : IComparable
{
	private string serverName;

	private string dbName;

	private string schemaName;

	private string name;

	private int id;

	private int type;

	private StringComparer svrComparer;

	private StringComparer dbComparer;

	public string ServerName
	{
		get
		{
			return serverName;
		}
		set
		{
			serverName = value;
		}
	}

	public string DatabaseName
	{
		get
		{
			return dbName;
		}
		set
		{
			dbName = value;
		}
	}

	public string SchemaName
	{
		get
		{
			return schemaName;
		}
		set
		{
			schemaName = value;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public int Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public int Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	internal ServerDbSchemaName(string serverName, string dbName, string schemaName, string name, int id, int type)
	{
		this.serverName = serverName;
		this.dbName = dbName;
		this.schemaName = schemaName;
		this.name = name;
		this.id = id;
		this.type = type;
	}

	internal ServerDbSchemaName(string serverName, string dbName, string schemaName, string name, int id, int type, StringComparer svrComparer, StringComparer dbComparer)
	{
		this.serverName = serverName;
		this.dbName = dbName;
		this.schemaName = schemaName;
		this.name = name;
		this.id = id;
		this.type = type;
		this.svrComparer = svrComparer;
		this.dbComparer = dbComparer;
	}

	public int CompareTo(object obj)
	{
		if (!(obj is ServerDbSchemaName serverDbSchemaName))
		{
			throw new InvalidArgumentException();
		}
		int num = ((svrComparer != null) ? svrComparer.Compare(serverDbSchemaName.serverName, serverName) : string.Compare(serverDbSchemaName.serverName, serverName, StringComparison.Ordinal));
		if (num != 0)
		{
			return num;
		}
		num = ((dbComparer != null) ? dbComparer.Compare(serverDbSchemaName.dbName, dbName) : string.Compare(serverDbSchemaName.dbName, dbName, StringComparison.Ordinal));
		if (num != 0)
		{
			return num;
		}
		if (serverDbSchemaName.type < type)
		{
			return -1;
		}
		if (serverDbSchemaName.type > type)
		{
			return 1;
		}
		if (serverDbSchemaName.id == id && id != 0)
		{
			return 0;
		}
		if (dbComparer == null)
		{
			num = string.Compare(serverDbSchemaName.schemaName, schemaName, StringComparison.Ordinal);
			if (num != 0)
			{
				return num;
			}
			num = string.Compare(serverDbSchemaName.name, name, StringComparison.Ordinal);
			if (num != 0)
			{
				return num;
			}
		}
		else
		{
			num = dbComparer.Compare(serverDbSchemaName.schemaName, schemaName);
			if (num != 0)
			{
				return num;
			}
			num = dbComparer.Compare(serverDbSchemaName.name, name);
			if (num != 0)
			{
				return num;
			}
		}
		return 0;
	}
}
