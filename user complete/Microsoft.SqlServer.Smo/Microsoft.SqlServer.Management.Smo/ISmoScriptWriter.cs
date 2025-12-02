using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal interface ISmoScriptWriter
{
	string Header { set; }

	void ScriptObject(IEnumerable<string> script, Urn obj);

	void ScriptData(IEnumerable<string> dataScript, Urn table);

	void ScriptContext(string databaseContext, Urn obj);
}
