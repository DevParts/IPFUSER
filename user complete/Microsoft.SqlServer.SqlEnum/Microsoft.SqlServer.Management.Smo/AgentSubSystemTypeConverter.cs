using Microsoft.SqlServer.Management.Smo.Agent;

namespace Microsoft.SqlServer.Management.Smo;

public class AgentSubSystemTypeConverter : EnumToDisplayNameConverter
{
	public AgentSubSystemTypeConverter()
		: base(typeof(AgentSubSystem))
	{
	}
}
