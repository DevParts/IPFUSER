using System;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.InkJets;

public class SauvenMsg
{
	public enum BARCODES : byte
	{
		ITF = 180,
		EAN13 = 181,
		UPCA = 182,
		CODE128 = 205,
		EAN128 = 206
	}

	public enum TEXT_ATTR : byte
	{
		NORMAL_X1 = 0,
		BOLD = 8,
		BIG_X2 = 0x10,
		HUGE_X4 = 0x20
	}

	private byte[][] Lines;

	private byte[][] Attr;

	private string CBarData;

	private BARCODES CBarType;

	public SauvenMsg()
	{
		Clear();
	}

	public void Clear()
	{
		Lines = new byte[4][];
		Attr = new byte[4][];
		int num = 0;
		checked
		{
			int num5;
			int num4;
			do
			{
				byte[] array = new byte[40];
				Lines[num] = array;
				array = new byte[40];
				Attr[num] = array;
				int num2 = 0;
				int num3;
				do
				{
					Lines[num][num2] = 32;
					num2++;
					num3 = num2;
					num4 = 39;
				}
				while (num3 <= num4);
				num++;
				num5 = num;
				num4 = 3;
			}
			while (num5 <= num4);
			CBarData = "";
		}
	}

	public bool SetLine(int LineNumber, string LineValue, TEXT_ATTR Attribute)
	{
		if (LineNumber < 1 || LineNumber > 4)
		{
			return false;
		}
		byte[] bytes = Encoding.Default.GetBytes(LineValue);
		checked
		{
			switch (unchecked((int)(Attribute & (TEXT_ATTR)56)))
			{
			case 0:
			{
				LineNumber--;
				Array.Copy(bytes, Lines[LineNumber], bytes.Length);
				int num7 = bytes.Length - 1;
				int num = 0;
				while (true)
				{
					int num8 = num;
					int num5 = num7;
					if (num8 <= num5)
					{
						Attr[LineNumber][num] = unchecked((byte)Attribute);
						num++;
						continue;
					}
					break;
				}
				break;
			}
			case 16:
			{
				if (unchecked(LineNumber != 1 && LineNumber != 3))
				{
					return false;
				}
				LineNumber--;
				int num = 0;
				int num11;
				int num5;
				do
				{
					Array.Copy(bytes, Lines[LineNumber + num], bytes.Length);
					int num9 = bytes.Length - 1;
					int num3 = 0;
					while (true)
					{
						int num10 = num3;
						num5 = num9;
						if (num10 > num5)
						{
							break;
						}
						Attr[LineNumber + num][num3] = unchecked((byte)Attribute);
						num3++;
					}
					num++;
					num11 = num;
					num5 = 1;
				}
				while (num11 <= num5);
				break;
			}
			case 32:
			{
				if (LineNumber != 1)
				{
					return false;
				}
				LineNumber--;
				int num = 0;
				int num6;
				int num5;
				do
				{
					Array.Copy(bytes, Lines[LineNumber + num], bytes.Length);
					int num2 = bytes.Length - 1;
					int num3 = 0;
					while (true)
					{
						int num4 = num3;
						num5 = num2;
						if (num4 > num5)
						{
							break;
						}
						Attr[LineNumber + num][num3] = unchecked((byte)Attribute);
						num3++;
					}
					num++;
					num6 = num;
					num5 = 3;
				}
				while (num6 <= num5);
				break;
			}
			}
			return true;
		}
	}

	public byte[] GetBinMessage()
	{
		byte[] array = new byte[255];
		Array.Copy(Lines[0], 0, array, 0, Lines[0].Length);
		Array.Copy(Lines[1], 0, array, 40, Lines[0].Length);
		Array.Copy(Lines[2], 0, array, 80, Lines[0].Length);
		Array.Copy(Lines[3], 0, array, 120, Lines[0].Length);
		if (Operators.CompareString(CBarData, "", TextCompare: false) != 0)
		{
			CBarData = CBarData.PadRight(48, ' ');
			byte[] bytes = Encoding.Default.GetBytes(CBarData);
			array[182] = (byte)CBarType;
			Array.Copy(bytes, 0, array, 183, bytes.Length);
		}
		return array;
	}

	public byte[] GetExtraBinMessage()
	{
		byte[] array = new byte[160];
		Array.Copy(Attr[0], 0, array, 0, Lines[0].Length);
		Array.Copy(Attr[1], 0, array, 40, Lines[0].Length);
		Array.Copy(Attr[2], 0, array, 80, Lines[0].Length);
		Array.Copy(Attr[3], 0, array, 120, Lines[0].Length);
		return array;
	}

	public bool SetCodeBar(BARCODES CbType, string Value)
	{
		int maxLength = GetMaxLength();
		int num = 0;
		int num2;
		int num3;
		do
		{
			Lines[num][maxLength] = (byte)CbType;
			Attr[num][maxLength] = 0;
			num = checked(num + 1);
			num2 = num;
			num3 = 3;
		}
		while (num2 <= num3);
		CBarType = CbType;
		CBarData = Value;
		bool result = default(bool);
		return result;
	}

	private byte GetMaxLength()
	{
		byte b = 0;
		int num = 0;
		checked
		{
			int num5;
			int num4;
			do
			{
				byte b2 = 0;
				int num2 = 39;
				while (Lines[num][num2] == 32)
				{
					b2 = (byte)num2;
					num2 += -1;
					int num3 = num2;
					num4 = 0;
					if (num3 < num4)
					{
						break;
					}
				}
				if (unchecked((uint)b2 > (uint)b))
				{
					b = b2;
				}
				num++;
				num5 = num;
				num4 = 3;
			}
			while (num5 <= num4);
			return b;
		}
	}
}
