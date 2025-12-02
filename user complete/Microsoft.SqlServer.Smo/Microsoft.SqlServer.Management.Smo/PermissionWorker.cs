using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class PermissionWorker
{
	internal enum PermissionEnumKind
	{
		Column,
		Object,
		Database,
		Server
	}

	private static void AddArrayToStringBuider(StringBuilder sb, string[] list)
	{
		bool flag = true;
		foreach (string s in list)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				sb.Append(", ");
			}
			sb.Append(SqlSmoObject.MakeSqlBraket(s));
		}
	}

	internal static void CheckPermissionsAllowed(SqlSmoObject obj)
	{
		if (obj.ServerVersion.Major < 9)
		{
			string name = obj.GetType().Name;
			if (name == "Login" || name == "User")
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.SupportedOnlyOn90);
			}
		}
	}

	internal static string ScriptPermissionInfo(SqlSmoObject obj, PermissionInfo pi, ScriptingPreferences sp, bool grantGrant, bool cascade)
	{
		if (PermissionState.GrantWithGrant == pi.PermissionState)
		{
			grantGrant = true;
			pi.SetPermissionState(PermissionState.Grant);
		}
		return Script(pi.PermissionState, obj, pi.PermissionTypeInternal, new string[1] { pi.Grantee }, (pi.ColumnName != null) ? new string[1] { pi.ColumnName } : null, grantGrant, cascade, pi.Grantor, sp);
	}

	internal static string ScriptPermissionInfo(SqlSmoObject obj, PermissionInfo pi, ScriptingPreferences sp)
	{
		return ScriptPermissionInfo(obj, pi, sp, grantGrant: false, cascade: false);
	}

	private static string GetObjectName(SqlSmoObject obj, ScriptingPreferences sp)
	{
		if (obj == null || obj is Database || obj is Server)
		{
			return null;
		}
		if (obj is NamedSmoObject namedSmoObject)
		{
			return namedSmoObject.PermissionPrefix + namedSmoObject.FormatFullNameForScripting(sp);
		}
		return SqlSmoObject.MakeSqlBraket(((SimpleObjectKey)obj.key).Name);
	}

	private static string Script(PermissionState ps, SqlSmoObject obj, PermissionSetBase pb, string[] granteeNames, string[] columnNames, bool grantGrant, bool cascade, string asRole, ScriptingPreferences sp)
	{
		string objectName = GetObjectName(obj, sp);
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		switch (ps)
		{
		case PermissionState.Grant:
			stringBuilder.Append("GRANT ");
			break;
		case PermissionState.Deny:
			stringBuilder.Append("DENY ");
			if (sp.TargetServerVersion < SqlServerVersion.Version90)
			{
				flag = false;
			}
			break;
		case PermissionState.Revoke:
			stringBuilder.Append("REVOKE ");
			break;
		}
		if (grantGrant && PermissionState.Revoke == ps)
		{
			stringBuilder.Append("GRANT OPTION FOR ");
		}
		if (!pb.AddPermissionList(stringBuilder))
		{
			throw new SmoException(ExceptionTemplatesImpl.NoPermissions);
		}
		if (objectName != null)
		{
			stringBuilder.Append(" ON ");
			stringBuilder.Append(objectName);
		}
		if (columnNames != null)
		{
			stringBuilder.Append(" (");
			AddArrayToStringBuider(stringBuilder, columnNames);
			stringBuilder.Append(")");
		}
		if (granteeNames == null)
		{
			throw new ArgumentNullException("granteeNames");
		}
		if (granteeNames.Length == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.EmptyInputParam("granteeNames", "StringCollection"));
		}
		stringBuilder.Append(" TO ");
		AddArrayToStringBuider(stringBuilder, granteeNames);
		if (grantGrant && PermissionState.Grant == ps)
		{
			stringBuilder.Append(" WITH GRANT OPTION ");
		}
		if (cascade)
		{
			stringBuilder.Append(" CASCADE");
		}
		if (!string.IsNullOrEmpty(asRole) && flag)
		{
			stringBuilder.Append(" AS ");
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket(asRole));
		}
		return stringBuilder.ToString();
	}

	internal static PermissionSetBase GetPermissionSetBase(PermissionEnumKind kind, int i)
	{
		PermissionSetBase permissionSetBase = null;
		switch (kind)
		{
		case PermissionEnumKind.Column:
			permissionSetBase = new ObjectPermissionSet();
			break;
		case PermissionEnumKind.Object:
			permissionSetBase = new ObjectPermissionSet();
			break;
		case PermissionEnumKind.Database:
			permissionSetBase = new DatabasePermissionSet();
			break;
		case PermissionEnumKind.Server:
			permissionSetBase = new ServerPermissionSet();
			break;
		}
		permissionSetBase.SetBitAt(i);
		return permissionSetBase;
	}

	internal static ObjectClass GetObjectClass(SqlSmoObject obj)
	{
		string name = obj.GetType().Name;
		switch (name)
		{
		case "XmlSchemaCollection":
			return ObjectClass.XmlNamespace;
		case "BrokerService":
			return ObjectClass.Service;
		case "UserDefinedDataType":
		case "UserDefinedTableType":
			return ObjectClass.UserDefinedType;
		case "ExtendedStoredProcedure":
		case "ServiceQueue":
		case "StoredProcedure":
		case "Synonym":
		case "Table":
		case "UserDefinedAggregate":
		case "UserDefinedFunction":
		case "View":
		case "Column":
			return ObjectClass.ObjectOrColumn;
		default:
			return (ObjectClass)Enum.Parse(typeof(ObjectClass), name, ignoreCase: true);
		}
	}

	internal static string GetObjectOwner(SqlSmoObject smoObj)
	{
		string text = string.Empty;
		SqlSmoObject sqlSmoObject = smoObj;
		if (smoObj is Column)
		{
			sqlSmoObject = ((Column)smoObj).Parent;
		}
		if (sqlSmoObject.Properties.Contains("Owner"))
		{
			text = sqlSmoObject.GetPropValueOptionalAllowNull("Owner") as string;
			if (string.IsNullOrEmpty(text) && sqlSmoObject.Properties.Contains("IsSchemaOwned") && sqlSmoObject is ScriptSchemaObjectBase)
			{
				string schema = ((ScriptSchemaObjectBase)sqlSmoObject).Schema;
				if (!string.IsNullOrEmpty(schema))
				{
					Database database = SfcResolverHelper.GetDatabase(sqlSmoObject);
					Schema schema2 = database.Schemas[schema];
					if (schema2 != null)
					{
						text = schema2.GetPropValueOptionalAllowNull("Owner") as string;
					}
				}
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			text = "dbo";
		}
		return text;
	}

	internal static void AddPermission(PermissionState ps, SqlSmoObject obj, PermissionSetBase pb, string[] granteeNames, bool grantGrant, bool cascade, string asRole)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < pb.Storage.Length; i++)
		{
			if (!pb.Storage[i])
			{
				continue;
			}
			foreach (string text in granteeNames)
			{
				PrincipalType principalType = PrincipalType.None;
				PrincipalType principalType2 = PrincipalType.None;
				ObjectClass objectClass = GetObjectClass(obj);
				string empty = string.Empty;
				empty = (string.IsNullOrEmpty(asRole) ? GetObjectOwner(obj) : asRole);
				stringBuilder.Length = 0;
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}_{1}_{2}_{3}_{4}_{5}_{6}", text, (int)principalType, empty, (int)principalType2, (int)objectClass, (int)ps, (ObjectPermissionSetValue)i);
				UserPermission userPermission = new UserPermission();
				userPermission.Parent = obj;
				userPermission.Name = stringBuilder.ToString();
				userPermission.PermissionState = ps;
				if (grantGrant)
				{
					if (userPermission.PermissionState == PermissionState.Grant)
					{
						userPermission.PermissionState = PermissionState.GrantWithGrant;
					}
					else
					{
						_ = userPermission.PermissionState;
						_ = 82;
					}
				}
				userPermission.Grantor = empty;
				userPermission.ObjectClass = objectClass;
				userPermission.GrantorType = principalType2;
				userPermission.GranteeType = principalType;
				userPermission.Code = (ObjectPermissionSetValue)i;
				userPermission.Grantee = text;
				obj.Permissions.AddExisting(userPermission);
			}
		}
	}

	internal static void Execute(PermissionState ps, SqlSmoObject obj, PermissionSetBase pb, string[] granteeNames, string[] columnNames, bool grantGrant, bool cascade, string asRole)
	{
		CheckPermissionsAllowed(obj);
		try
		{
			StringCollection stringCollection = new StringCollection();
			string text = obj?.GetDBName();
			if (text == null || text.Length <= 0)
			{
				text = "master";
			}
			SqlSmoObject obj2 = obj;
			if (obj is Database || obj is Server)
			{
				obj2 = null;
			}
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.ForDirectExecution = true;
			if (obj.DatabaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse)
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "use {0}", new object[1] { SqlSmoObject.MakeSqlBraket(text) }));
			}
			stringCollection.Add(Script(ps, obj2, pb, granteeNames, columnNames, grantGrant, cascade, asRole, scriptingPreferences));
			if (!obj.IsDesignMode)
			{
				obj.ExecutionManager.ExecuteNonQuery(stringCollection);
				obj.ClearUserPemissions();
			}
			else if (columnNames != null && columnNames.Length != 0)
			{
				ColumnCollection columnCollection = null;
				switch (obj.GetType().Name)
				{
				case "Table":
					columnCollection = ((Table)obj).Columns;
					break;
				case "View":
					columnCollection = ((View)obj).Columns;
					break;
				case "UserDefinedFunction":
					columnCollection = ((UserDefinedFunction)obj).Columns;
					break;
				}
				foreach (string name in columnNames)
				{
					Column column = columnCollection[name];
					if (column != null)
					{
						AddPermission(ps, column, pb, granteeNames, grantGrant, cascade, asRole);
						continue;
					}
					throw new MissingObjectException(ExceptionTemplatesImpl.ObjectDoesNotExist("Column", name));
				}
			}
			else
			{
				AddPermission(ps, obj, pb, granteeNames, grantGrant, cascade, asRole);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ps.ToString(), obj, ex).SetHelpContext(ps.ToString());
		}
	}

	internal static PermissionInfo GetPermissionInfo(PermissionEnumKind kind)
	{
		PermissionInfo result = null;
		switch (kind)
		{
		case PermissionEnumKind.Column:
			result = new ObjectPermissionInfo();
			break;
		case PermissionEnumKind.Object:
			result = new ObjectPermissionInfo();
			break;
		case PermissionEnumKind.Database:
			result = new DatabasePermissionInfo();
			break;
		case PermissionEnumKind.Server:
			result = new ServerPermissionInfo();
			break;
		}
		return result;
	}

	internal static PermissionInfo[] GetPermissionInfoArray(PermissionEnumKind kind, int count)
	{
		PermissionInfo[] result = null;
		switch (kind)
		{
		case PermissionEnumKind.Column:
			result = new ObjectPermissionInfo[count];
			break;
		case PermissionEnumKind.Object:
			result = new ObjectPermissionInfo[count];
			break;
		case PermissionEnumKind.Database:
			result = new DatabasePermissionInfo[count];
			break;
		case PermissionEnumKind.Server:
			result = new ServerPermissionInfo[count];
			break;
		}
		return result;
	}

	private static string GetFilter(ServerVersion ver, string granteeName, PermissionSetBase permissions)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (granteeName != null)
		{
			stringBuilder.Append("[@Grantee='" + SqlSmoObject.SqlString(granteeName) + "'");
		}
		if (permissions != null)
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			permissions.AddPermissionFilter(stringBuilder2, ver);
			if (stringBuilder2.Length != 0)
			{
				stringBuilder.Append((stringBuilder.Length > 0) ? " and (" : "[(");
				stringBuilder.Append(stringBuilder2.ToString());
				stringBuilder.Append(")");
			}
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Append("]");
		}
		return stringBuilder.ToString();
	}

	internal static PermissionInfo[] EnumPermissions(PermissionEnumKind kind, SqlSmoObject obj, string granteeName, PermissionSetBase permissions)
	{
		ArrayList arrayList = new ArrayList();
		PermissionInfo.ObjIdent objIdent = null;
		if (obj.IsDesignMode)
		{
			UserPermissionCollection userPermissionCollection = null;
			if (kind == PermissionEnumKind.Column)
			{
				ColumnCollection columnCollection = null;
				switch (obj.GetType().Name)
				{
				case "Table":
					columnCollection = ((Table)obj).Columns;
					break;
				case "View":
					columnCollection = ((View)obj).Columns;
					break;
				case "UserDefinedFunction":
					columnCollection = ((UserDefinedFunction)obj).Columns;
					break;
				}
				foreach (Column item in columnCollection)
				{
					userPermissionCollection = item.GetUserPermissions();
					objIdent = RetrievePermission(userPermissionCollection, kind, item.Name, granteeName, permissions, arrayList, objIdent);
				}
			}
			else
			{
				userPermissionCollection = obj.GetUserPermissions();
				objIdent = RetrievePermission(userPermissionCollection, kind, string.Empty, granteeName, permissions, arrayList, objIdent);
			}
		}
		else
		{
			Request request = new Request();
			if (kind == PermissionEnumKind.Column)
			{
				request.Urn = obj.Urn.Value + "/Column/Permission";
				request.Fields = new string[8] { "Grantee", "Grantor", "PermissionState", "Code", "ObjectClass", "GranteeType", "GrantorType", "ColumnName" };
			}
			else
			{
				request.Urn = obj.Urn.Value + "/Permission";
				request.Fields = new string[7] { "Grantee", "Grantor", "PermissionState", "Code", "ObjectClass", "GranteeType", "GrantorType" };
			}
			request.Urn = request.Urn.Value + GetFilter(obj.ServerVersion, granteeName, permissions);
			IDataReader enumeratorDataReader = obj.ExecutionManager.GetEnumeratorDataReader(request);
			try
			{
				while (enumeratorDataReader.Read())
				{
					if (objIdent == null)
					{
						objIdent = new PermissionInfo.ObjIdent((ObjectClass)enumeratorDataReader.GetInt32(4));
					}
					int @int = enumeratorDataReader.GetInt32(3);
					if (@int >= 0)
					{
						PermissionInfo permissionInfo = GetPermissionInfo(kind);
						permissionInfo.SetPermissionInfoData(enumeratorDataReader.GetString(0), (PrincipalType)enumeratorDataReader.GetInt32(5), enumeratorDataReader.GetString(1), (PrincipalType)enumeratorDataReader.GetInt32(6), (PermissionState)enumeratorDataReader.GetInt32(2), GetPermissionSetBase(kind, @int), (kind == PermissionEnumKind.Column) ? enumeratorDataReader.GetString(7) : null, objIdent);
						arrayList.Add(permissionInfo);
					}
				}
			}
			finally
			{
				enumeratorDataReader.Close();
			}
		}
		objIdent?.SetData(obj);
		PermissionInfo[] permissionInfoArray = GetPermissionInfoArray(kind, arrayList.Count);
		arrayList.CopyTo(permissionInfoArray);
		return permissionInfoArray;
	}

	private static PermissionInfo.ObjIdent RetrievePermission(UserPermissionCollection userPermCollection, PermissionEnumKind kind, string columnName, string granteeName, PermissionSetBase permissions, ArrayList ar, PermissionInfo.ObjIdent objectIdent)
	{
		foreach (UserPermission item in userPermCollection)
		{
			if ((granteeName == null || !(item.Grantee != granteeName)) && (permissions == null || permissions.Storage[(int)item.Code]))
			{
				if (objectIdent == null)
				{
					objectIdent = new PermissionInfo.ObjIdent(item.ObjectClass);
				}
				PermissionInfo permissionInfo = GetPermissionInfo(kind);
				permissionInfo.SetPermissionInfoData(item.Grantee, item.GranteeType, item.Grantor, item.GrantorType, item.PermissionState, GetPermissionSetBase(kind, (int)item.Code), columnName, objectIdent);
				ar.Add(permissionInfo);
			}
		}
		return objectIdent;
	}

	internal static PermissionInfo[] EnumAllPermissions(SqlSmoObject obj, string granteeName, ObjectPermissionSet permissions)
	{
		Request request = new Request();
		request.Urn = obj.Urn.Value + "/LevelPermission" + GetFilter(obj.ServerVersion, granteeName, permissions);
		request.Fields = new string[11]
		{
			"Grantee", "Grantor", "PermissionState", "Code", "ObjectClass", "ColumnName", "ObjectName", "ObjectSchema", "ObjectID", "GranteeType",
			"GrantorType"
		};
		IDataReader enumeratorDataReader = obj.ExecutionManager.GetEnumeratorDataReader(request);
		ArrayList arrayList = new ArrayList();
		try
		{
			while (enumeratorDataReader.Read())
			{
				int @int = enumeratorDataReader.GetInt32(3);
				if (@int >= 0)
				{
					PermissionInfo.ObjIdent objIdent = new PermissionInfo.ObjIdent((ObjectClass)enumeratorDataReader.GetInt32(4), enumeratorDataReader.GetString(6), enumeratorDataReader.GetValue(7) as string, enumeratorDataReader.GetInt32(8));
					ObjectPermissionInfo objectPermissionInfo = new ObjectPermissionInfo();
					objectPermissionInfo.SetPermissionInfoData(enumeratorDataReader.GetString(0), (PrincipalType)enumeratorDataReader.GetInt32(9), enumeratorDataReader.GetString(1), (PrincipalType)enumeratorDataReader.GetInt32(10), (PermissionState)enumeratorDataReader.GetInt32(2), GetPermissionSetBase(PermissionEnumKind.Object, @int), enumeratorDataReader.GetValue(5) as string, objIdent);
					arrayList.Add(objectPermissionInfo);
				}
			}
		}
		finally
		{
			enumeratorDataReader.Close();
		}
		ObjectPermissionInfo[] array = new ObjectPermissionInfo[arrayList.Count];
		arrayList.CopyTo(array);
		return array;
	}

	internal static Urn[] EnumOwnedObjects(SqlSmoObject obj)
	{
		Request request = new Request();
		request.Urn = obj.Urn.Value + "/OwnedObject";
		request.Fields = new string[1] { "Urn" };
		IDataReader enumeratorDataReader = obj.ExecutionManager.GetEnumeratorDataReader(request);
		ArrayList arrayList = new ArrayList();
		try
		{
			while (enumeratorDataReader.Read())
			{
				arrayList.Add((Urn)enumeratorDataReader.GetString(0));
			}
		}
		finally
		{
			enumeratorDataReader.Close();
		}
		Urn[] array = new Urn[arrayList.Count];
		arrayList.CopyTo(array);
		return array;
	}
}
