using System.Globalization;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessOwnObjects : PostProcess
{
	private string GetDatabaseLevel(DataProvider dp)
	{
		return string.Format(CultureInfo.InvariantCulture, "Server[@Name='{0}']/Database[@Name='{1}']", new object[2]
		{
			Util.EscapeString(GetTriggeredString(dp, 1), '\''),
			Util.EscapeString(GetTriggeredString(dp, 2), '\'')
		});
	}

	private string GetUrn(DataProvider dp, string type, bool bWithSchema, string tentativeParent)
	{
		string text = GetDatabaseLevel(dp);
		if (tentativeParent != null && !IsNull(dp, 5))
		{
			text += string.Format(CultureInfo.InvariantCulture, "/{0}[@Name='{1}' and @Schema='{2}']", new object[3]
			{
				tentativeParent,
				Util.EscapeString(GetTriggeredString(dp, 5), '\''),
				Util.EscapeString(GetTriggeredString(dp, 6), '\'')
			});
		}
		if (!bWithSchema)
		{
			return text + string.Format(CultureInfo.InvariantCulture, "/{0}[@Name='{1}']", new object[2]
			{
				type,
				Util.EscapeString(GetTriggeredString(dp, 3), '\'')
			});
		}
		return text + string.Format(CultureInfo.InvariantCulture, "/{0}[@Name='{1}' and @Schema='{2}']", new object[3]
		{
			type,
			Util.EscapeString(GetTriggeredString(dp, 3), '\''),
			Util.EscapeString(GetTriggeredString(dp, 4), '\'')
		});
	}

	private string GetUrn(DataProvider dp)
	{
		switch (GetTriggeredString(dp, 0))
		{
		case "ASSEMBLY":
			return GetUrn(dp, "SqlAssembly", bWithSchema: false, null);
		case "SCHEMA":
			return GetUrn(dp, "Schema", bWithSchema: false, null);
		case "UDDT":
			return GetUrn(dp, "UserDefinedDataType", bWithSchema: true, null);
		case "UDT":
			return GetUrn(dp, "UserDefinedType", bWithSchema: true, null);
		case "XMLSCHCOL":
			return GetUrn(dp, "XmlSchemaCollection", bWithSchema: true, null);
		case "AF":
			return GetUrn(dp, "UserDefinedAggregate", bWithSchema: true, null);
		case "C ":
			return GetUrn(dp, "Check", bWithSchema: false, "Table");
		case "D ":
			if (!IsNull(dp, 5))
			{
				return GetUrn(dp, "Default", bWithSchema: false, "Table");
			}
			return GetUrn(dp, "Default", bWithSchema: true, null);
		case "F ":
			return GetUrn(dp, "ForeignKey", bWithSchema: false, "Table");
		case "PK":
			return GetUrn(dp, "Index", bWithSchema: false, "Table");
		case "P ":
		case "PC":
		case "RF":
			return GetUrn(dp, "StoredProcedure", bWithSchema: true, null);
		case "FN":
		case "FS":
		case "FT":
		case "IF":
		case "TF":
			return GetUrn(dp, "UserDefinedFunction", bWithSchema: true, null);
		case "R ":
			return GetUrn(dp, "Rule", bWithSchema: true, null);
		case "SN":
			return GetUrn(dp, "Synonym", bWithSchema: true, null);
		case "SO":
			return GetUrn(dp, "Sequence", bWithSchema: true, null);
		case "SQ":
			return GetUrn(dp, "ServiceBroker/ServiceQueue", bWithSchema: true, null);
		case "TA":
		case "TR":
			return GetUrn(dp, "Trigger", bWithSchema: true, "Table");
		case "S ":
		case "U ":
			return GetUrn(dp, "Table", bWithSchema: true, null);
		case "UQ":
			return GetUrn(dp, "Index", bWithSchema: false, "Table");
		case "V ":
			return GetUrn(dp, "View", bWithSchema: true, null);
		case "X ":
			return GetUrn(dp, "ExtendedStoredProcedure", bWithSchema: true, null);
		default:
			throw new InternalEnumeratorException(StringSqlEnumerator.FailedToCreateUrn(GetTriggeredString(dp, 0)));
		}
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		return GetUrn(dp);
	}
}
