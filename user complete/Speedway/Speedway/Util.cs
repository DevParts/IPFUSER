using System;

namespace Speedway;

public class Util
{
	public static byte[] ConvertBinaryStringArrayToBytes(string binaryString, int mask_len)
	{
		try
		{
			int result = 0;
			long num = Math.DivRem(binaryString.Length, 8, out result);
			string text = "";
			if (result != 0)
			{
				text = text.PadRight(8 - result, '0');
				binaryString += text;
				num++;
			}
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				string value = binaryString.Substring(i * 8, 8);
				array[i] = Convert.ToByte(value, 2);
			}
			return array;
		}
		catch
		{
			return null;
		}
	}
}
