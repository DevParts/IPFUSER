namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessPermissionCode : PostProcess
{
	private int GetSmoCodeFromSqlCodeYukon(string sqlCode, ObjectClass objClass)
	{
		sqlCode = sqlCode.TrimEnd();
		if (!GetSmoCodeFromSqlCode(sqlCode, out var smoCode))
		{
			if (ObjectClass.Server != objClass && objClass != ObjectClass.Database)
			{
				return (int)PermissionDecode.ToPermissionSetValueEnum<ObjectPermissionSetValue>(sqlCode);
			}
			return -1;
		}
		return smoCode;
	}

	private int GetSmoCodeFromSqlCodeShiloh(string sqlCode)
	{
		sqlCode = sqlCode.TrimEnd();
		if (!GetSmoCodeFromSqlCode(sqlCode, out var smoCode))
		{
			return (int)PermissionDecode.ToPermissionSetValueEnum<ObjectPermissionSetValue>(sqlCode);
		}
		return smoCode;
	}

	private bool GetSmoCodeFromSqlCode(string sqlCode, out int smoCode)
	{
		bool result = false;
		smoCode = -1;
		if ("Permission" == base.ObjectName)
		{
			string type = base.Request.Urn.Parent.Type;
			if ("Server" == type)
			{
				result = true;
				smoCode = (int)PermissionDecode.ToPermissionSetValueEnum<ServerPermissionSetValue>(sqlCode);
			}
			if ("Database" == type)
			{
				result = true;
				smoCode = (int)PermissionDecode.ToPermissionSetValueEnum<DatabasePermissionSetValue>(sqlCode);
			}
		}
		return result;
	}

	private string ShilohToYukonPermission(int permType)
	{
		return permType switch
		{
			26 => "RF", 
			178 => "CRFN", 
			193 => "SL", 
			195 => "IN", 
			196 => "DL", 
			197 => "UP", 
			198 => "CRTB", 
			203 => "CRDB", 
			207 => "CRVW", 
			222 => "CRPR", 
			224 => "EX", 
			228 => "BADB", 
			233 => "CRDF", 
			235 => "BALO", 
			236 => "CRRU", 
			_ => "", 
		};
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (ExecuteSql.GetServerVersion(base.ConnectionInfo).Major < 9)
		{
			return GetSmoCodeFromSqlCodeShiloh(ShilohToYukonPermission(GetTriggeredInt32(dp, 0)));
		}
		return GetSmoCodeFromSqlCodeYukon(GetTriggeredString(dp, 0), (ObjectClass)GetTriggeredInt32(dp, 1));
	}
}
