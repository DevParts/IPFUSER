namespace Microsoft.SqlServer.Management.Smo;

internal class MailPreferences
{
	public bool Accounts { get; set; }

	public bool Principals { get; set; }

	internal MailPreferences()
	{
		Init();
	}

	private void Init()
	{
		Accounts = true;
		Principals = true;
	}

	internal object Clone()
	{
		return MemberwiseClone();
	}
}
