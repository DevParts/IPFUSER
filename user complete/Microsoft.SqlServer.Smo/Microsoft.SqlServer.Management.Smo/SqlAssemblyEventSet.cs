using System.Collections;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SqlAssemblyEventSet : EventSetBase
{
	public override int NumberOfElements => 2;

	public bool AlterAssembly
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

	public bool DropAssembly
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

	public SqlAssemblyEventSet()
	{
	}

	public SqlAssemblyEventSet(SqlAssemblyEventSet eventSet)
		: base(eventSet)
	{
	}

	public SqlAssemblyEventSet(SqlAssemblyEvent anEvent)
	{
		SetBit(anEvent);
	}

	public SqlAssemblyEventSet(params SqlAssemblyEvent[] events)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (SqlAssemblyEvent bit in events)
		{
			SetBit(bit);
		}
	}

	public override EventSetBase Copy()
	{
		return new SqlAssemblyEventSet(base.Storage);
	}

	internal SqlAssemblyEventSet(BitArray storage)
	{
		base.Storage = (BitArray)storage.Clone();
	}

	internal void SetBit(SqlAssemblyEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = true;
	}

	internal void ResetBit(SqlAssemblyEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = false;
	}

	public SqlAssemblyEventSet Add(SqlAssemblyEvent anEvent)
	{
		SetBit(anEvent);
		return this;
	}

	public SqlAssemblyEventSet Remove(SqlAssemblyEvent anEvent)
	{
		ResetBit(anEvent);
		return this;
	}

	public static SqlAssemblyEventSet operator +(SqlAssemblyEventSet eventSet, SqlAssemblyEvent anEvent)
	{
		SqlAssemblyEventSet sqlAssemblyEventSet = new SqlAssemblyEventSet(eventSet);
		sqlAssemblyEventSet.SetBit(anEvent);
		return sqlAssemblyEventSet;
	}

	public static SqlAssemblyEventSet Add(SqlAssemblyEventSet eventSet, SqlAssemblyEvent anEvent)
	{
		return eventSet + anEvent;
	}

	public static SqlAssemblyEventSet operator -(SqlAssemblyEventSet eventSet, SqlAssemblyEvent anEvent)
	{
		SqlAssemblyEventSet sqlAssemblyEventSet = new SqlAssemblyEventSet(eventSet);
		sqlAssemblyEventSet.ResetBit(anEvent);
		return sqlAssemblyEventSet;
	}

	public static SqlAssemblyEventSet Subtract(SqlAssemblyEventSet eventSet, SqlAssemblyEvent anEvent)
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
				stringBuilder.Append(((SqlAssemblyEventValues)num).ToString());
			}
			num++;
		}
		return stringBuilder.ToString();
	}
}
