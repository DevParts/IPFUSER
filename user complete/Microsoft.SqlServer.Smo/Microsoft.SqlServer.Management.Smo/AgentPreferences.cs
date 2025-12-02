namespace Microsoft.SqlServer.Management.Smo;

internal class AgentPreferences
{
	public bool AlertJob { get; set; }

	public bool JobId { get; set; }

	public bool Notify { get; set; }

	internal bool InScriptJob { get; set; }

	internal AgentPreferences()
	{
		Init();
	}

	private void Init()
	{
		JobId = true;
	}

	internal object Clone()
	{
		return MemberwiseClone();
	}
}
