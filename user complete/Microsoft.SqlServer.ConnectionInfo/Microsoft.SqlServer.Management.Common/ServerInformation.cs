using System;
using System.Data;
using System.Text;

namespace Microsoft.SqlServer.Management.Common;

internal class ServerInformation
{
	private const string serverVersionQuery = "DECLARE @edition sysname;\r\nSET @edition = cast(SERVERPROPERTY(N'EDITION') as sysname);\r\nSELECT case when @edition = N'SQL Azure' then 2 else 1 end as 'DatabaseEngineType',\r\nSERVERPROPERTY('EngineEdition') AS DatabaseEngineEdition,\r\nSERVERPROPERTY('ProductVersion') AS ProductVersion,\r\n@@MICROSOFTVERSION AS MicrosoftVersion;\r\n";

	private readonly ServerVersion serverVersion;

	private readonly Version productVersion;

	private readonly DatabaseEngineType databaseEngineType;

	private readonly DatabaseEngineEdition databaseEngineEdition;

	private readonly string hostPlatform;

	private readonly NetworkProtocol connectionProtocol;

	public string HostPlatform => hostPlatform;

	public ServerVersion ServerVersion => serverVersion;

	public Version ProductVersion => productVersion;

	public DatabaseEngineType DatabaseEngineType => databaseEngineType;

	public DatabaseEngineEdition DatabaseEngineEdition => databaseEngineEdition;

	public NetworkProtocol ConnectionProtocol => connectionProtocol;

	public ServerInformation(ServerVersion sv, Version productVersion, DatabaseEngineType dt, DatabaseEngineEdition databaseEngineEdition)
		: this(sv, productVersion, dt, databaseEngineEdition, "Windows", NetworkProtocol.NotSpecified)
	{
	}

	public ServerInformation(ServerVersion sv, Version productVersion, DatabaseEngineType dt, DatabaseEngineEdition databaseEngineEdition, string hostPlatform, NetworkProtocol connectionProtocol)
	{
		serverVersion = sv;
		this.productVersion = productVersion;
		databaseEngineType = dt;
		this.databaseEngineEdition = databaseEngineEdition;
		this.hostPlatform = hostPlatform;
		this.connectionProtocol = connectionProtocol;
	}

	public static ServerInformation GetServerInformation(IDbConnection sqlConnection, IDbDataAdapter dataAdapter, string serverVersionString)
	{
		ServerVersion serverVersion = ParseStringServerVersion(serverVersionString);
		StringBuilder stringBuilder = new StringBuilder("DECLARE @edition sysname;\r\nSET @edition = cast(SERVERPROPERTY(N'EDITION') as sysname);\r\nSELECT case when @edition = N'SQL Azure' then 2 else 1 end as 'DatabaseEngineType',\r\nSERVERPROPERTY('EngineEdition') AS DatabaseEngineEdition,\r\nSERVERPROPERTY('ProductVersion') AS ProductVersion,\r\n@@MICROSOFTVERSION AS MicrosoftVersion;\r\n");
		if (serverVersion.Major >= 14)
		{
			stringBuilder.AppendLine("select host_platform from sys.dm_os_host_info");
		}
		else
		{
			stringBuilder.AppendLine("select N'Windows' as host_platform");
		}
		if (serverVersion.Major >= 10)
		{
			stringBuilder.AppendLine("if @edition = N'SQL Azure' \r\n  select 'TCP' as ConnectionProtocol\r\nelse\r\n  exec ('select CONVERT(nvarchar(40),CONNECTIONPROPERTY(''net_transport'')) as ConnectionProtocol')");
		}
		else
		{
			stringBuilder.AppendLine("select NULL as ConnectionProtocol");
		}
		using IDbCommand dbCommand = sqlConnection.CreateCommand();
		dbCommand.CommandText = stringBuilder.ToString();
		DataSet dataSet = new DataSet();
		dataAdapter.SelectCommand = dbCommand;
		dataAdapter.Fill(dataSet);
		DatabaseEngineType dt = (DatabaseEngineType)dataSet.Tables[0].Rows[0]["DatabaseEngineType"];
		DatabaseEngineEdition databaseEngineEdition = (DatabaseEngineEdition)dataSet.Tables[0].Rows[0]["DatabaseEngineEdition"];
		if (databaseEngineEdition == DatabaseEngineEdition.SqlManagedInstance)
		{
			dt = DatabaseEngineType.Standalone;
			serverVersion = ParseMicrosoftVersion(Convert.ToUInt32(dataSet.Tables[0].Rows[0]["MicrosoftVersion"].ToString()));
		}
		object obj = dataSet.Tables[2].Rows[0]["ConnectionProtocol"];
		return new ServerInformation(serverVersion, new Version((string)dataSet.Tables[0].Rows[0]["ProductVersion"]), dt, databaseEngineEdition, (string)dataSet.Tables[1].Rows[0]["host_platform"], (obj == DBNull.Value) ? NetworkProtocol.NotSpecified : ProtocolFromNetTransport((string)obj));
	}

	private static NetworkProtocol ProtocolFromNetTransport(string netTransport)
	{
		switch (netTransport.ToLowerInvariant())
		{
		case "tcp":
		case "http":
		case "ssl":
			return NetworkProtocol.TcpIp;
		case "named pipe":
			return NetworkProtocol.NamedPipes;
		case "shared memory":
			return NetworkProtocol.SharedMemory;
		case "via":
			return NetworkProtocol.Via;
		default:
			return NetworkProtocol.NotSpecified;
		}
	}

	public static ServerVersion ParseStringServerVersion(string version)
	{
		Version version2 = new Version(version.Substring(0, 10));
		return new ServerVersion(version2.Major, version2.Minor, version2.Build);
	}

	public static ServerVersion ParseMicrosoftVersion(uint version)
	{
		return new ServerVersion((int)(version / 16777216), (int)((version / 65536) & 0xF), (int)(version & 0xFF));
	}
}
