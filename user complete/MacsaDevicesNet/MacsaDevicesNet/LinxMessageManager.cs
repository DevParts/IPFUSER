using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet;

public class LinxMessageManager
{
	private enum LinxFieldType
	{
		TextField,
		LogoField,
		TimeField,
		SequentialMessageField,
		SequentialNumberField,
		DateField,
		BarcodeField,
		RemoteField,
		PixelField,
		GS1128DataMatrixField
	}

	private const byte LOW_BYTE_PART = 15;

	private const byte HIGH_BYTE_PART = 240;

	private const byte MESSAGE_HEADER_LENGTH = 41;

	private const byte FIELD_HEADER = 28;

	private const byte FIELD_LENGTH_START_BYTE = 2;

	[DebuggerNonUserCode]
	public LinxMessageManager()
	{
	}

	public byte[] ProcessMessage(string theMessageName, ushort theCounterValue)
	{
		byte[] theMessageData = getMessageBytes(theMessageName);
		checked
		{
			int num;
			for (int i = 41; i < theMessageData.Length; i += num)
			{
				num = BitConverter.ToUInt16(theMessageData, i + 2);
				if (!isSequentialFieldTypeStart(ref theMessageData, i))
				{
					continue;
				}
				int theIndex = i + num - 2;
				int num2 = locateSequentialNumberField(ref theMessageData, ref theIndex);
				string text = theCounterValue.ToString("D" + Conversions.ToString(num2));
				double num3 = Math.Pow(10.0, num2);
				if (num3 < (double)unchecked((int)theCounterValue))
				{
					throw new ArgumentException("Value " + text + " does not fit in a " + Conversions.ToString(num2) + " digit sequential field");
				}
				theIndex++;
				int num4 = text.Length - 1;
				int num5 = 0;
				while (true)
				{
					int num6 = num5;
					int num7 = num4;
					if (num6 > num7)
					{
						break;
					}
					theMessageData[theIndex] = (byte)Strings.Asc(text[num5]);
					theIndex++;
					num5++;
				}
			}
			writeMessageBytes(ref theMessageData);
			return theMessageData;
		}
	}

	private bool isSequentialFieldTypeStart(ref byte[] theMessageData, int theFieldStartIndex)
	{
		byte b = (byte)(theMessageData[checked(theFieldStartIndex + 1)] & 0xF);
		return (theMessageData[theFieldStartIndex] == 28 && b == 4) ? true : false;
	}

	public byte[] getMessageBytes(string theMessageName)
	{
		FileStream input = new FileStream(theMessageName, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(input);
		FileInfo fileInfo = new FileInfo(theMessageName);
		checked
		{
			byte[] array = new byte[(int)fileInfo.Length - 1 + 1];
			binaryReader.Read(array, 0, (int)fileInfo.Length);
			binaryReader.Close();
			return array;
		}
	}

	private void writeMessageBytes(ref byte[] theMessageData)
	{
		FileStream output = new FileStream(Application.StartupPath + "\\UltimoEnvio.dat", FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(output);
		binaryWriter.Write(theMessageData);
		binaryWriter.Close();
	}

	private int locateSequentialNumberField(ref byte[] theMessageData, ref int theIndex)
	{
		int num = 0;
		checked
		{
			while (theMessageData[theIndex] != 0)
			{
				num++;
				theIndex--;
			}
			return num;
		}
	}
}
