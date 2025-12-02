using System.Management;

namespace Microsoft.SqlServer.Management.Common;

internal class WmiMgmtScopeConnection : ConnectionInfoBase
{
	private ManagementScope managementScope;

	public ManagementScope ManagementScope
	{
		get
		{
			return managementScope;
		}
		set
		{
			managementScope = value;
		}
	}

	public WmiMgmtScopeConnection()
		: base(ConnectionType.WmiManagementScope)
	{
	}

	public WmiMgmtScopeConnection(ManagementScope managementScope)
		: base(ConnectionType.WmiManagementScope)
	{
		ManagementScope = managementScope;
	}

	private WmiMgmtScopeConnection(WmiMgmtScopeConnection conn)
		: base(ConnectionType.WmiManagementScope)
	{
		ManagementScope = conn.ManagementScope;
	}

	public WmiMgmtScopeConnection Copy()
	{
		return new WmiMgmtScopeConnection(this);
	}

	protected override void ConnectionParmsChanged()
	{
	}
}
