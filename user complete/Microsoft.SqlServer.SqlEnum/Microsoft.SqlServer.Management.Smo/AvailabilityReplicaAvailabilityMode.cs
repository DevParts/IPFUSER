using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityReplicaAvailabilityModeConverter))]
public enum AvailabilityReplicaAvailabilityMode
{
	[TsqlSyntaxString("ASYNCHRONOUS_COMMIT")]
	[LocDisplayName("aramAsynchronousCommit")]
	AsynchronousCommit = 0,
	[TsqlSyntaxString("SYNCHRONOUS_COMMIT")]
	[LocDisplayName("aramSynchronousCommit")]
	SynchronousCommit = 1,
	[TsqlSyntaxString("CONFIGURATION_ONLY")]
	[LocDisplayName("aramConfigurationOnly")]
	ConfigurationOnly = 4,
	[LocDisplayName("Unknown")]
	[Browsable(false)]
	Unknown = 2
}
