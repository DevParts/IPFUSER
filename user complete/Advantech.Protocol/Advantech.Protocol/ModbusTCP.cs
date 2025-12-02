using System;
using Advantech.Common;

namespace Advantech.Protocol;

public class ModbusTCP
{
	private static ErrorCode m_error;

	public static ErrorCode LastError => m_error;

	static ModbusTCP()
	{
	}

	protected static void ConstructSendPacket(int i_iAddr, int i_iFun, int i_iStartIndex, int i_iTotalPoint, out byte[] o_byPacket)
	{
		m_error = ErrorCode.No_Error;
		string text = i_iAddr.ToString("X02") + i_iFun.ToString("X02") + (i_iStartIndex - 1).ToString("X04") + i_iTotalPoint.ToString("X04");
		string text2 = "0000000000" + (text.Length / 2).ToString("X02") + text;
		Tool.HexStringToRTUArray(text2, out var o_byRTU);
		int num = text2.Length / 2;
		o_byPacket = new byte[num];
		Array.Copy(o_byRTU, 0, o_byPacket, 0, num);
	}

	protected static bool VerifyRecvPacket(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket, out byte[] o_byRecvData)
	{
		m_error = ErrorCode.No_Error;
		if (i_iRecvLen > 9)
		{
			if (i_bySendPacket[7] == i_byRecvPacket[7])
			{
				int num = i_byRecvPacket[8];
				o_byRecvData = new byte[num];
				Array.Copy(i_byRecvPacket, 9, o_byRecvData, 0, num);
				return true;
			}
			m_error = (ErrorCode)(1074335744 + i_byRecvPacket[8]);
		}
		else
		{
			m_error = ErrorCode.Modbus_Invalid_Length;
		}
		o_byRecvData = new byte[1];
		return false;
	}

	public static void ConstructReadCoilStatus(int i_iAddr, int i_iStartIndex, int i_iTotalPoint, out byte[] o_byPacket)
	{
		ConstructSendPacket(i_iAddr, 1, i_iStartIndex, i_iTotalPoint, out o_byPacket);
	}

	public static bool VerifyReadCoilStatus(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket, out byte[] o_byRecvData)
	{
		return VerifyRecvPacket(i_iSendLen, i_bySendPacket, i_iRecvLen, i_byRecvPacket, out o_byRecvData);
	}

	public static void ConstructReadInputStatus(int i_iAddr, int i_iStartIndex, int i_iTotalPoint, out byte[] o_byPacket)
	{
		ConstructSendPacket(i_iAddr, 2, i_iStartIndex, i_iTotalPoint, out o_byPacket);
	}

	public static bool VerifyReadInputStatus(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket, out byte[] o_byRecvData)
	{
		return VerifyRecvPacket(i_iSendLen, i_bySendPacket, i_iRecvLen, i_byRecvPacket, out o_byRecvData);
	}

	public static void ConstructReadHoldingRegs(int i_iAddr, int i_iStartIndex, int i_iTotalPoint, out byte[] o_byPacket)
	{
		ConstructSendPacket(i_iAddr, 3, i_iStartIndex, i_iTotalPoint, out o_byPacket);
	}

	public static bool VerifyReadHoldingRegs(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket, out byte[] o_byRecvData)
	{
		return VerifyRecvPacket(i_iSendLen, i_bySendPacket, i_iRecvLen, i_byRecvPacket, out o_byRecvData);
	}

	public static void ConstructReadInputRegs(int i_iAddr, int i_iStartIndex, int i_iTotalPoint, out byte[] o_byPacket)
	{
		ConstructSendPacket(i_iAddr, 4, i_iStartIndex, i_iTotalPoint, out o_byPacket);
	}

	public static bool VerifyReadInputRegs(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket, out byte[] o_byRecvData)
	{
		return VerifyRecvPacket(i_iSendLen, i_bySendPacket, i_iRecvLen, i_byRecvPacket, out o_byRecvData);
	}

	public static void ConstructForceSingleCoil(int i_iAddr, int i_iCoilIndex, int i_iData, out byte[] o_byPacket)
	{
		int num = 5;
		string text = ((i_iData != 0) ? "FF00" : "0000");
		string text2 = i_iAddr.ToString("X02") + num.ToString("X02") + (i_iCoilIndex - 1).ToString("X04") + text;
		string text3 = "0000000000" + (text2.Length / 2).ToString("X02") + text2;
		Tool.HexStringToRTUArray(text3, out var o_byRTU);
		int num2 = text3.Length / 2;
		o_byPacket = new byte[num2];
		Array.Copy(o_byRTU, 0, o_byPacket, 0, num2);
	}

