namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServiceQueueEvent
{
	private ServiceQueueEventValues m_value;

	internal ServiceQueueEventValues Value => m_value;

	public static ServiceQueueEvent AlterQueue => new ServiceQueueEvent(ServiceQueueEventValues.AlterQueue);

	public static ServiceQueueEvent BrokerQueueDisabled => new ServiceQueueEvent(ServiceQueueEventValues.BrokerQueueDisabled);

	public static ServiceQueueEvent DropQueue => new ServiceQueueEvent(ServiceQueueEventValues.DropQueue);

	public static ServiceQueueEvent QueueActivation => new ServiceQueueEvent(ServiceQueueEventValues.QueueActivation);

	internal ServiceQueueEvent(ServiceQueueEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator ServiceQueueEventSet(ServiceQueueEvent eventValue)
	{
		return new ServiceQueueEventSet(eventValue);
	}

	public static ServiceQueueEventSet operator +(ServiceQueueEvent eventLeft, ServiceQueueEvent eventRight)
	{
		ServiceQueueEventSet serviceQueueEventSet = new ServiceQueueEventSet(eventLeft);
		serviceQueueEventSet.SetBit(eventRight);
		return serviceQueueEventSet;
	}

	public static ServiceQueueEventSet Add(ServiceQueueEvent eventLeft, ServiceQueueEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static ServiceQueueEventSet operator |(ServiceQueueEvent eventLeft, ServiceQueueEvent eventRight)
	{
		ServiceQueueEventSet serviceQueueEventSet = new ServiceQueueEventSet(eventLeft);
		serviceQueueEventSet.SetBit(eventRight);
		return serviceQueueEventSet;
	}

	public static ServiceQueueEventSet BitwiseOr(ServiceQueueEvent eventLeft, ServiceQueueEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}

	public static bool operator ==(ServiceQueueEvent a, ServiceQueueEvent b)
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

	public static bool operator !=(ServiceQueueEvent a, ServiceQueueEvent b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return this == obj as ServiceQueueEvent;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
