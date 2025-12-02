namespace Microsoft.SqlServer.Management.Smo;

internal class SecurityPreferences
{
	public bool ExecuteAs { get; set; }

	public bool Sid { get; set; }

	internal SecurityPreferences()
	{
		Init();
	}

	private void Init()
	{
		ExecuteAs = true;
	}

	internal object Clone()
	{
		return MemberwiseClone();
	}
}
