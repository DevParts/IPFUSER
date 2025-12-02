using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class UserDefinedDataTypeResolver
{
	public static object Resolve(object instance, params object[] parameters)
	{
		DataType dataType = SfcResolverHelper.GetDataType(instance);
		if (dataType == null || dataType.SqlDataType != SqlDataType.UserDefinedDataType)
		{
			return null;
		}
		Database database = SfcResolverHelper.GetDatabase(instance);
		return database.UserDefinedDataTypes[dataType.Name, dataType.Schema];
	}

	public static object ResolveUrn(object instance, params object[] parameters)
	{
		DataType dataType = SfcResolverHelper.GetDataType(instance);
		if (dataType == null || dataType.SqlDataType != SqlDataType.UserDefinedDataType)
		{
			return null;
		}
		Database database = SfcResolverHelper.GetDatabase(instance);
		return new Urn(database.Urn.ToString() + string.Format(SmoApplication.DefaultCulture, "/UserDefinedDataType[@Name = '{0}' and @Schema = '{1}']", new object[2]
		{
			SfcSecureString.EscapeSquote(dataType.Name),
			SfcSecureString.EscapeSquote(dataType.Schema)
		}));
	}
}
