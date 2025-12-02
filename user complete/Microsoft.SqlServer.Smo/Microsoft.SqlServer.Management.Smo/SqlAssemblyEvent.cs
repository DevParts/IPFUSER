namespace Microsoft.SqlServer.Management.Smo;

public sealed class SqlAssemblyEvent
{
	private SqlAssemblyEventValues m_value;

	internal SqlAssemblyEventValues Value => m_value;

	public static SqlAssemblyEvent AlterAssembly => new SqlAssemblyEvent(SqlAssemblyEventValues.AlterAssembly);

	public static SqlAssemblyEvent DropAssembly => new SqlAssemblyEvent(SqlAssemblyEventValues.DropAssembly);

	internal SqlAssemblyEvent(SqlAssemblyEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator SqlAssemblyEventSet(SqlAssemblyEvent eventValue)
	{
		return new SqlAssemblyEventSet(eventValue);
	}

	public static SqlAssemblyEventSet operator +(SqlAssemblyEvent eventLeft, SqlAssemblyEvent eventRight)
	{
		SqlAssemblyEventSet sqlAssemblyEventSet = new SqlAssemblyEventSet(eventLeft);
		sqlAssemblyEventSet.SetBit(eventRight);
		return sqlAssemblyEventSet;
	}

	public static SqlAssemblyEventSet Add(SqlAssemblyEvent eventLeft, SqlAssemblyEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static SqlAssemblyEventSet operator |(SqlAssemblyEvent eventLeft, SqlAssemblyEvent eventRight)
	{
		SqlAssemblyEventSet sqlAssemblyEventSet = new SqlAssemblyEventSet(eventLeft);
		sqlAssemblyEventSet.SetBit(eventRight);
		return sqlAssemblyEventSet;
	}

	public static SqlAssemblyEventSet BitwiseOr(SqlAssemblyEvent eventLeft, SqlAssemblyEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}

	public static bool operator ==(SqlAssemblyEvent a, SqlAssemblyEvent b)
	{
		if ((object)a == null && (object)b == null)
		{
			return true;
		}
		if ((object)a == null || (object)b == null)
		{
			return false;
		}
		return a.m_value == b.m_value;
	}

	public static bool operator !=(SqlAssemblyEvent a, SqlAssemblyEvent b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return this == obj as SqlAssemblyEvent;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
