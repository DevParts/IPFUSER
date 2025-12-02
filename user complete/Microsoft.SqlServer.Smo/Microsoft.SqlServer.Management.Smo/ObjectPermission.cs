namespace Microsoft.SqlServer.Management.Smo;

public sealed class ObjectPermission
{
	private ObjectPermissionSetValue m_value;

	internal ObjectPermissionSetValue Value => m_value;

	public static ObjectPermission Alter => new ObjectPermission(ObjectPermissionSetValue.Alter);

	public static ObjectPermission Control => new ObjectPermission(ObjectPermissionSetValue.Control);

	public static ObjectPermission Connect => new ObjectPermission(ObjectPermissionSetValue.Connect);

	public static ObjectPermission Delete => new ObjectPermission(ObjectPermissionSetValue.Delete);

	public static ObjectPermission Execute => new ObjectPermission(ObjectPermissionSetValue.Execute);

	public static ObjectPermission Impersonate => new ObjectPermission(ObjectPermissionSetValue.Impersonate);

	public static ObjectPermission Insert => new ObjectPermission(ObjectPermissionSetValue.Insert);

	public static ObjectPermission Receive => new ObjectPermission(ObjectPermissionSetValue.Receive);

	public static ObjectPermission References => new ObjectPermission(ObjectPermissionSetValue.References);

	public static ObjectPermission Select => new ObjectPermission(ObjectPermissionSetValue.Select);

	public static ObjectPermission Send => new ObjectPermission(ObjectPermissionSetValue.Send);

	public static ObjectPermission TakeOwnership => new ObjectPermission(ObjectPermissionSetValue.TakeOwnership);

	public static ObjectPermission Update => new ObjectPermission(ObjectPermissionSetValue.Update);

	public static ObjectPermission ViewDefinition => new ObjectPermission(ObjectPermissionSetValue.ViewDefinition);

	public static ObjectPermission ViewChangeTracking => new ObjectPermission(ObjectPermissionSetValue.ViewChangeTracking);

	public static ObjectPermission CreateSequence => new ObjectPermission(ObjectPermissionSetValue.CreateSequence);

	internal ObjectPermission(ObjectPermissionSetValue permissionValue)
	{
		m_value = permissionValue;
	}

	public static implicit operator ObjectPermissionSet(ObjectPermission permission)
	{
		return new ObjectPermissionSet(permission);
	}

	public static ObjectPermissionSet ToObjectPermissionSet(ObjectPermission permission)
	{
		return permission;
	}

	public static ObjectPermissionSet operator +(ObjectPermission permissionLeft, ObjectPermission permissionRight)
	{
		ObjectPermissionSet objectPermissionSet = new ObjectPermissionSet(permissionLeft);
		objectPermissionSet.SetBit(permissionRight);
		return objectPermissionSet;
	}

	public static ObjectPermissionSet Add(ObjectPermission permissionLeft, ObjectPermission permissionRight)
	{
		return permissionLeft + permissionRight;
	}

	public static ObjectPermissionSet operator |(ObjectPermission permissionLeft, ObjectPermission permissionRight)
	{
		ObjectPermissionSet objectPermissionSet = new ObjectPermissionSet(permissionLeft);
		objectPermissionSet.SetBit(permissionRight);
		return objectPermissionSet;
	}

	public static ObjectPermissionSet BitwiseOr(ObjectPermission permissionLeft, ObjectPermission permissionRight)
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

	public static bool operator ==(ObjectPermission p1, ObjectPermission p2)
	{
		return p1?.Equals(p2) ?? ((object)null == p2);
	}

	public static bool operator !=(ObjectPermission p1, ObjectPermission p2)
	{
		return !(p1 == p2);
	}
}
