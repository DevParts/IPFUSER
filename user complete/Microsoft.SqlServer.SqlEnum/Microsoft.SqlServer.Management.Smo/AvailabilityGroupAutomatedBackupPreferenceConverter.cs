namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityGroupAutomatedBackupPreferenceConverter : EnumToDisplayNameConverter
{
	public AvailabilityGroupAutomatedBackupPreferenceConverter()
		: base(typeof(AvailabilityGroupAutomatedBackupPreference))
	{
	}
}
