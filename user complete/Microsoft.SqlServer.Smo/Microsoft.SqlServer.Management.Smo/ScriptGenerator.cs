using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal delegate void ScriptGenerator(StringCollection stringCollection, ScriptingPreferences sp);
