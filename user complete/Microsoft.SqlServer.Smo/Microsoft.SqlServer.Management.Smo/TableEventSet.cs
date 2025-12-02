using System.Collections;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class TableEventSet : EventSetBase
{
	public override int NumberOfElements => 13;

	public bool AlterFulltextIndex
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

	public bool AlterIndex
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

	public bool AlterTable
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

	public bool CreateFulltextIndex
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

	public bool CreateIndex
	{
		get
		{
			return base.Storage[4];
		}
		set
		{
			base.Storage[4] = value;
		}
	}

	public bool CreateSpatialIndex
	{
		get
		{
			return base.Storage[5];
		}
		set
		{
			base.Storage[5] = value;
		}
	}

	public bool CreateStatistics
	{
		get
		{
			return base.Storage[6];
		}
		set
		{
			base.Storage[6] = value;
		}
	}

	public bool CreateXmlIndex
	{
		get
		{
			return base.Storage[7];
		}
		set
		{
			base.Storage[7] = value;
		}
	}

	public bool DropFulltextIndex
	{
		get
		{
			return base.Storage[8];
		}
		set
		{
			base.Storage[8] = value;
		}
	}

	public bool DropIndex
	{
		get
		{
			return base.Storage[9];
		}
		set
		{
			base.Storage[9] = value;
		}
	}

	public bool DropStatistics
	{
		get
		{
			return base.Storage[10];
		}
		set
		{
			base.Storage[10] = value;
		}
	}

	public bool DropTable
	{
		get
		{
			return base.Storage[11];
		}
		set
		{
			base.Storage[11] = value;
		}
	}

	public bool UpdateStatistics
	{
		get
		{
			return base.Storage[12];
		}
		set
		{
			base.Storage[12] = value;
		}
	}

	public TableEventSet()
	{
	}

	public TableEventSet(TableEventSet eventSet)
		: base(eventSet)
	{
	}

	public TableEventSet(TableEvent anEvent)
	{
		SetBit(anEvent);
	}

	public TableEventSet(params TableEvent[] events)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (TableEvent bit in events)
		{
			SetBit(bit);
		}
	}

	public override EventSetBase Copy()
	{
		return new TableEventSet(base.Storage);
	}

	internal TableEventSet(BitArray storage)
	{
		base.Storage = (BitArray)storage.Clone();
	}

	internal void SetBit(TableEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = true;
	}

	internal void ResetBit(TableEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = false;
	}

	public TableEventSet Add(TableEvent anEvent)
	{
		SetBit(anEvent);
		return this;
	}

	public TableEventSet Remove(TableEvent anEvent)
	{
		ResetBit(anEvent);
		return this;
	}

	public static TableEventSet operator +(TableEventSet eventSet, TableEvent anEvent)
	{
		TableEventSet tableEventSet = new TableEventSet(eventSet);
		tableEventSet.SetBit(anEvent);
		return tableEventSet;
	}

	public static TableEventSet Add(TableEventSet eventSet, TableEvent anEvent)
	{
		return eventSet + anEvent;
	}

	public static TableEventSet operator -(TableEventSet eventSet, TableEvent anEvent)
	{
		TableEventSet tableEventSet = new TableEventSet(eventSet);
		tableEventSet.ResetBit(anEvent);
		return tableEventSet;
	}

	public static TableEventSet Subtract(TableEventSet eventSet, TableEvent anEvent)
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
				stringBuilder.Append(((TableEventValues)num).ToString());
			}
			num++;
		}
		return stringBuilder.ToString();
	}
}
