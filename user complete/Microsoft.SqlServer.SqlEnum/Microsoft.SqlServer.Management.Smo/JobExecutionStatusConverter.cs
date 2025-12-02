using Microsoft.SqlServer.Management.Smo.Agent;

namespace Microsoft.SqlServer.Management.Smo;

internal class JobExecutionStatusConverter : EnumToDisplayNameConverter
{
	public JobExecutionStatusConverter()
		: base(typeof(JobExecutionStatus))
	{
	}
}
