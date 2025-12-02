namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerPermission
{
	private ServerPermissionSetValue m_value;

	internal ServerPermissionSetValue Value => m_value;

	public static ServerPermission AdministerBulkOperations => new ServerPermission(ServerPermissionSetValue.AdministerBulkOperations);

	public static ServerPermission AlterAnyServerAudit => new ServerPermission(ServerPermissionSetValue.AlterAnyServerAudit);

	public static ServerPermission AlterAnyCredential => new ServerPermission(ServerPermissionSetValue.AlterAnyCredential);

	public static ServerPermission AlterAnyConnection => new ServerPermission(ServerPermissionSetValue.AlterAnyConnection);

	public static ServerPermission AlterAnyDatabase => new ServerPermission(ServerPermissionSetValue.AlterAnyDatabase);

	public static ServerPermission AlterAnyEventNotification => new ServerPermission(ServerPermissionSetValue.AlterAnyEventNotification);

	public static ServerPermission AlterAnyEndpoint => new ServerPermission(ServerPermissionSetValue.AlterAnyEndpoint);

	public static ServerPermission AlterAnyLogin => new ServerPermission(ServerPermissionSetValue.AlterAnyLogin);

	public static ServerPermission AlterAnyLinkedServer => new ServerPermission(ServerPermissionSetValue.AlterAnyLinkedServer);

	public static ServerPermission AlterResources => new ServerPermission(ServerPermissionSetValue.AlterResources);

	public static ServerPermission AlterServerState => new ServerPermission(ServerPermissionSetValue.AlterServerState);

	public static ServerPermission AlterSettings => new ServerPermission(ServerPermissionSetValue.AlterSettings);

	public static ServerPermission AlterTrace => new ServerPermission(ServerPermissionSetValue.AlterTrace);

	public static ServerPermission AuthenticateServer => new ServerPermission(ServerPermissionSetValue.AuthenticateServer);

	public static ServerPermission ControlServer => new ServerPermission(ServerPermissionSetValue.ControlServer);

	public static ServerPermission ConnectSql => new ServerPermission(ServerPermissionSetValue.ConnectSql);

	public static ServerPermission CreateAnyDatabase => new ServerPermission(ServerPermissionSetValue.CreateAnyDatabase);

	public static ServerPermission CreateDdlEventNotification => new ServerPermission(ServerPermissionSetValue.CreateDdlEventNotification);

	public static ServerPermission CreateEndpoint => new ServerPermission(ServerPermissionSetValue.CreateEndpoint);

	public static ServerPermission CreateTraceEventNotification => new ServerPermission(ServerPermissionSetValue.CreateTraceEventNotification);

	public static ServerPermission Shutdown => new ServerPermission(ServerPermissionSetValue.Shutdown);

	public static ServerPermission ViewAnyDefinition => new ServerPermission(ServerPermissionSetValue.ViewAnyDefinition);

	public static ServerPermission ViewAnyDatabase => new ServerPermission(ServerPermissionSetValue.ViewAnyDatabase);

	public static ServerPermission ViewServerState => new ServerPermission(ServerPermissionSetValue.ViewServerState);

	public static ServerPermission ExternalAccessAssembly => new ServerPermission(ServerPermissionSetValue.ExternalAccessAssembly);

	public static ServerPermission UnsafeAssembly => new ServerPermission(ServerPermissionSetValue.UnsafeAssembly);

	public static ServerPermission AlterAnyServerRole => new ServerPermission(ServerPermissionSetValue.AlterAnyServerRole);

	public static ServerPermission CreateServerRole => new ServerPermission(ServerPermissionSetValue.CreateServerRole);

	public static ServerPermission AlterAnyAvailabilityGroup => new ServerPermission(ServerPermissionSetValue.AlterAnyAvailabilityGroup);

	public static ServerPermission CreateAvailabilityGroup => new ServerPermission(ServerPermissionSetValue.CreateAvailabilityGroup);

	public static ServerPermission AlterAnyEventSession => new ServerPermission(ServerPermissionSetValue.AlterAnyEventSession);

	public static ServerPermission SelectAllUserSecurables => new ServerPermission(ServerPermissionSetValue.SelectAllUserSecurables);

	public static ServerPermission ConnectAnyDatabase => new ServerPermission(ServerPermissionSetValue.ConnectAnyDatabase);

	public static ServerPermission ImpersonateAnyLogin => new ServerPermission(ServerPermissionSetValue.ImpersonateAnyLogin);

	internal ServerPermission(ServerPermissionSetValue permissionValue)
	{
		m_value = permissionValue;
	}

	public static implicit operator ServerPermissionSet(ServerPermission permission)
	{
		return new ServerPermissionSet(permission);
	}

	public static ServerPermissionSet ToServerPermissionSet(ServerPermission permission)
	{
		return permission;
	}

	public static ServerPermissionSet operator +(ServerPermission permissionLeft, ServerPermission permissionRight)
	{
		ServerPermissionSet serverPermissionSet = new ServerPermissionSet(permissionLeft);
		serverPermissionSet.SetBit(permissionRight);
		return serverPermissionSet;
	}

	public static ServerPermissionSet Add(ServerPermission permissionLeft, ServerPermission permissionRight)
	{
		return permissionLeft + permissionRight;
	}

	public static ServerPermissionSet operator |(ServerPermission permissionLeft, ServerPermission permissionRight)
	{
		ServerPermissionSet serverPermissionSet = new ServerPermissionSet(permissionLeft);
		serverPermissionSet.SetBit(permissionRight);
		return serverPermissionSet;
	}

	public static ServerPermissionSet BitwiseOr(ServerPermission permissionLeft, ServerPermission permissionRight)
	{
		return permissionLeft | permissionRight;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}

	public override bool Equals(object o)
	{
		return m_value.Equals(o);
	}

	public static bool operator ==(ServerPermission p1, ServerPermission p2)
	{
		return p1?.Equals(p2) ?? ((object)null == p2);
	}

	public static bool operator !=(ServerPermission p1, ServerPermission p2)
	{
		return !(p1 == p2);
	}
}
