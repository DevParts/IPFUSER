using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SmoSimpleNodeAdapter : SimpleNodeAdapter
{
	private static readonly IList<string> SYSTEM_SCHEMA_NAMES = new List<string>(new string[13]
	{
		"dbo", "guest", "INFORMATION_SCHEMA", "sys", "db_owner", "db_accessadmin", "db_securityadmin", "db_ddladmin", "db_backupoperator", "db_datareader",
		"db_datawriter", "db_denydatareader", "db_denydatawriter"
	});

	private static readonly IList<string> SYSTEM_USER_NAMES = new List<string>(new string[4] { "dbo", "guest", "INFORMATION_SCHEMA", "sys" });

	public override bool IsSupported(object reference)
	{
		return reference is SqlSmoObject;
	}

	public override Urn GetUrn(object reference)
	{
		SqlSmoObject sqlSmoObject = (SqlSmoObject)reference;
		return sqlSmoObject.Urn;
	}

	public override object GetProperty(object reference, string propertyName)
	{
		SqlSmoObject sqlSmoObject = (SqlSmoObject)reference;
		return sqlSmoObject.Properties[propertyName].Value;
	}

	public override bool IsCriteriaMatched(object reference)
	{
		if (!(reference is SqlSmoObject))
		{
			return base.IsCriteriaMatched(reference);
		}
		SqlSmoObject sqlSmoObject = (SqlSmoObject)reference;
		if (sqlSmoObject.Properties.Contains("IsSystemObject") && (bool)sqlSmoObject.Properties["IsSystemObject"].Value)
		{
			return false;
		}
		if (sqlSmoObject is DatabaseRole)
		{
			DatabaseRole databaseRole = (DatabaseRole)sqlSmoObject;
			if (databaseRole.IsFixedRole)
			{
				return false;
			}
			if (System.StringComparer.Ordinal.Compare("public", databaseRole.Name) == 0)
			{
				return false;
			}
		}
		else if (sqlSmoObject is User)
		{
			if (IsDesignModeSystemUser((User)sqlSmoObject))
			{
				return false;
			}
		}
		else if (sqlSmoObject is Schema && IsDesignModeSystemSchema((Schema)sqlSmoObject))
		{
			return false;
		}
		return true;
	}

	private bool IsDesignModeSystemSchema(Schema schema)
	{
		if (SYSTEM_SCHEMA_NAMES.Contains(schema.Name) && schema.IsDesignMode)
		{
			return true;
		}
		return false;
	}

	private bool IsDesignModeSystemUser(User user)
	{
		if (SYSTEM_USER_NAMES.Contains(user.Name) && user.IsDesignMode)
		{
			return true;
		}
		return false;
	}
}
