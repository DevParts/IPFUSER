using System.Collections;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class UserDefinedFunctionEventSet : EventSetBase
{
	public override int NumberOfElements => 2;

	public bool AlterFunction
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

	public bool DropFunction
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

	public UserDefinedFunctionEventSet()
	{
	}

	public UserDefinedFunctionEventSet(UserDefinedFunctionEventSet eventSet)
		: base(eventSet)
	{
	}

	public UserDefinedFunctionEventSet(UserDefinedFunctionEvent anEvent)
	{
		SetBit(anEvent);
	}

	public UserDefinedFunctionEventSet(params UserDefinedFunctionEvent[] events)
	{
		base.Storage = new BitArray(NumberOfElements);
		foreach (UserDefinedFunctionEvent bit in events)
		{
			SetBit(bit);
		}
	}

	public override EventSetBase Copy()
	{
		return new UserDefinedFunctionEventSet(base.Storage);
	}

	internal UserDefinedFunctionEventSet(BitArray storage)
	{
		base.Storage = (BitArray)storage.Clone();
	}

	internal void SetBit(UserDefinedFunctionEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = true;
	}

	internal void ResetBit(UserDefinedFunctionEvent anEvent)
	{
		base.Storage[(int)anEvent.Value] = false;
	}

	public UserDefinedFunctionEventSet Add(UserDefinedFunctionEvent anEvent)
	{
		SetBit(anEvent);
		return this;
	}

	public UserDefinedFunctionEventSet Remove(UserDefinedFunctionEvent anEvent)
	{
		ResetBit(anEvent);
		return this;
	}

	public static UserDefinedFunctionEventSet operator +(UserDefinedFunctionEventSet eventSet, UserDefinedFunctionEvent anEvent)
	{
		UserDefinedFunctionEventSet userDefinedFunctionEventSet = new UserDefinedFunctionEventSet(eventSet);
		userDefinedFunctionEventSet.SetBit(anEvent);
		return userDefinedFunctionEventSet;
	}

	public static UserDefinedFunctionEventSet Add(UserDefinedFunctionEventSet eventSet, UserDefinedFunctionEvent anEvent)
	{
		return eventSet + anEvent;
	}

	public static UserDefinedFunctionEventSet operator -(UserDefinedFunctionEventSet eventSet, UserDefinedFunctionEvent anEvent)
	{
		UserDefinedFunctionEventSet userDefinedFunctionEventSet = new UserDefinedFunctionEventSet(eventSet);
		userDefinedFunctionEventSet.ResetBit(anEvent);
		return userDefinedFunctionEventSet;
	}

	public static UserDefinedFunctionEventSet Subtract(UserDefinedFunctionEventSet eventSet, UserDefinedFunctionEvent anEvent)
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
				stringBuilder.Append(((UserDefinedFunctionEventValues)num).ToString());
			}
			num++;
		}
		return stringBuilder.ToString();
	}
}
