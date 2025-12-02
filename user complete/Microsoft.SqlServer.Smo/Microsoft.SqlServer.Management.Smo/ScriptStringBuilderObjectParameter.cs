using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SqlServer.Management.Smo;

internal class ScriptStringBuilderObjectParameter : IScriptStringBuilderParameter
{
	private string key { get; set; }

	private IList<IScriptStringBuilderParameter> parameters { get; set; }

	public ScriptStringBuilderObjectParameter(string key, IList<IScriptStringBuilderParameter> parameters)
	{
		this.key = key;
		this.parameters = parameters;
	}

	public string GetKey()
	{
		return key;
	}

	public string ToScript()
	{
		return string.Format("{0} {1}", key, parameters.Any() ? string.Format("({0})", string.Join(", ", parameters.Select((IScriptStringBuilderParameter p) => p.ToScript()))) : string.Empty);
	}
}
