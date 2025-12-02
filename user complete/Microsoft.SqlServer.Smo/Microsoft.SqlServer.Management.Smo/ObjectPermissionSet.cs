using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ObjectPermissionSet : PermissionSetBase
{
	internal override int NumberOfElements => Enum.GetNames(typeof(ObjectPermissionSetValue)).Length;

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

	public bool Control
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

	public bool Connect
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

	public bool Delete
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

	public bool Execute
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

	public bool Impersonate
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

	public bool Insert
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

	public bool Receive
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

	public bool References
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

	public bool Select
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

	public bool Send
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

	public bool TakeOwnership
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

	public bool Update
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

	public bool ViewDefinition
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

	public bool ViewChangeTracking
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

	public bool CreateSequence
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

	public ObjectPermissionSet()
	{
	}

	public ObjectPermissionSet(ObjectPermissionSet oObjectPermissionSet)
		: base(oObjectPermissionSet)
	{
	}

	public ObjectPermissionSet(ObjectPermission permission)
	{
		SetBit(permission);
	}

	public ObjectPermissionSet(params ObjectPermission[] permissions)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (ObjectPermission bit in permissions)
		{
			SetBit(bit);
		}
	}

	internal void SetBit(ObjectPermission permission)
	{
		base.Storage[(int)permission.Value] = true;
	}

	internal void ResetBit(ObjectPermission permission)
	{
		base.Storage[(int)permission.Value] = false;
	}

	public ObjectPermissionSet Add(ObjectPermission permission)
	{
		SetBit(permission);
		return this;
	}

	public ObjectPermissionSet Remove(ObjectPermission permission)
	{
		ResetBit(permission);
		return this;
	}

	public static ObjectPermissionSet operator +(ObjectPermissionSet permissionLeft, ObjectPermission permissionRight)
	{
		ObjectPermissionSet objectPermissionSet = new ObjectPermissionSet(permissionLeft);
		objectPermissionSet.SetBit(permissionRight);
		return objectPermissionSet;
	}

	public static ObjectPermissionSet Add(ObjectPermissionSet permissionLeft, ObjectPermission permissionRight)
	{
		return permissionLeft + permissionRight;
	}

	public static ObjectPermissionSet operator -(ObjectPermissionSet permissionLeft, ObjectPermission permissionRight)
	{
		ObjectPermissionSet objectPermissionSet = new ObjectPermissionSet(permissionLeft);
		objectPermissionSet.ResetBit(permissionRight);
		return objectPermissionSet;
	}

	public static ObjectPermissionSet Subtract(ObjectPermissionSet permissionLeft, ObjectPermission permissionRight)
	{
		return permissionLeft - permissionRight;
	}

	internal override string PermissionCodeToPermissionName(int permissionCode)
	{
		return PermissionDecode.PermissionCodeToPermissionName<ObjectPermissionSetValue>(permissionCode);
	}

	internal override string PermissionCodeToPermissionType(int permissionCode)
	{
		return PermissionDecode.PermissionCodeToPermissionType<ObjectPermissionSetValue>(permissionCode);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object o)
	{
		return base.Equals(o);
	}

	public static bool operator ==(ObjectPermissionSet p1, ObjectPermissionSet p2)
	{
		return p1?.Equals(p2) ?? ((object)null == p2);
	}

	public static bool operator !=(ObjectPermissionSet p1, ObjectPermissionSet p2)
	{
		return !(p1 == p2);
	}
}
