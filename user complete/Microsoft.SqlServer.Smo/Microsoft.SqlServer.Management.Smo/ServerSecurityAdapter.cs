using System.Data;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ServerSecurityAdapter : ServerAdapterBase, IDmfAdapter, IServerSecurityFacet, IDmfFacet
{
	public bool CmdExecRightsForSystemAdminsOnly => base.Server.JobServer.SysAdminOnly;

	public bool ReplaceAlertTokensEnabled
	{
		get
		{
			return base.Server.JobServer.ReplaceAlertTokensEnabled;
		}
		set
		{
			base.Server.JobServer.ReplaceAlertTokensEnabled = value;
		}
	}

	public bool ProxyAccountIsGrantedToPublicRole
	{
		get
		{
			DataTable dataTable = base.Server.Roles["public"].EnumAgentProxyAccounts();
			return dataTable.Rows.Count > 0;
		}
	}

	public bool ProxyAccountEnabled => base.Server.ProxyAccount.IsEnabled;

	public bool PublicServerRoleIsGrantedPermissions
	{
		get
		{
			bool result = false;
			ServerPermissionInfo[] array = base.Server.EnumServerPermissions("public");
			ServerPermissionInfo[] array2 = array;
			foreach (ServerPermissionInfo serverPermissionInfo in array2)
			{
				if (serverPermissionInfo.PermissionState == PermissionState.Grant || serverPermissionInfo.PermissionState == PermissionState.GrantWithGrant)
				{
					return true;
				}
			}
			ObjectPermissionInfo[] array3 = base.Server.EnumObjectPermissions("public");
			ObjectPermissionInfo[] array4 = array3;
			foreach (ObjectPermissionInfo objectPermissionInfo in array4)
			{
				if (objectPermissionInfo.PermissionState == PermissionState.Grant || objectPermissionInfo.PermissionState == PermissionState.GrantWithGrant)
				{
					return true;
				}
			}
			return result;
		}
	}

	public ServerSecurityAdapter(Server obj)
		: base(obj)
	{
	}

	public override void Refresh()
	{
		base.Server.Information.Refresh();
		base.Server.Configuration.Refresh();
		base.Server.Settings.Refresh();
		base.Server.JobServer.Refresh();
	}

	public override void Alter()
	{
		base.Server.Configuration.Alter(overrideValueChecking: true);
		base.Server.JobServer.Alter();
	}
}
