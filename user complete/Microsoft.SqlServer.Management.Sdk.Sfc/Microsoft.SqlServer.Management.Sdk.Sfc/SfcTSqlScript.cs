using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcTSqlScript : ISfcScript
{
	private StringCollection m_script;

	private SfcTSqlExecutor.ExecutionMode m_executionMode;

	private ExecutionTypes m_executionType;

	public SfcTSqlExecutor.ExecutionMode ExecutionMode
	{
		get
		{
			return m_executionMode;
		}
		set
		{
			m_executionMode = value;
		}
	}

	public ExecutionTypes ExecutionType
	{
		get
		{
			return m_executionType;
		}
		set
		{
			m_executionType = value;
		}
	}

	private void Init()
	{
		m_script = new StringCollection();
		m_executionMode = SfcTSqlExecutor.ExecutionMode.Scalar;
		m_executionType = ExecutionTypes.NoCommands;
	}

	public SfcTSqlScript()
	{
		Init();
	}

	public SfcTSqlScript(string batch)
	{
		Init();
		m_script.Add(batch);
	}

	public void AddBatch(string batch)
	{
		m_script.Add(batch);
	}

	void ISfcScript.Add(ISfcScript otherScript)
	{
		SfcTSqlScript sfcTSqlScript = (SfcTSqlScript)otherScript;
		StringEnumerator enumerator = sfcTSqlScript.GetScript().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				AddBatch(current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	public StringCollection GetScript()
	{
		return m_script;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringEnumerator enumerator = m_script.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				stringBuilder.AppendLine(current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		return stringBuilder.ToString();
	}
}
