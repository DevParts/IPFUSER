using System;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Common;

public sealed class CapturedSql
{
	private StringCollection m_CapturedSql;

	public StringCollection Text
	{
		get
		{
			StringCollection stringCollection = new StringCollection();
			StringEnumerator enumerator = m_CapturedSql.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					stringCollection.Add(current);
				}
				return stringCollection;
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
	}

	internal CapturedSql()
	{
		m_CapturedSql = new StringCollection();
	}

	public void Add(string sqlStatement)
	{
		m_CapturedSql.Add(sqlStatement);
	}

	public void Clear()
	{
		m_CapturedSql.Clear();
	}
}
