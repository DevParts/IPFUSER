namespace Microsoft.SqlServer.Management.Smo;

public sealed class StoredProcedureEvent
{
	private StoredProcedureEventValues m_value;

	internal StoredProcedureEventValues Value => m_value;

	public static StoredProcedureEvent AlterProcedure => new StoredProcedureEvent(StoredProcedureEventValues.AlterProcedure);

	public static StoredProcedureEvent DropProcedure => new StoredProcedureEvent(StoredProcedureEventValues.DropProcedure);

	internal StoredProcedureEvent(StoredProcedureEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator StoredProcedureEventSet(StoredProcedureEvent eventValue)
	{
		return new StoredProcedureEventSet(eventValue);
	}

	public static StoredProcedureEventSet operator +(StoredProcedureEvent eventLeft, StoredProcedureEvent eventRight)
	{
		StoredProcedureEventSet storedProcedureEventSet = new StoredProcedureEventSet(eventLeft);
		storedProcedureEventSet.SetBit(eventRight);
		return storedProcedureEventSet;
	}

	public static StoredProcedureEventSet Add(StoredProcedureEvent eventLeft, StoredProcedureEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static StoredProcedureEventSet operator |(StoredProcedureEvent eventLeft, StoredProcedureEvent eventRight)
	{
		StoredProcedureEventSet storedProcedureEventSet = new StoredProcedureEventSet(eventLeft);
		storedProcedureEventSet.SetBit(eventRight);
		return storedProcedureEventSet;
	}

	public static StoredProcedureEventSet BitwiseOr(StoredProcedureEvent eventLeft, StoredProcedureEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}

	public static bool operator ==(StoredProcedureEvent a, StoredProcedureEvent b)
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

	public static bool operator !=(StoredProcedureEvent a, StoredProcedureEvent b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return this == obj as StoredProcedureEvent;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
