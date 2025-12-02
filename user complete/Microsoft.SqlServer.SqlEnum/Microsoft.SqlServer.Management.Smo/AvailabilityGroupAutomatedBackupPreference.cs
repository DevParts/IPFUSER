using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityGroupAutomatedBackupPreferenceConverter))]
public enum AvailabilityGroupAutomatedBackupPreference
{
	[LocDisplayName("agabpPrimary")]
	Primary,
	[LocDisplayName("agabpSecondaryOnly")]
	SecondaryOnly,
	[LocDisplayName("agabpSecondary")]
	Secondary,
	[LocDisplayName("agabpNone")]
	None,
	[LocDisplayName("Unknown")]
	[Browsable(false)]
	Unknown
}
