using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabasePermissionSet : PermissionSetBase
{
	internal override int NumberOfElements => Enum.GetNames(typeof(DatabasePermissionSetValue)).Length;

	public bool Alter
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

	public bool AlterAnyAsymmetricKey
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

	public bool AlterAnyApplicationRole
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

	public bool AlterAnyAssembly
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

	public bool AlterAnyCertificate
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

	public bool AlterAnyDatabaseAudit
	{
		get
		{
			return base.Storage[61];
		}
		set
		{
			base.Storage[61] = value;
		}
	}

	public bool AlterAnyDataspace
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

	public bool AlterAnyDatabaseEventNotification
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

	public bool AlterAnyExternalDataSource
	{
		get
		{
			return base.Storage[63];
		}
		set
		{
			base.Storage[63] = value;
		}
	}

	public bool AlterAnyExternalFileFormat
	{
		get
		{
			return base.Storage[64];
		}
		set
		{
			base.Storage[64] = value;
		}
	}

	public bool AlterAnyFulltextCatalog
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

	public bool AlterAnyMask
	{
		get
		{
			return base.Storage[65];
		}
		set
		{
			base.Storage[65] = value;
		}
	}

	public bool AlterAnyMessageType
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

	public bool AlterAnyRole
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

	public bool AlterAnyRoute
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

	public bool AlterAnyRemoteServiceBinding
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

	public bool AlterAnyContract
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

	public bool AlterAnySymmetricKey
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

	public bool AlterAnySchema
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

	public bool AlterAnySecurityPolicy
	{
		get
		{
			return base.Storage[62];
		}
		set
		{
			base.Storage[62] = value;
		}
	}

	public bool AlterAnyService
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

	public bool AlterAnyDatabaseDdlTrigger
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

	public bool AlterAnyUser
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

	public bool Authenticate
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

	public bool BackupDatabase
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

	public bool BackupLog
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

	public bool Control
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

	public bool Connect
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

	public bool ConnectReplication
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

	public bool Checkpoint
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

	public bool CreateAggregate
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

	public bool CreateAsymmetricKey
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

	public bool CreateAssembly
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

	public bool CreateCertificate
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

	public bool CreateDatabase
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

	public bool CreateDefault
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

	public bool CreateDatabaseDdlEventNotification
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

	public bool CreateFunction
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

	public bool CreateFulltextCatalog
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

	public bool CreateMessageType
	{
		get
		{
			return base.Storage[34];
		}
		set
		{
			base.Storage[34] = value;
		}
	}

	public bool CreateProcedure
	{
		get
		{
			return base.Storage[35];
		}
		set
		{
			base.Storage[35] = value;
		}
	}

	public bool CreateQueue
	{
		get
		{
			return base.Storage[36];
		}
		set
		{
			base.Storage[36] = value;
		}
	}

	public bool CreateRole
	{
		get
		{
			return base.Storage[37];
		}
		set
		{
			base.Storage[37] = value;
		}
	}

	public bool CreateRoute
	{
		get
		{
			return base.Storage[38];
		}
		set
		{
			base.Storage[38] = value;
		}
	}

	public bool CreateRule
	{
		get
		{
			return base.Storage[39];
		}
		set
		{
			base.Storage[39] = value;
		}
	}

	public bool CreateRemoteServiceBinding
	{
		get
		{
			return base.Storage[40];
		}
		set
		{
			base.Storage[40] = value;
		}
	}

	public bool CreateContract
	{
		get
		{
			return base.Storage[41];
		}
		set
		{
			base.Storage[41] = value;
		}
	}

	public bool CreateSymmetricKey
	{
		get
		{
			return base.Storage[42];
		}
		set
		{
			base.Storage[42] = value;
		}
	}

	public bool CreateSchema
	{
		get
		{
			return base.Storage[43];
		}
		set
		{
			base.Storage[43] = value;
		}
	}

	public bool CreateSynonym
	{
		get
		{
			return base.Storage[44];
		}
		set
		{
			base.Storage[44] = value;
		}
	}

	public bool CreateService
	{
		get
		{
			return base.Storage[45];
		}
		set
		{
			base.Storage[45] = value;
		}
	}

	public bool CreateTable
	{
		get
		{
			return base.Storage[46];
		}
		set
		{
			base.Storage[46] = value;
		}
	}

	public bool CreateType
	{
		get
		{
			return base.Storage[47];
		}
		set
		{
			base.Storage[47] = value;
		}
	}

	public bool CreateView
	{
		get
		{
			return base.Storage[48];
		}
		set
		{
			base.Storage[48] = value;
		}
	}

	public bool CreateXmlSchemaCollection
	{
		get
		{
			return base.Storage[49];
		}
		set
		{
			base.Storage[49] = value;
		}
	}

	public bool Delete
	{
		get
		{
			return base.Storage[50];
		}
		set
		{
			base.Storage[50] = value;
		}
	}

	public bool Execute
	{
		get
		{
			return base.Storage[51];
		}
		set
		{
			base.Storage[51] = value;
		}
	}

	public bool Insert
	{
		get
		{
			return base.Storage[52];
		}
		set
		{
			base.Storage[52] = value;
		}
	}

	public bool References
	{
		get
		{
			return base.Storage[53];
		}
		set
		{
			base.Storage[53] = value;
		}
	}

	public bool Select
	{
		get
		{
			return base.Storage[54];
		}
		set
		{
			base.Storage[54] = value;
		}
	}

	public bool Showplan
	{
		get
		{
			return base.Storage[55];
		}
		set
		{
			base.Storage[55] = value;
		}
	}

	public bool SubscribeQueryNotifications
	{
		get
		{
			return base.Storage[56];
		}
		set
		{
			base.Storage[56] = value;
		}
	}

	public bool TakeOwnership
	{
		get
		{
			return base.Storage[57];
		}
		set
		{
			base.Storage[57] = value;
		}
	}

	public bool Unmask
	{
		get
		{
			return base.Storage[66];
		}
		set
		{
			base.Storage[66] = value;
		}
	}

	public bool Update
	{
		get
		{
			return base.Storage[58];
		}
		set
		{
			base.Storage[58] = value;
		}
	}

	public bool ViewDefinition
	{
		get
		{
			return base.Storage[59];
		}
		set
		{
			base.Storage[59] = value;
		}
	}

	public bool ViewDatabaseState
	{
		get
		{
			return base.Storage[60];
		}
		set
		{
			base.Storage[60] = value;
		}
	}

	public DatabasePermissionSet()
	{
	}

	public DatabasePermissionSet(DatabasePermissionSet oDatabasePermissionSet)
		: base(oDatabasePermissionSet)
	{
	}

	public DatabasePermissionSet(DatabasePermission permission)
	{
		SetBit(permission);
	}

	public DatabasePermissionSet(params DatabasePermission[] permissions)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (DatabasePermission bit in permissions)
		{
			SetBit(bit);
		}
	}

	internal void SetBit(DatabasePermission permission)
	{
		base.Storage[(int)permission.Value] = true;
	}

	internal void ResetBit(DatabasePermission permission)
	{
		base.Storage[(int)permission.Value] = false;
	}

	public DatabasePermissionSet Add(DatabasePermission permission)
	{
		SetBit(permission);
		return this;
	}

	public DatabasePermissionSet Remove(DatabasePermission permission)
	{
		ResetBit(permission);
		return this;
	}

	public static DatabasePermissionSet operator +(DatabasePermissionSet permissionLeft, DatabasePermission permissionRight)
	{
		DatabasePermissionSet databasePermissionSet = new DatabasePermissionSet(permissionLeft);
		databasePermissionSet.SetBit(permissionRight);
		return databasePermissionSet;
	}

	public static DatabasePermissionSet Add(DatabasePermissionSet permissionLeft, DatabasePermission permissionRight)
	{
		return permissionLeft + permissionRight;
	}

	public static DatabasePermissionSet operator -(DatabasePermissionSet permissionLeft, DatabasePermission permissionRight)
	{
		DatabasePermissionSet databasePermissionSet = new DatabasePermissionSet(permissionLeft);
		databasePermissionSet.ResetBit(permissionRight);
		return databasePermissionSet;
	}

	public static DatabasePermissionSet Subtract(DatabasePermissionSet permissionLeft, DatabasePermission permissionRight)
	{
		return permissionLeft - permissionRight;
	}

	internal override string PermissionCodeToPermissionName(int permissionCode)
	{
		return PermissionDecode.PermissionCodeToPermissionName<DatabasePermissionSetValue>(permissionCode);
	}

	internal override string PermissionCodeToPermissionType(int permissionCode)
	{
		return PermissionDecode.PermissionCodeToPermissionType<DatabasePermissionSetValue>(permissionCode);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object o)
	{
		return base.Equals(o);
	}

	public static bool operator ==(DatabasePermissionSet p1, DatabasePermissionSet p2)
	{
		return p1?.Equals(p2) ?? ((object)null == p2);
	}

	public static bool operator !=(DatabasePermissionSet p1, DatabasePermissionSet p2)
	{
		return !(p1 == p2);
	}
}
