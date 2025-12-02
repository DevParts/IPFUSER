using System.Globalization;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

internal class PrimaryFile : SqlObject
{
	public override EnumResult GetData(EnumResult erParent)
	{
		SqlEnumResult sqlEnumResult = (SqlEnumResult)erParent;
		sqlEnumResult.StatementBuilder.AddPrefix("declare @Path nvarchar(255)\ndeclare @Name nvarchar(255)\n");
		ComputeFixedProperties();
		string fixedStringProperty = GetFixedStringProperty("Name", removeEscape: false);
		if (fixedStringProperty == null)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.PropMustBeSpecified("Name", "PrimaryFile"));
		}
		sqlEnumResult.StatementBuilder.AddPrefix(string.Format(CultureInfo.InvariantCulture, "declare @fileName nvarchar(255)\nselect @fileName = N'{0}'\n", new object[1] { Util.EscapeString(fixedStringProperty, '\'') }));
		base.Filter = null;
		return base.GetData(erParent);
	}
}
