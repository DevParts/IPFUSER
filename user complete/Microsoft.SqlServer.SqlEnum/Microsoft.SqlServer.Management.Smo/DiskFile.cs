using System.Globalization;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

internal class DiskFile : SqlObject
{
	public override EnumResult GetData(EnumResult erParent)
	{
		SqlEnumResult sqlEnumResult = (SqlEnumResult)erParent;
		sqlEnumResult.StatementBuilder.AddPrefix("declare @Path nvarchar(255)\ndeclare @Name nvarchar(255)\n");
		ComputeFixedProperties();
		string fixedStringProperty = GetFixedStringProperty("Path", removeEscape: false);
		string fixedStringProperty2 = GetFixedStringProperty("FullName", removeEscape: false);
		if ((fixedStringProperty == null && fixedStringProperty2 == null) || (fixedStringProperty != null && fixedStringProperty2 != null))
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.OnlyPathOrFullName);
		}
		if (fixedStringProperty != null)
		{
			sqlEnumResult.StatementBuilder.AddPrefix(string.Format(CultureInfo.InvariantCulture, "select @Path = N'{0}'\n", new object[1] { fixedStringProperty }));
		}
		else
		{
			sqlEnumResult.StatementBuilder.AddPrefix("select @Path = null;");
		}
		if (fixedStringProperty2 != null)
		{
			sqlEnumResult.StatementBuilder.AddPrefix(string.Format(CultureInfo.InvariantCulture, "select @Name = N'{0}'\n", new object[1] { fixedStringProperty2 }));
		}
		else
		{
			sqlEnumResult.StatementBuilder.AddPrefix("select @Name = null;");
		}
		base.Filter = null;
		return base.GetData(erParent);
	}
}
