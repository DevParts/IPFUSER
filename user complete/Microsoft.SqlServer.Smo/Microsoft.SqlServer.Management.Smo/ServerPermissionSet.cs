using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerPermissionSet : PermissionSetBase
{
	internal override int NumberOfElements => Enum.GetNames(typeof(ServerPermissionSetValue)).Length;

	public bool AdministerBulkOperations
	{
		get
		{
			return base.Storage[0];
		}
		set
		{
			base.Storage[0] = value;
		}
	}

	public bool AlterAnyServerAudit
	{
		get
		{
			return base.Storage[25];
		}
		set
		{
			base.Storage[25] = value;
		}
	}

	public bool AlterAnyCredential
	{
		get
		{
			return base.Storage[1];
		}
		set
		{
			base.Storage[1] = value;
		}
	}

	public bool AlterAnyConnection
	{
		get
		{
			return base.Storage[2];
		}
		set
		{
			base.Storage[2] = value;
		}
	}

	public bool AlterAnyDatabase
	{
		get
		{
			return base.Storage[3];
		}
		set
		{
			base.Storage[3] = value;
		}
	}

	public bool AlterAnyEventNotification
	{
		get
		{
			return base.Storage[4];
		}
		set
		{
			base.Storage[4] = value;
		}
	}

	public bool AlterAnyEndpoint
	{
		get
		{
			return base.Storage[5];
		}
		set
		{
			base.Storage[5] = value;
		}
	}

	public bool AlterAnyLogin
	{
		get
		{
			return base.Storage[6];
		}
		set
		{
			base.Storage[6] = value;
		}
	}

	public bool AlterAnyLinkedServer
	{
		get
		{
			return base.Storage[7];
		}
		set
		{
			base.Storage[7] = value;
		}
	}

	public bool AlterResources
	{
		get
		{
			return base.Storage[8];
		}
		set
		{
			base.Storage[8] = value;
		}
	}

	public bool AlterServerState
	{
		get
		{
			return base.Storage[9];
		}
		set
		{
			base.Storage[9] = value;
		}
	}

	public bool AlterSettings
	{
		get
		{
			return base.Storage[10];
		}
		set
		{
			base.Storage[10] = value;
		}
	}

	public bool AlterTrace
	{
		get
		{
			return base.Storage[11];
		}
		set
		{
			base.Storage[11] = value;
		}
	}

	public bool AuthenticateServer
	{
		get
		{
			return base.Storage[12];
		}
		set
		{
			base.Storage[12] = value;
		}
	}

	public bool ControlServer
	{
		get
		{
			return base.Storage[13];
		}
		set
		{
			base.Storage[13] = value;
		}
	}

	public bool ConnectSql
	{
		get
		{
			return base.Storage[14];
		}
		set
		{
			base.Storage[14] = value;
		}
	}

	public bool CreateAnyDatabase
	{
		get
		{
			return base.Storage[15];
		}
		set
		{
			base.Storage[15] = value;
		}
	}

	public bool CreateDdlEventNotification
	{
		get
		{
			return base.Storage[16];
		}
		set
		{
			base.Storage[16] = value;
		}
	}

	public bool CreateEndpoint
	{
		get
		{
			return base.Storage[17];
		}
		set
		{
			base.Storage[17] = value;
		}
	}

	public bool CreateTraceEventNotification
	{
		get
		{
			return base.Storage[18];
		}
		set
		{
			base.Storage[18] = value;
		}
	}

	public bool Shutdown
	{
		get
		{
			return base.Storage[19];
		}
		set
		{
			base.Storage[19] = value;
		}
	}

	public bool ViewAnyDefinition
	{
		get
		{
			return base.Storage[20];
		}
		set
		{
			base.Storage[20] = value;
		}
	}

	public bool ViewAnyDatabase
	{
		get
		{
			return base.Storage[21];
		}
		set
		{
			base.Storage[21] = value;
		}
	}

	public bool ViewServerState
	{
		get
		{
			return base.Storage[22];
		}
		set
		{
			base.Storage[22] = value;
		}
	}

	public bool ExternalAccessAssembly
	{
		get
		{
			return base.Storage[23];
		}
		set
		{
			base.Storage[23] = value;
		}
	}

	public bool UnsafeAssembly
	{
		get
		{
			return base.Storage[24];
		}
		set
		{
			base.Storage[24] = value;
		}
	}

	public bool AlterAnyServerRole
	{
		get
		{
			return base.Storage[26];
		}
		set
		{
			base.Storage[26] = value;
		}
	}

	public bool CreateServerRole
	{
		get
		{
			return base.Storage[27];
		}
		set
		{
			base.Storage[27] = value;
		}
	}

	public bool AlterAnyAvailabilityGroup
	{
		get
		{
			return base.Storage[28];
		}
		set
		{
			base.Storage[28] = value;
		}
	}

	public bool CreateAvailabilityGroup
	{
		get
		{
			return base.Storage[29];
		}
		set
		{
			base.Storage[29] = value;
		}
	}

	public bool AlterAnyEventSession
	{
		get
		{
			return base.Storage[30];
		}
		set
		{
			base.Storage[30] = value;
		}
	}

	public bool SelectAllUserSecurables
	{
		get
		{
			return base.Storage[31];
		}
		set
		{
			base.Storage[31] = value;
		}
	}

	public bool ConnectAnyDatabase
	{
		get
		{
			return base.Storage[32];
		}
		set
		{
			base.Storage[32] = value;
		}
	}

	public bool ImpersonateAnyLogin
	{
		get
		{
			return base.Storage[33];
		}
		set
		{
			base.Storage[33] = value;
		}
	}

	public ServerPermissionSet()
	{
	}

	public ServerPermissionSet(ServerPermissionSet oServerPermissionSet)
		: base(oServerPermissionSet)
	{
	}

	public ServerPermissionSet(ServerPermission permission)
	{
		SetBit(permission);
	}

	public ServerPermissionSet(params ServerPermission[] permissions)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (ServerPermission bit in permissions)
		{
			SetBit(bit);
		}
	}

	internal void SetBit(ServerPermission permission)
	{
		base.Storage[(int)permission.Value] = true;
	}

	internal void ResetBit(ServerPermission permission)
	{
		base.Storage[(int)permission.Value] = false;
	}

	public ServerPermissionSet Add(ServerPermission permission)
	{
		SetBit(permission);
		return this;
	}

	public ServerPermissionSet Remove(ServerPermission permission)
	{
		ResetBit(permission);
		return this;
	}

	public static ServerPermissionSet operator +(ServerPermissionSet permissionLeft, ServerPermission permissionRight)
	{
		ServerPermissionSet serverPermissionSet = new ServerPermissionSet(permissionLeft);
		serverPermissionSet.SetBit(permissionRight);
		return serverPermissionSet;
	}

	public static ServerPermissionSet Add(ServerPermissionSet permissionLeft, ServerPermission permissionRight)
	{
		return permissionLeft + permissionRight;
	}

	public static ServerPermissionSet operator -(ServerPermissionSet permissionLeft, ServerPermission permissionRight)
	{
		ServerPermissionSet serverPermissionSet = new ServerPermissionSet(permissionLeft);
		serverPermissionSet.ResetBit(permissionRight);
		return serverPermissionSet;
	}

	public static ServerPermissionSet Subtract(ServerPermissionSet permissionLeft, ServerPermission permissionRight)
	{
		return permissionLeft - permissionRight;
	}

	internal override string PermissionCodeToPermissionName(int permissionCode)
	{
		return PermissionDecode.PermissionCodeToPermissionName<ServerPermissionSetValue>(permissionCode);
	}

	internal override string PermissionCodeToPermissionType(int permissionCode)
	{
		return PermissionDecode.PermissionCodeToPermissionType<ServerPermissionSetValue>(permissionCode);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object o)
	{
		return base.Equals(o);
	}

	public static bool operator ==(ServerPermissionSet p1, ServerPermissionSet p2)
	{
		return p1?.Equals(p2) ?? ((object)null == p2);
	}

	public static bool operator !=(ServerPermissionSet p1, ServerPermissionSet p2)
	{
		return !(p1 == p2);
	}
}
