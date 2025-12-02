using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class ProviderUrnPrefixFactory
{
	private const string SQL_PROVIDER_URN_PREFIX = "Sql";

	internal static string GetProviderUrnPrefix(string rootLevel, object ci)
	{
		if (HasProviders(rootLevel))
		{
			if (IsSqlConnection(ci))
			{
				return "Sql";
			}
			throw new InternalEnumeratorException(SfcStrings.InvalidConnectionType);
		}
		return string.Empty;
	}

	private static bool IsSqlConnection(object ci)
	{
		if (ci is ServerConnection || ci is SqlConnectionInfoWithConnection || ci is SqlConnectionInfo || ci is SqlConnection || ci is SqlDirectConnection)
		{
			return true;
		}
		return false;
	}

	private static bool HasProviders(string rootLevel)
	{
		if (rootLevel == "XEStore")
		{
			return true;
		}
		return false;
	}
}
