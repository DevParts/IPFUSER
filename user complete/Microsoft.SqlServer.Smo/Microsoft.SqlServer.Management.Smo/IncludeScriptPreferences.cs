namespace Microsoft.SqlServer.Management.Smo;

internal class IncludeScriptPreferences
{
	public bool Data { get; set; }

	public bool Permissions { get; set; }

	public bool ExistenceCheck { get; set; }

	public bool Header { get; set; }

	public bool ScriptingParameterHeader { get; set; }

	public bool SchemaQualify { get; set; }

	internal bool SchemaQualifyForeignKeysReferences { get; set; }

	internal bool ExtendedProperties { get; set; }

	public bool Collation { get; set; }

	public bool Owner { get; set; }

	public bool DatabaseContext { get; set; }

	public bool Associations { get; set; }

	public bool AnsiPadding { get; set; }

	public bool Ddl { get; set; }

	internal bool CreateDdlTriggerDisabled { get; set; }

	internal IncludeScriptPreferences()
	{
		Init();
	}

	private void Init()
	{
		SchemaQualify = true;
		Ddl = true;
		Collation = true;
		ScriptingParameterHeader = false;
	}

	internal object Clone()
	{
		return MemberwiseClone();
	}
}
