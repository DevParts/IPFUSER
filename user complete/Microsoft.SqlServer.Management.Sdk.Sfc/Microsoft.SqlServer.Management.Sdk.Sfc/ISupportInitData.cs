using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISupportInitData
{
	void LoadInitData(string file, ServerVersion ver);
}
