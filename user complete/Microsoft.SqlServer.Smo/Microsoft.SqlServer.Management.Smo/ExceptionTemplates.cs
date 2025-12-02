namespace Microsoft.SqlServer.Management.Smo;

internal class ExceptionTemplates : ExceptionTemplatesImpl
{
	public new static string IncludeHeader(string objectType, string name, string dateString)
	{
		string name2 = (name.Contains("*/") ? "?" : ((!name.Contains("/*")) ? name : "?"));
		return ExceptionTemplatesImpl.IncludeHeader(objectType, name2, dateString);
	}
}
