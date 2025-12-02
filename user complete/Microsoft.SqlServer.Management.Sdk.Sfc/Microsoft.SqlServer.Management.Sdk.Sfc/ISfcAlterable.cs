using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcAlterable : IAlterable
{
	ISfcScript ScriptAlter();
}
