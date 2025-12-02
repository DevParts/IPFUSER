using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcCreatable : ICreatable
{
	ISfcScript ScriptCreate();
}
