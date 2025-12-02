using System;
using System.Globalization;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessRemoteDataArchiveDatabaseProperties : PostProcessWithRowCaching
{
	private const string remoteDataArchiveEndpoint = "RemoteDataArchiveEndpoint";

	private const string remoteDataArchiveDatabaseName = "RemoteDatabaseName";

	private const string remoteDataArchiveLinkedServer = "RemoteDataArchiveLinkedServer";

	private const string remoteDataArchiveFederatedServiceAccount = "RemoteDataArchiveUseFederatedServiceAccount";

	private const string remoteDataArchiveCredential = "RemoteDataArchiveCredential";

	protected override string SqlQuery
	{
		get
		{
			StatementBuilder statementBuilder = new StatementBuilder();
			statementBuilder.AddProperty("RemoteDataArchiveEndpoint", "eds.location");
			statementBuilder.AddProperty("RemoteDataArchiveLinkedServer", "eds.name");
			statementBuilder.AddProperty("RemoteDatabaseName", "rdad.remote_database_name");
			statementBuilder.AddProperty("RemoteDataArchiveUseFederatedServiceAccount", "rdad.federated_service_account");
			statementBuilder.AddProperty("RemoteDataArchiveCredential", "case when rdad.federated_service_account = 1 then null else cred.name end");
			statementBuilder.AddFrom("sys.remote_data_archive_databases rdad");
			statementBuilder.AddJoin("INNER JOIN sys.external_data_sources eds ON rdad.data_source_id = eds.data_source_id");
			statementBuilder.AddJoin("LEFT OUTER JOIN sys.database_scoped_credentials cred ON eds.credential_id = cred.credential_id");
			return statementBuilder.SqlStatement;
		}
	}

	private bool IsStretchSmoSupportedOnVersion(Version sqlServerVersion)
	{
		if (sqlServerVersion < new Version(13, 0, 700))
		{
			return false;
		}
		return true;
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (!string.IsNullOrEmpty(name))
		{
			ServerVersion serverVersion = ExecuteSql.GetServerVersion(base.ConnectionInfo);
			if (IsStretchSmoSupportedOnVersion(new Version(serverVersion.Major, serverVersion.Minor, serverVersion.BuildNumber)) && (name.Equals("RemoteDataArchiveEndpoint", StringComparison.InvariantCultureIgnoreCase) || name.Equals("RemoteDatabaseName", StringComparison.InvariantCultureIgnoreCase) || name.Equals("RemoteDataArchiveLinkedServer", StringComparison.InvariantCultureIgnoreCase) || name.Equals("RemoteDataArchiveUseFederatedServiceAccount", StringComparison.InvariantCultureIgnoreCase) || name.Equals("RemoteDataArchiveCredential", StringComparison.InvariantCultureIgnoreCase)))
			{
				GetCachedRowResultsForDatabase(dp, GetTriggeredString(dp, 0));
			}
			data = DBNull.Value;
			switch (name)
			{
			case "RemoteDataArchiveEndpoint":
			case "RemoteDataArchiveLinkedServer":
			case "RemoteDatabaseName":
			case "RemoteDataArchiveUseFederatedServiceAccount":
			case "RemoteDataArchiveCredential":
				if (rowResults != null && rowResults.Count > 0)
				{
					data = rowResults[0][name];
				}
				break;
			default:
				TraceHelper.Assert(condition: false, string.Format(CultureInfo.InvariantCulture, "PostProcessRemoteDataArchiveDatabaseProperties - Unknown property {0}", new object[1] { name }));
				break;
			}
			return data;
		}
		TraceHelper.Assert(condition: false, string.Format(CultureInfo.InvariantCulture, "PostProcessRemoteDataArchiveDatabaseProperties - Column name is null"));
		return null;
	}
}
