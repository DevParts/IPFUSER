using System.Globalization;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

internal class RestorePlanInternal : SqlObject
{
	public override EnumResult GetData(EnumResult erParent)
	{
		SqlEnumResult sqlEnumResult = (SqlEnumResult)erParent;
		sqlEnumResult.StatementBuilder.AddPrefix("DECLARE @db_name              sysname ,@restore_to_datetime  datetime \n");
		ComputeFixedProperties();
		string fixedStringProperty = GetFixedStringProperty("DatabaseName", removeEscape: false);
		string fixedStringProperty2 = GetFixedStringProperty("BackupStartDate", removeEscape: false);
		if (fixedStringProperty == null)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.DatabaseNameMustBeSpecified);
		}
		sqlEnumResult.StatementBuilder.AddPrefix(string.Format(CultureInfo.InvariantCulture, "select @db_name = N'{0}'\n", new object[1] { fixedStringProperty }));
		if (fixedStringProperty2 == null)
		{
			sqlEnumResult.StatementBuilder.AddPrefix("select @restore_to_datetime = GETDATE()\n");
		}
		else
		{
			sqlEnumResult.StatementBuilder.AddPrefix(string.Format(CultureInfo.InvariantCulture, "select @restore_to_datetime = N'{0}'\n", new object[1] { fixedStringProperty2 }));
		}
		base.Filter = null;
		return base.GetData(erParent);
	}
}
