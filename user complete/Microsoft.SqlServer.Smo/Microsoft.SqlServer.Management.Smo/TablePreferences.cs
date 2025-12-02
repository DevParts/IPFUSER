namespace Microsoft.SqlServer.Management.Smo;

internal class TablePreferences
{
	public bool SystemNamesForConstraints { get; set; }

	public bool ConstraintsWithNoCheck { get; set; }

	public bool Identities { get; set; }

	internal TablePreferences()
	{
		Init();
	}

	private void Init()
	{
		Identities = true;
	}

	internal object Clone()
	{
		return MemberwiseClone();
	}
}
