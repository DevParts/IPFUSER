using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISupportInitDatabaseEngineData
{
	void LoadInitData(string file, ServerVersion ver, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition);
}
