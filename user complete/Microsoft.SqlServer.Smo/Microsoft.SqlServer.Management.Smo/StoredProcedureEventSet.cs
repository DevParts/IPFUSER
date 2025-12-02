using System.Collections;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class StoredProcedureEventSet : EventSetBase
{
	public override int NumberOfElements => 2;

	public bool AlterProcedure
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

	public bool DropProcedure
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

	public StoredProcedureEventSet()
	{
	}

	public StoredProcedureEventSet(StoredProcedureEventSet eventSet)
		: base(eventSet)
	{
	}

	public StoredProcedureEventSet(StoredProcedureEvent anEvent)
	{
		SetBit(anEvent);
	}

	public StoredProcedureEventSet(params StoredProcedureEvent[] events)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (StoredProcedureEvent bit in events)
		{
			SetBit(bit);
		}
	}

	public override EventSetBase Copy()
	{
		return new StoredProcedureEventSet(base.Storage);
	}

	internal StoredProcedureEventSet(BitArray storage)
	{
		base.Storage = (BitArray)storage.Clone();
	}

	internal void SetBit(StoredProcedureEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = true;
	}

	internal void ResetBit(StoredProcedureEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = false;
	}

	public StoredProcedureEventSet Add(StoredProcedureEvent anEvent)
	{
		SetBit(anEvent);
		return this;
	}

	public StoredProcedureEventSet Remove(StoredProcedureEvent anEvent)
	{
		ResetBit(anEvent);
		return this;
	}

	public static StoredProcedureEventSet operator +(StoredProcedureEventSet eventSet, StoredProcedureEvent anEvent)
	{
		StoredProcedureEventSet storedProcedureEventSet = new StoredProcedureEventSet(eventSet);
		storedProcedureEventSet.SetBit(anEvent);
		return storedProcedureEventSet;
	}

	public static StoredProcedureEventSet Add(StoredProcedureEventSet eventSet, StoredProcedureEvent anEvent)
	{
		return eventSet + anEvent;
	}

	public static StoredProcedureEventSet operator -(StoredProcedureEventSet eventSet, StoredProcedureEvent anEvent)
	{
		StoredProcedureEventSet storedProcedureEventSet = new StoredProcedureEventSet(eventSet);
		storedProcedureEventSet.ResetBit(anEvent);
		return storedProcedureEventSet;
	}

	public static StoredProcedureEventSet Subtract(StoredProcedureEventSet eventSet, StoredProcedureEvent anEvent)
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
				stringBuilder.Append(((StoredProcedureEventValues)num).ToString());
			}
			num++;
		}
		return stringBuilder.ToString();
	}
}
