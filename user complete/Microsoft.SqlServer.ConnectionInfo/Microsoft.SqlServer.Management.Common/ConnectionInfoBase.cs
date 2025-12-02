using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
[ComVisible(false)]
public abstract class ConnectionInfoBase
{
	private ServerVersion m_sv;

	private ServerCaseSensitivity m_scs;

	private ConnectionType m_eServerType;

	private static readonly CultureInfo defaultCulture = CultureInfo.InvariantCulture;

	public ConnectionType ServerType => m_eServerType;

	public ServerVersion ServerVersion
	{
		get
		{
			return m_sv;
		}
		set
		{
			m_sv = value;
		}
	}

	public ServerCaseSensitivity ServerCaseSensitivity
	{
		get
		{
			return m_scs;
		}
		set
		{
			m_scs = value;
		}
	}

	internal static CultureInfo DefaultCulture => defaultCulture;

	protected ConnectionInfoBase()
	{
		throw new InvalidOperationException(StringConnectionInfo.ClassDefaulConstructorCannotBeUsed("ConnectionInfoBase"));
	}

	protected ConnectionInfoBase(ConnectionType serverType)
	{
		m_eServerType = serverType;
		m_sv = null;
		m_scs = ServerCaseSensitivity.Unknown;
	}

	protected ConnectionInfoBase(ConnectionInfoBase conn)
	{
		m_eServerType = conn.m_eServerType;
		m_sv = conn.m_sv;
		m_scs = conn.ServerCaseSensitivity;
	}

	protected abstract void ConnectionParmsChanged();

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("server type = {0}", ServerType);
		return stringBuilder.ToString();
	}
}
