using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public class DtsConnectionInfo : SqlOlapConnectionInfoBase
{
	protected StringBuilder applicationNameBuilder;

	protected StringBuilder workstationIdBuilder;

	[Browsable(false)]
	public override string ConnectionString
	{
		get
		{
			if (base.RebuildConnectionStringInternal)
			{
				base.ConnectionStringInternal = EncryptionUtility.EncryptString(string.Format(CultureInfo.InvariantCulture, "server={0};", new object[1] { base.ServerName }));
				base.RebuildConnectionStringInternal = false;
			}
			return EncryptionUtility.DecryptSecureString(base.ConnectionStringInternal);
		}
	}

	public string ApplicationName
	{
		get
		{
			if (applicationNameBuilder == null)
			{
				return string.Empty;
			}
			return applicationNameBuilder.ToString();
		}
		set
		{
			if (applicationNameBuilder == null || string.Compare(applicationNameBuilder.ToString(), value, StringComparison.Ordinal) != 0)
			{
				applicationNameBuilder = new StringBuilder(value);
				ConnectionParmsChanged();
			}
		}
	}

	public string WorkstationID
	{
		get
		{
			if (workstationIdBuilder == null)
			{
				return string.Empty;
			}
			return workstationIdBuilder.ToString();
		}
		set
		{
			if (workstationIdBuilder == null || string.Compare(workstationIdBuilder.ToString(), value, StringComparison.Ordinal) != 0)
			{
				workstationIdBuilder = new StringBuilder(value);
				ConnectionParmsChanged();
			}
		}
	}

	public DtsConnectionInfo()
		: base(ConnectionType.IntegrationServer)
	{
	}

	public DtsConnectionInfo(string serverName)
		: base(serverName, ConnectionType.IntegrationServer)
	{
	}

	public DtsConnectionInfo(DtsConnectionInfo conn)
		: base(conn)
	{
		applicationNameBuilder = conn.applicationNameBuilder;
		workstationIdBuilder = conn.workstationIdBuilder;
	}

	public override IDbConnection CreateConnectionObject()
	{
		Type type = Type.GetType("Microsoft.SqlServer.Dts.SmoEnum.DtsDbConnection, Microsoft.SqlServer.DTEnum, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91", throwOnError: true);
		IDbConnection dbConnection = (IDbConnection)Activator.CreateInstance(type);
		dbConnection.ConnectionString = ConnectionString;
		return dbConnection;
	}
}
