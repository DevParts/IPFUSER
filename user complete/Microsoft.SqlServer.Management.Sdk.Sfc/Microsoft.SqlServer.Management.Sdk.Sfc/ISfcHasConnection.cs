using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcHasConnection
{
	SfcConnectionContext ConnectionContext { get; }

	ISfcConnection GetConnection();

	void SetConnection(ISfcConnection connection);

	ISfcConnection GetConnection(SfcObjectQueryMode activeQueriesMode);
}
