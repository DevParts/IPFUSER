using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.RegSvrEnum;

namespace Microsoft.SqlServer.Management.Smo.RegisteredServers;

internal sealed class ServerType : ServerGroupBase
{
	public static readonly int WindowsAuthentication = 0;

	public static readonly int SqlAuthentication = 1;

	private RegSvrConnectionInfo m_RegSvrConnectionInfo;

	internal override RegSvrConnectionInfo ConnectionInfo => m_RegSvrConnectionInfo;

	public override Urn Urn => new Urn(UrnSuffix);

	internal static string UrnSuffix
	{
		get
		{
			Guid sqlServerTypeGuid = RegSvrConnectionInfo.SqlServerTypeGuid;
			return "ServerType[@ServerType='" + sqlServerTypeGuid.ToString() + "']";
		}
	}

	protected internal override Urn UrnSkeleton => new Urn("ServerType");

	public ServerType(string name)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		m_RegSvrConnectionInfo = new RegSvrConnectionInfo();
		if (!RegistrationProvider.ServerTypeNodeExists(RegSvrConnectionInfo.SqlServerTypeGuid))
		{
			ServerTypeRegistrationInfo val = new ServerTypeRegistrationInfo();
			((RegistrationInfo)val).ServerType = RegSvrConnectionInfo.SqlServerTypeGuid;
			((RegistrationInfo)val).FriendlyName = ExceptionTemplatesImpl.SqlServerTypeName;
			RegistrationProvider.AddServerTypeNode(val);
			RegistrationProvider.MarkNodeModified((RegistrationInfo)(object)val);
		}
		key = new SimpleObjectKey(name);
		SetState(SqlSmoState.Existing);
	}

	public override void Refresh()
	{
		RegistrationProvider.Refresh();
		ClearCollections();
	}
}
