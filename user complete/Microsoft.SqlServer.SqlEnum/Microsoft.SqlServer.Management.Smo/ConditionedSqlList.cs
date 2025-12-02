using System.Collections;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class ConditionedSqlList
{
	internal sealed class ConditionedSqlListEnumerator : IEnumerator
	{
		private IEnumerator baseEnumerator;

		object IEnumerator.Current => baseEnumerator.Current;

		internal ConditionedSqlListEnumerator(IEnumerator enumerator)
		{
			baseEnumerator = enumerator;
		}

		public bool MoveNext()
		{
			return baseEnumerator.MoveNext();
		}

		public void Reset()
		{
			baseEnumerator.Reset();
		}
	}

	private ArrayList m_conditionedSqlList;

	public int Count => m_conditionedSqlList.Count;

	public ConditionedSql this[int i] => (ConditionedSql)m_conditionedSqlList[i];

	public ConditionedSqlList()
	{
		m_conditionedSqlList = new ArrayList();
	}

	public void Add(ConditionedSql obj)
	{
		m_conditionedSqlList.Add(obj);
	}

	public void ClearHits()
	{
		foreach (ConditionedSql conditionedSql in m_conditionedSqlList)
		{
			conditionedSql.ClearHit();
		}
	}

	public bool AddHits(SqlObjectBase obj, string field, StatementBuilder sb)
	{
		bool result = false;
		foreach (ConditionedSql conditionedSql in m_conditionedSqlList)
		{
			if (conditionedSql.IsHit(field))
			{
				result = true;
				conditionedSql.MarkHit();
				conditionedSql.AddHit(field, obj, sb);
			}
		}
		return result;
	}

	public void AddDefault(StatementBuilder sb)
	{
		foreach (ConditionedSql conditionedSql in m_conditionedSqlList)
		{
			if (conditionedSql.IsDefault())
			{
				conditionedSql.MarkHit();
			}
		}
	}

	public IEnumerator GetEnumerator()
	{
		return new ConditionedSqlListEnumerator(m_conditionedSqlList.GetEnumerator());
	}
}
