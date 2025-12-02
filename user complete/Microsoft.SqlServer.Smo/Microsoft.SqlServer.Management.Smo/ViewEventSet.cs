using System.Collections;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ViewEventSet : EventSetBase
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

	public bool AlterView
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

	public bool DropView
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

	public ViewEventSet()
	{
	}

	public ViewEventSet(ViewEventSet eventSet)
		: base(eventSet)
	{
	}

	public ViewEventSet(ViewEvent anEvent)
	{
		SetBit(anEvent);
	}

	public ViewEventSet(params ViewEvent[] events)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (ViewEvent bit in events)
		{
			SetBit(bit);
		}
	}

	public override EventSetBase Copy()
	{
		return new ViewEventSet(base.Storage);
	}

	internal ViewEventSet(BitArray storage)
	{
		base.Storage = (BitArray)storage.Clone();
	}

	internal void SetBit(ViewEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = true;
	}

	internal void ResetBit(ViewEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = false;
	}

	public ViewEventSet Add(ViewEvent anEvent)
	{
		SetBit(anEvent);
		return this;
	}

	public ViewEventSet Remove(ViewEvent anEvent)
	{
		ResetBit(anEvent);
		return this;
	}

	public static ViewEventSet operator +(ViewEventSet eventSet, ViewEvent anEvent)
	{
		ViewEventSet viewEventSet = new ViewEventSet(eventSet);
		viewEventSet.SetBit(anEvent);
		return viewEventSet;
	}

	public static ViewEventSet Add(ViewEventSet eventSet, ViewEvent anEvent)
	{
		return eventSet + anEvent;
	}

	public static ViewEventSet operator -(ViewEventSet eventSet, ViewEvent anEvent)
	{
		ViewEventSet viewEventSet = new ViewEventSet(eventSet);
		viewEventSet.ResetBit(anEvent);
		return viewEventSet;
	}

	public static ViewEventSet Subtract(ViewEventSet eventSet, ViewEvent anEvent)
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
				stringBuilder.Append(((ViewEventValues)num).ToString());
			}
			num++;
		}
		return stringBuilder.ToString();
	}
}
