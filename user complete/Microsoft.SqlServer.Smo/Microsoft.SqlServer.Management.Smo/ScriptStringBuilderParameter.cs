namespace Microsoft.SqlServer.Management.Smo;

internal class ScriptStringBuilderParameter : IScriptStringBuilderParameter
{
	private readonly string key;

	private readonly string value;

	private readonly ParameterValueFormat format;

	public ScriptStringBuilderParameter(string key, string value, ParameterValueFormat format = ParameterValueFormat.CharString)
	{
		this.key = key;
		this.value = value;
		this.format = format;
	}

	public string GetKey()
	{
		return key;
	}

	public string ToScript()
	{
		string arg = value;
		switch (format)
		{
		case ParameterValueFormat.CharString:
			arg = "'" + value + "'";
			break;
		case ParameterValueFormat.NVarCharString:
			arg = "N'" + value + "'";
			break;
		}
		return $"{key} = {arg}";
	}
}
