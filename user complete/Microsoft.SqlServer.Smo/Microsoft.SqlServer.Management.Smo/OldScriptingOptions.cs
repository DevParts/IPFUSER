namespace Microsoft.SqlServer.Management.Smo;

internal class OldScriptingOptions
{
	public bool Bindings { get; set; }

	public bool IncludeDatabaseRoleMemberships { get; set; }

	public bool NoViewColumns { get; set; }

	public bool EnforceScriptingPreferences { get; set; }

	public bool DdlHeaderOnly { get; set; }

	public bool DdlBodyOnly { get; set; }

	public bool NoVardecimal { get; set; }

	public bool IncludeFullTextCatalogRootPath { get; set; }

	public bool PrimaryObject { get; set; }

	public OldScriptingOptions()
	{
		Init();
	}

	private void Init()
	{
		PrimaryObject = true;
		NoVardecimal = true;
	}

	internal object Clone()
	{
		return MemberwiseClone();
	}
}
