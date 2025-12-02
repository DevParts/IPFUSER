using System.Collections;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServiceQueueEventSet : EventSetBase
{
	public override int NumberOfElements => 4;

	public bool AlterQueue
	{
		get
		{
			return base.Storage[0];
		}
		set
		{
			base.Storage[0] = value;
		}
	}

	public bool BrokerQueueDisabled
	{
		get
		{
			return base.Storage[1];
		}
		set
		{
			base.Storage[1] = value;
		}
	}

	public bool DropQueue
	{
		get
		{
			return base.Storage[2];
		}
		set
		{
			base.Storage[2] = value;
		}
	}

	public bool QueueActivation
	{
		get
		{
			return base.Storage[3];
		}
		set
		{
			base.Storage[3] = value;
		}
	}

	public ServiceQueueEventSet()
	{
	}

	public ServiceQueueEventSet(ServiceQueueEventSet eventSet)
		: base(eventSet)
	{
	}

	public ServiceQueueEventSet(ServiceQueueEvent anEvent)
	{
		SetBit(anEvent);
	}

	public ServiceQueueEventSet(params ServiceQueueEvent[] events)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (ServiceQueueEvent bit in events)
		{
			SetBit(bit);
		}
	}

	public override EventSetBase Copy()
	{
		return new ServiceQueueEventSet(base.Storage);
	}

	internal ServiceQueueEventSet(BitArray storage)
	{
		base.Storage = (BitArray)storage.Clone();
	}

	internal void SetBit(ServiceQueueEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = true;
	}

	internal void ResetBit(ServiceQueueEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = false;
	}

	public ServiceQueueEventSet Add(ServiceQueueEvent anEvent)
	{
		SetBit(anEvent);
		return this;
	}

	public ServiceQueueEventSet Remove(ServiceQueueEvent anEvent)
	{
		ResetBit(anEvent);
		return this;
	}

	public static ServiceQueueEventSet operator +(ServiceQueueEventSet eventSet, ServiceQueueEvent anEvent)
	{
		ServiceQueueEventSet serviceQueueEventSet = new ServiceQueueEventSet(eventSet);
		serviceQueueEventSet.SetBit(anEvent);
		return serviceQueueEventSet;
	}

	public static ServiceQueueEventSet Add(ServiceQueueEventSet eventSet, ServiceQueueEvent anEvent)
	{
		return eventSet + anEvent;
	}

	public static ServiceQueueEventSet operator -(ServiceQueueEventSet eventSet, ServiceQueueEvent anEvent)
	{
		ServiceQueueEventSet serviceQueueEventSet = new ServiceQueueEventSet(eventSet);
		serviceQueueEventSet.ResetBit(anEvent);
		return serviceQueueEventSet;
	}

	public static ServiceQueueEventSet Subtract(ServiceQueueEventSet eventSet, ServiceQueueEvent anEvent)
	{
		return eventSet - anEvent;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetType().Name + ": ");
		int num = 0;
		bool flag = true;
		foreach (bool item in base.Storage)
		{
			if (item)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(((ServiceQueueEventValues)num).ToString());
			}
			num++;
		}
		return stringBuilder.ToString();
	}
}
