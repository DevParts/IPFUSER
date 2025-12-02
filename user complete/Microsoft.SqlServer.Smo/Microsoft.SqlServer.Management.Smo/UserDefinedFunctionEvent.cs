namespace Microsoft.SqlServer.Management.Smo;

public sealed class UserDefinedFunctionEvent
{
	private UserDefinedFunctionEventValues m_value;

	internal UserDefinedFunctionEventValues Value => m_value;

	public static UserDefinedFunctionEvent AlterFunction => new UserDefinedFunctionEvent(UserDefinedFunctionEventValues.AlterFunction);

	public static UserDefinedFunctionEvent DropFunction => new UserDefinedFunctionEvent(UserDefinedFunctionEventValues.DropFunction);

	internal UserDefinedFunctionEvent(UserDefinedFunctionEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator UserDefinedFunctionEventSet(UserDefinedFunctionEvent eventValue)
	{
		return new UserDefinedFunctionEventSet(eventValue);
	}

	public static UserDefinedFunctionEventSet operator +(UserDefinedFunctionEvent eventLeft, UserDefinedFunctionEvent eventRight)
	{
		UserDefinedFunctionEventSet userDefinedFunctionEventSet = new UserDefinedFunctionEventSet(eventLeft);
		userDefinedFunctionEventSet.SetBit(eventRight);
		return userDefinedFunctionEventSet;
	}

	public static UserDefinedFunctionEventSet Add(UserDefinedFunctionEvent eventLeft, UserDefinedFunctionEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static UserDefinedFunctionEventSet operator |(UserDefinedFunctionEvent eventLeft, UserDefinedFunctionEvent eventRight)
	{
		UserDefinedFunctionEventSet userDefinedFunctionEventSet = new UserDefinedFunctionEventSet(eventLeft);
		userDefinedFunctionEventSet.SetBit(eventRight);
		return userDefinedFunctionEventSet;
	}

	public static UserDefinedFunctionEventSet BitwiseOr(UserDefinedFunctionEvent eventLeft, UserDefinedFunctionEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}

	public static bool operator ==(UserDefinedFunctionEvent a, UserDefinedFunctionEvent b)
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

	public static bool operator !=(UserDefinedFunctionEvent a, UserDefinedFunctionEvent b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return this == obj as UserDefinedFunctionEvent;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
