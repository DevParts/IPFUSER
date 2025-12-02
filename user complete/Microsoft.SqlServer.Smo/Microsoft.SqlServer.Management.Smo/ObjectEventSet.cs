using System.Collections;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ObjectEventSet : EventSetBase
{
	public override int NumberOfElements => 2;

	public bool Alter
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

	public bool Drop
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

	public ObjectEventSet()
	{
	}

	public ObjectEventSet(ObjectEventSet eventSet)
		: base(eventSet)
	{
	}

	public ObjectEventSet(ObjectEvent anEvent)
	{
		SetBit(anEvent);
	}

	public ObjectEventSet(params ObjectEvent[] events)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (ObjectEvent bit in events)
		{
			SetBit(bit);
		}
	}

	public override EventSetBase Copy()
	{
		return new ObjectEventSet(base.Storage);
	}

	internal ObjectEventSet(BitArray storage)
	{
		base.Storage = (BitArray)storage.Clone();
	}

	internal void SetBit(ObjectEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = true;
	}

	internal void ResetBit(ObjectEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = false;
	}

	public ObjectEventSet Add(ObjectEvent anEvent)
	{
		SetBit(anEvent);
		return this;
	}

	public ObjectEventSet Remove(ObjectEvent anEvent)
	{
		ResetBit(anEvent);
		return this;
	}

	public static ObjectEventSet operator +(ObjectEventSet eventSet, ObjectEvent anEvent)
	{
		ObjectEventSet objectEventSet = new ObjectEventSet(eventSet);
		objectEventSet.SetBit(anEvent);
		return objectEventSet;
	}

	public static ObjectEventSet Add(ObjectEventSet eventSet, ObjectEvent anEvent)
	{
		return eventSet + anEvent;
	}

	public static ObjectEventSet operator -(ObjectEventSet eventSet, ObjectEvent anEvent)
	{
		ObjectEventSet objectEventSet = new ObjectEventSet(eventSet);
		objectEventSet.ResetBit(anEvent);
		return objectEventSet;
	}

	public static ObjectEventSet Subtract(ObjectEventSet eventSet, ObjectEvent anEvent)
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
				stringBuilder.Append(((ObjectEventValues)num).ToString());
			}
			num++;
		}
		return stringBuilder.ToString();
	}
}
