using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

internal class BitStorage
{
	internal enum BitIndex
	{
		Retrieved,
		Dirty,
		Enabled,
		Uninitialized
	}

	private const int BitsPerItem = 4;

	private const uint BitPattern = 2290649224u;

	private int count;

	private uint[] bitArray;

	internal int Count => count;

	internal BitStorage(int itemCount)
	{
		count = itemCount;
		bitArray = new uint[(itemCount * 4 + 31) / 32];
		for (int i = 0; i < bitArray.Length; i++)
		{
			bitArray[i] = 2290649224u;
		}
	}

	protected void SetBit(int itemIndex, BitIndex bitIndex, bool value)
	{
		TraceHelper.Assert(itemIndex >= 0 && itemIndex < count);
		int num = (int)(itemIndex * 4 + bitIndex);
		if (value)
		{
			bitArray[num / 32] |= (uint)(1 << num % 32);
		}
		else
		{
			bitArray[num / 32] &= (uint)(~(1 << num % 32));
		}
	}

	protected bool GetBit(int itemIndex, BitIndex bitIndex)
	{
		TraceHelper.Assert(itemIndex >= 0 && itemIndex < count);
		int num = (int)(itemIndex * 4 + bitIndex);
		return (bitArray[num / 32] & (1 << num % 32)) != 0;
	}
}
