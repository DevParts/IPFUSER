using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcTSqlExecutionEngine : ISfcExecutionEngine
{
	private ServerConnection m_connection;

	public SfcTSqlExecutionEngine(ServerConnection connection)
	{
		m_connection = connection;
	}

	object ISfcExecutionEngine.Execute(ISfcScript script)
	{
		SfcTSqlScript sfcTSqlScript = (SfcTSqlScript)script;
		return SfcTSqlExecutor.Execute(m_connection, sfcTSqlScript.ToString(), sfcTSqlScript.ExecutionMode, sfcTSqlScript.ExecutionType);
	}
}
