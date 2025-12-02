using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SqlServer.Management.Smo;

internal class ScriptStringBuilder
{
	private readonly string statement;

	private readonly IList<IScriptStringBuilderParameter> parameters = new List<IScriptStringBuilderParameter>();

	private readonly ScriptingPreferences scriptingPreferences;

	public ScriptStringBuilder(string baseStatement, ScriptingPreferences scriptingPreferences = null)
	{
		statement = baseStatement;
		this.scriptingPreferences = scriptingPreferences;
	}

	public ScriptStringBuilder SetParameter(string name, string value, ParameterValueFormat format = ParameterValueFormat.CharString)
	{
		return SetParameter(new ScriptStringBuilderParameter(name, value, format));
	}

	public ScriptStringBuilder SetParameter(string name, IList<IScriptStringBuilderParameter> parameters)
	{
		return SetParameter(new ScriptStringBuilderObjectParameter(name, parameters));
	}

	public ScriptStringBuilder SetParameter(IScriptStringBuilderParameter param)
	{
		bool flag = false;
		for (int i = 0; i < parameters.Count; i++)
		{
			if (flag)
			{
				break;
			}
			if (parameters[i].GetKey().Equals(param.GetKey()))
			{
				parameters[i] = param;
				flag = true;
			}
		}
		if (!flag)
		{
			parameters.Add(param);
		}
		return this;
	}

	public override string ToString()
	{
		return ToString(scriptSemiColon: true);
	}

	public string ToString(bool scriptSemiColon, bool pretty = false)
	{
		string text = string.Empty;
		string text2;
		string text3;
		string text4;
		if (pretty && scriptingPreferences != null)
		{
			text2 = scriptingPreferences.NewLine;
			text3 = Globals.tab;
			text4 = Globals.comma;
		}
		else
		{
			text2 = string.Empty;
			text3 = string.Empty;
			text4 = Globals.commaspace;
		}
		if (parameters.Any())
		{
			text = "(" + text2;
			text = text + text3 + string.Join(text4 + text2 + text3, parameters.Select((IScriptStringBuilderParameter p) => p.ToScript()).ToArray());
			text = text + text2 + ")";
		}
		return string.Format("{0}{1}{2}{3}", statement, pretty ? string.Empty : " ", text, scriptSemiColon ? ";" : string.Empty);
	}
}
