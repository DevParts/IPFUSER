namespace Microsoft.SqlServer.Management.Smo;

public struct DdlTextParserHeaderInfo
{
	public bool scriptForCreate;

	public int indexCreate;

	public bool scriptContainsOrAlter;

	public int indexOrAlterStart;

	public int indexOrAlterEnd;

	public int indexNameStart;

	public int indexNameEnd;

	public string objectType;

	public string schema;

	public string name;

	public string database;

	public string procedureNumber;

	public int indexNameStartSecondary;

	public int indexNameEndSecondary;

	public string schemaSecondary;

	public string nameSecondary;

	public string databaseSecondary;
}
