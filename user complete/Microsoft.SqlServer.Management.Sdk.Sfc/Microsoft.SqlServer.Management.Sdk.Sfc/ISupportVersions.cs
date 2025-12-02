using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISupportVersions
{
	ServerVersion GetServerVersion(object conn);
}
