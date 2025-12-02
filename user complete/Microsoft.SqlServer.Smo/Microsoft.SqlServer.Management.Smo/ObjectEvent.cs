namespace Microsoft.SqlServer.Management.Smo;

public sealed class ObjectEvent
{
	private ObjectEventValues m_value;

	internal ObjectEventValues Value => m_value;

	public static ObjectEvent Alter => new ObjectEvent(ObjectEventValues.Alter);

	public static ObjectEvent Drop => new ObjectEvent(ObjectEventValues.Drop);

	internal ObjectEvent(ObjectEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator ObjectEventSet(ObjectEvent eventValue)
	{
		return new ObjectEventSet(eventValue);
	}

	public static ObjectEventSet operator +(ObjectEvent eventLeft, ObjectEvent eventRight)
	{
		ObjectEventSet objectEventSet = new ObjectEventSet(eventLeft);
		objectEventSet.SetBit(eventRight);
		return objectEventSet;
	}

	public static ObjectEventSet Add(ObjectEvent eventLeft, ObjectEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static ObjectEventSet operator |(ObjectEvent eventLeft, ObjectEvent eventRight)
	{
		ObjectEventSet objectEventSet = new ObjectEventSet(eventLeft);
		objectEventSet.SetBit(eventRight);
		return objectEventSet;
	}

	public static ObjectEventSet BitwiseOr(ObjectEvent eventLeft, ObjectEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}

	public static bool operator ==(ObjectEvent a, ObjectEvent b)
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

	public static bool operator !=(ObjectEvent a, ObjectEvent b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return this == obj as ObjectEvent;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
