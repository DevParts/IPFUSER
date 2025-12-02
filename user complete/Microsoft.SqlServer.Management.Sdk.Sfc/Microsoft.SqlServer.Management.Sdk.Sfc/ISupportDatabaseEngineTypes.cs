using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISupportDatabaseEngineTypes
{
	DatabaseEngineType GetDatabaseEngineType(object conn);
}
