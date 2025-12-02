using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

public interface IScriptable
{
	StringCollection Script();

	StringCollection Script(ScriptingOptions scriptingOptions);
}
