namespace Microsoft.SqlServer.Management.Smo;

public class PermissionInfo
{
	internal class ObjIdent
	{
		private ObjectClass objectClass;

		private string objectName;

		private string objectSchema;

		private int objectId;

		public ObjectClass ObjectClass => objectClass;

		public string ObjectName => objectName;

		public string ObjectSchema => objectSchema;

		public int ObjectID => objectId;

		internal ObjIdent(ObjectClass objectClass, string objectName, string objectSchema, int objectId)
		{
			this.objectClass = objectClass;
			this.objectName = objectName;
			this.objectSchema = objectSchema;
			this.objectId = objectId;
		}

		internal ObjIdent(ObjectClass objectClass)
		{
			this.objectClass = objectClass;
		}

		internal void SetData(SqlSmoObject obj)
		{
			objectName = ((SimpleObjectKey)obj.key).Name;
			objectId = ((!(obj is Server)) ? ((int)obj.GetPropValue("ID")) : 0);
			if (obj is ScriptSchemaObjectBase scriptSchemaObjectBase)
			{
				objectSchema = scriptSchemaObjectBase.Schema;
			}
		}
	}

	private string grantee;

	private PrincipalType granteeType;

	private string grantor;

	private PrincipalType grantorType;

	private PermissionState permissionState;

	private PermissionSetBase permissionSet;

	private string columnName;

	private ObjIdent objIdent;

	public string Grantee => grantee;

	public PrincipalType GranteeType => granteeType;

	public string Grantor => grantor;

	public PrincipalType GrantorType => grantorType;

	public PermissionState PermissionState => permissionState;

	protected internal PermissionSetBase PermissionTypeInternal => permissionSet;

	public string ColumnName => columnName;

	public ObjectClass ObjectClass => objIdent.ObjectClass;

	public string ObjectName => objIdent.ObjectName;

	public string ObjectSchema => objIdent.ObjectSchema;

	public int ObjectID => objIdent.ObjectID;

	internal PermissionInfo()
	{
	}

	internal void SetPermissionInfoData(string grantee, PrincipalType granteeType, string grantor, PrincipalType grantorType, PermissionState permissionState, PermissionSetBase permissionSet, string columnName, ObjIdent objIdent)
	{
		this.grantee = grantee;
		this.granteeType = granteeType;
		this.grantor = grantor;
		this.grantorType = grantorType;
		this.permissionState = permissionState;
		this.permissionSet = permissionSet;
		this.columnName = columnName;
		this.objIdent = objIdent;
	}

	internal void SetPermissionState(PermissionState ps)
	{
		permissionState = ps;
	}

	public override string ToString()
	{
		if (ColumnName == null)
		{
			return string.Format(SmoApplication.DefaultCulture, "{0} {1}: {2}, {3}, {4}", SqlSmoObject.MakeSqlBraket(ObjectName), ObjectClass.ToString(), Grantee, PermissionState, PermissionTypeInternal);
		}
		return string.Format(SmoApplication.DefaultCulture, "{0}.{5} {1}: {2}, {3}, {4}", SqlSmoObject.MakeSqlBraket(ObjectName), ObjectClass.ToString(), Grantee, PermissionState, PermissionTypeInternal, SqlSmoObject.MakeSqlBraket(ColumnName));
	}
}