	public static bool VerifyForceSingleCoil(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket)
	{
		m_error = ErrorCode.No_Error;
		if (i_iRecvLen >= 12)
		{
			if (i_bySendPacket[7] == i_byRecvPacket[7])
			{
				return true;
			}
			m_error = (ErrorCode)(1074335744 + i_byRecvPacket[8]);
		}
		else
		{
			m_error = ErrorCode.Modbus_Invalid_Length;
		}
		return false;
	}

	public static void ConstructPresetSingleReg(int i_iAddr, int i_iRegIndex, int i_iData, out byte[] o_byPacket)
	{
		string text = i_iAddr.ToString("X02") + 6.ToString("X02") + (i_iRegIndex - 1).ToString("X04") + i_iData.ToString("X04");
		string text2 = "0000000000" + (text.Length / 2).ToString("X02") + text;
		Tool.HexStringToRTUArray(text2, out var o_byRTU);
		int num = text2.Length / 2;
		o_byPacket = new byte[num];
		Array.Copy(o_byRTU, 0, o_byPacket, 0, num);
	}

	public static bool VerifyPresetSingleReg(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket)
	{
		m_error = ErrorCode.No_Error;
		if (i_iRecvLen >= 12)
		{
			if (i_bySendPacket[7] == i_byRecvPacket[7])
			{
				return true;
			}
			m_error = (ErrorCode)(1074335744 + i_byRecvPacket[8]);
		}
		else
		{
			m_error = ErrorCode.Modbus_Invalid_Length;
		}
		return false;
	}

	public static void ConstructForceMultiCoils(int i_iAddr, int i_iCoilIndex, int i_iTotalPoint, int i_iTotalByte, byte[] i_byData, out byte[] o_byPacket)
	{
		int num = 15;
		string text = i_iAddr.ToString("X02") + num.ToString("X02") + (i_iCoilIndex - 1).ToString("X04") + i_iTotalPoint.ToString("X04") + i_iTotalByte.ToString("X02");
		for (int i = 0; i < i_iTotalByte; i++)
		{
			text += i_byData[i].ToString("X02");
		}
		string text2 = "0000000000" + (text.Length / 2).ToString("X02") + text;
		Tool.HexStringToRTUArray(text2, out var o_byRTU);
		int num2 = text2.Length / 2;
		o_byPacket = new byte[num2];
		Array.Copy(o_byRTU, 0, o_byPacket, 0, num2);
	}

	public static bool VerifyForceMultiCoils(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket)
	{
		m_error = ErrorCode.No_Error;
		if (i_iRecvLen >= 8 && Tool.ValidateCRC(ref i_byRecvPacket, ref i_iRecvLen))
		{
			if (i_bySendPacket[7] == i_byRecvPacket[7])
			{
				return true;
			}
			m_error = (ErrorCode)(1074335744 + i_byRecvPacket[8]);
		}
		else
		{
			m_error = ErrorCode.Modbus_Invalid_Length;
		}
		return false;
	}

	public static void ConstructPresetMultiRegs(int i_iAddr, int i_iStartReg, int i_iTotalReg, int i_iTotalByte, byte[] i_byData, out byte[] o_byPacket)
	{
		int num = 16;
		string text = i_iAddr.ToString("X02") + num.ToString("X02") + (i_iStartReg - 1).ToString("X04") + i_iTotalReg.ToString("X04") + i_iTotalByte.ToString("X02");
		for (int i = 0; i < i_iTotalByte; i++)
		{
			text += i_byData[i].ToString("X02");
		}
		string text2 = "0000000000" + (text.Length / 2).ToString("X02") + text;
		Tool.HexStringToRTUArray(text2, out var o_byRTU);
		int num2 = text2.Length / 2;
		o_byPacket = new byte[num2];
		Array.Copy(o_byRTU, 0, o_byPacket, 0, num2);
	}

	public static bool VerifyPresetMultiRegs(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket)
	{
		m_error = ErrorCode.No_Error;
		if (i_iRecvLen >= 12)
		{
			if (i_bySendPacket[7] == i_byRecvPacket[7])
			{
				return true;
			}
			m_error = (ErrorCode)(1074335744 + i_byRecvPacket[8]);
		}
		else
		{
			m_error = ErrorCode.Modbus_Invalid_Length;
		}
		return false;
	}

	public static void ConstructReadAdvantechRegs(int i_iAddr, int i_iStartIndex, int i_iTotalPoint, out byte[] o_byPacket)
	{
		ConstructSendPacket(i_iAddr, 32, i_iStartIndex, i_iTotalPoint, out o_byPacket);
	}

	public static bool VerifyReadAdvantechRegs(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket, out byte[] o_byRecvData)
	{
		return VerifyRecvPacket(i_iSendLen, i_bySendPacket, i_iRecvLen, i_byRecvPacket, out o_byRecvData);
	}
}
