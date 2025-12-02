using System;

namespace Microsoft.SqlServer.Management.Common;

public class StatementEventArgs : EventArgs
{
	private string m_sqlStatement;

	private DateTime m_timeStamp;

	public string SqlStatement => m_sqlStatement;

	public DateTime TimeStamp => m_timeStamp;

	public StatementEventArgs(string sqlStatement, DateTime timeStamp)
	{
		m_sqlStatement = sqlStatement;
		m_timeStamp = timeStamp;
	}

	public override string ToString()
	{
		return SqlStatement;
	}
}
