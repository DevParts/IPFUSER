using System.Diagnostics;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Displays;

public class Panel : MacsaDevice
{
	public enum LINE_MODE
	{
		Unknown = 0,
		Correr = 224,
		Temp = 159,
		Immediato = 240,
		Blink = 160,
		ChangeSpeed = 196,
		Line = 199,
		Sinc = 201,
		NoSinc = 202
	}

	private const byte CMD_FASTEXEC = 39;

	private const byte CMD_GETHORA = 11;

	private const byte CMD_STOPPRG = 3;

	private const byte CMD_SEND = 12;

	[DebuggerNonUserCode]
	public Panel()
	{
	}

	private byte[] BuildFrame(byte bCode, string sData)
	{
		byte[] array = new byte[2];
		checked
		{
			int num = Strings.Len(sData) + 7;
			byte[] array2 = new byte[num - 1 + 1];
			byte b = (byte)((num & 0xFF00) >> 8);
			byte b2 = (byte)(num & 0xFF);
			array2[0] = 22;
			array2[1] = b2;
			array2[2] = b;
			array2[3] = 1;
			array2[4] = bCode;
			int num2 = 4;
			int num3 = Strings.Len(sData);
			int num4 = 1;
			while (true)
			{
				int num5 = num4;
				int num6 = num3;
				if (num5 > num6)
				{
					break;
				}
				array2[num2 + num4] = (byte)Strings.Asc(Strings.Mid(sData, num4, 1));
				num4++;
			}
			array = GetChecksum(array2);
			array2[num2 + num4] = array[1];
			array2[num2 + num4 + 1] = array[0];
			return array2;
		}
	}

	private byte[] GetChecksum(byte[] bData)
	{
		byte[] array = new byte[2];
		int num = 0;
		checked
		{
			int num2 = Information.UBound(bData) - 1;
			int num3 = 0;
			while (true)
			{
				int num4 = num3;
				int num5 = num2;
				if (num4 > num5)
				{
					break;
				}
				num += bData[num3];
				num3++;
			}
			array[0] = (byte)((num & 0xFF00) >> 8);
			array[1] = (byte)(num & 0xFF);
			return array;
		}
	}

	public void SetText1Line(string sText, LINE_MODE lMode, string sSpeed = "0")
	{
		string text = "";
		if ((Conversions.ToInteger(sSpeed) > 0) & (Conversions.ToInteger(sSpeed) < 128))
		{
			text = Conversions.ToString(Strings.Chr(196)) + sSpeed;
		}
		text = text + Conversions.ToString(Strings.Chr((int)lMode)) + sText;
		SendData(BuildFrame(39, text), 0L);
	}

	public void SetText2Line(string sLine1, string sLine2, LINE_MODE lMode, bool bSincro, string sSpeed = "0")
	{
		string text = "";
		if ((Conversions.ToInteger(sSpeed) > 0) & (Conversions.ToInteger(sSpeed) < 128))
		{
			text = Conversions.ToString(Strings.Chr(196)) + sSpeed;
		}
		if (bSincro)
		{
			text += Conversions.ToString(Strings.Chr(201));
		}
		text = text + Conversions.ToString(Strings.Chr(199)) + "1" + Conversions.ToString(Strings.Chr((int)lMode)) + sLine1 + Conversions.ToString(Strings.Chr(199)) + "2" + Conversions.ToString(Strings.Chr((int)lMode)) + sLine2;
		if (bSincro)
		{
			text += Conversions.ToString(Strings.Chr(202));
		}
		SendData(BuildFrame(39, text), 0L);
	}

	public void GetHora()
	{
		SendData(BuildFrame(11, ""), 0L);
	}

	public void StopProgram()
	{
		SendData(BuildFrame(3, ""), 0L);
	}

	public void SendPrg(string sData)
	{
		SendData(BuildFrame(39, sData), 0L);
	}

	protected override void TcpComm_DataReceived(string sData)
	{
	}
}
