using System.Data;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal interface ISqlSmoObjectInitialize
{
	void InitializeFromDataReader(IDataReader reader);
}
