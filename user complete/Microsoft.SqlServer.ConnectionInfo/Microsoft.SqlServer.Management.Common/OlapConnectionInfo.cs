using System;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
[ComVisible(false)]
public class OlapConnectionInfo : SqlOlapConnectionInfoBase
{
	private bool shouldEncryptConnection;

	private string applicationName;

	private string otherParams;

	private string integratedSecurity;

	public bool EncryptConnection
	{
		get
		{
			return shouldEncryptConnection;
		}
		set
		{
			shouldEncryptConnection = value;
		}
	}

	public string ApplicationName
	{
		get
		{
			return applicationName;
		}
		set
		{
			applicationName = value;
		}
	}

	public string IntegratedSecurity
	{
		get
		{
			return integratedSecurity;
		}
		set
		{
			integratedSecurity = value;
		}
	}

	public string OtherParameters
	{
		get
		{
			return otherParams;
		}
		set
		{
			otherParams = value;
		}
	}

	[Browsable(false)]
	public override string ConnectionString
	{
		get
		{
			if (base.RebuildConnectionStringInternal)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Provider=MSOLAP;Data Source={0}", base.ServerName);
				if (base.DatabaseNameInternal != null && base.DatabaseNameInternal.Length > 0)
				{
					stringBuilder.AppendFormat(";Initial Catalog={0}", base.DatabaseNameInternal);
				}
				if (base.ConnectionTimeout != -1)
				{
					stringBuilder.AppendFormat(";Connect Timeout={0}", base.ConnectionTimeout);
				}
				if (base.QueryTimeout != -1)
				{
					stringBuilder.AppendFormat(";Timeout={0}", base.QueryTimeout);
				}
				if (!string.IsNullOrEmpty(base.UserName))
				{
					stringBuilder.AppendFormat(";User ID={0}", base.UserName);
				}
				if (!string.IsNullOrEmpty(base.Password))
				{
					stringBuilder.AppendFormat(";Password='{0}'", base.Password);
				}
				if (shouldEncryptConnection)
				{
					stringBuilder.AppendFormat(";Use Encryption for Data=true");
				}
				if (!string.IsNullOrEmpty(applicationName))
				{
					stringBuilder.AppendFormat(";Application Name={0}", applicationName);
				}
				if (!string.IsNullOrEmpty(integratedSecurity))
				{
					stringBuilder.AppendFormat(";Integrated Security={0}", integratedSecurity);
				}
				if (!string.IsNullOrEmpty(otherParams))
				{
					stringBuilder.AppendFormat(";{0}", otherParams);
				}
				base.ConnectionStringInternal = EncryptionUtility.EncryptString(stringBuilder.ToString());
				stringBuilder = null;
				base.RebuildConnectionStringInternal = false;
			}
			return EncryptionUtility.DecryptSecureString(base.ConnectionStringInternal);
		}
	}

	public OlapConnectionInfo()
		: base(ConnectionType.Olap)
	{
	}

	public OlapConnectionInfo(OlapConnectionInfo conn)
		: base(conn)
	{
		shouldEncryptConnection = conn.shouldEncryptConnection;
		applicationName = conn.applicationName;
	}

	public OlapConnectionInfo Copy()
	{
		return new OlapConnectionInfo(this);
	}

	public override IDbConnection CreateConnectionObject()
	{
		return new OleDbConnection(ConnectionString);
	}
}
