namespace Microsoft.SqlServer.Management.Smo;

public interface ITextObject
{
	string TextBody { get; set; }

	string TextHeader { get; set; }

	bool TextMode { get; set; }

	string ScriptHeader(bool forAlter);

	string ScriptHeader(ScriptNameObjectBase.ScriptHeaderType scriptHeaderType);
}
