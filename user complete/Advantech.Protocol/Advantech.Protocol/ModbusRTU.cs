using System;
using Advantech.Common;

namespace Advantech.Protocol;

public class ModbusRTU
{
	private static ErrorCode m_error;

	public static ErrorCode LastError => m_error;

	static ModbusRTU()
	{
	}

	protected static void ConstructSendPacket(int i_iAddr, int i_iFun, int i_iStartIndex, int i_iTotalPoint, out byte[] o_byPacket)
	{
		m_error = ErrorCode.No_Error;
		string text = i_iAddr.ToString("X02") + i_iFun.ToString("X02") + (i_iStartIndex - 1).ToString("X04") + i_iTotalPoint.ToString("X04");
		Tool.HexStringToRTUArray(text, out var o_byRTU);
		int num = text.Length / 2;
		Tool.CRC16(o_byRTU, num, out var o_byCRC);
		o_byPacket = new byte[num + 2];
		Array.Copy(o_byRTU, 0, o_byPacket, 0, num);
		o_byPacket[num] = o_byCRC[0];
		o_byPacket[num + 1] = o_byCRC[1];
	}

	protected static bool VerifyRecvPacket(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket, out byte[] o_byRecvData)
	{
		m_error = ErrorCode.No_Error;
		if (i_iRecvLen > 3 && Tool.ValidateCRC(ref i_byRecvPacket, ref i_iRecvLen))
		{
			if (i_bySendPacket[1] == i_byRecvPacket[1])
			{
				int num = i_byRecvPacket[2];
				o_byRecvData = new byte[num];
				Array.Copy(i_byRecvPacket, 3, o_byRecvData, 0, num);
				return true;
			}
			m_error = (ErrorCode)(1074335744 + i_byRecvPacket[2]);
		}
		else
		{
			m_error = ErrorCode.Modbus_Invalid_CRC;
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
		m_error = ErrorCode.No_Error;
		string text = ((i_iData != 0) ? "FF00" : "0000");
		string text2 = i_iAddr.ToString("X02") + num.ToString("X02") + (i_iCoilIndex - 1).ToString("X04") + text;
		Tool.HexStringToRTUArray(text2, out var o_byRTU);
		int num2 = text2.Length / 2;
		Tool.CRC16(o_byRTU, num2, out var o_byCRC);
		o_byPacket = new byte[num2 + 2];
		o_byRTU.CopyTo(o_byPacket, 0);
		o_byPacket[num2] = o_byCRC[0];
		o_byPacket[num2 + 1] = o_byCRC[1];
	}

	public static bool VerifyForceSingleCoil(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket)
	{
		m_error = ErrorCode.No_Error;
		if (i_iRecvLen >= 8 && Tool.ValidateCRC(ref i_byRecvPacket, ref i_iRecvLen))
		{
			if (i_bySendPacket[1] == i_byRecvPacket[1])
			{
				return true;
			}
			m_error = (ErrorCode)(1074335744 + i_byRecvPacket[2]);
		}
		else
		{
			m_error = ErrorCode.Modbus_Invalid_CRC;
		}
		return false;
	}

	public static void ConstructPresetSingleReg(int i_iAddr, int i_iRegIndex, int i_iData, out byte[] o_byPacket)
	{
		int num = 6;
		m_error = ErrorCode.No_Error;
		string text = i_iAddr.ToString("X02") + num.ToString("X02") + (i_iRegIndex - 1).ToString("X04") + i_iData.ToString("X04");
		Tool.HexStringToRTUArray(text, out var o_byRTU);
		int num2 = text.Length / 2;
		Tool.CRC16(o_byRTU, num2, out var o_byCRC);
		o_byPacket = new byte[num2 + 2];
		o_byRTU.CopyTo(o_byPacket, 0);
		o_byPacket[num2] = o_byCRC[0];
		o_byPacket[num2 + 1] = o_byCRC[1];
	}

	public static bool VerifyPresetSingleReg(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket)
	{
		m_error = ErrorCode.No_Error;
		if (i_iRecvLen >= 8 && Tool.ValidateCRC(ref i_byRecvPacket, ref i_iRecvLen))
		{
			if (i_bySendPacket[1] == i_byRecvPacket[1])
			{
				return true;
			}
			m_error = (ErrorCode)(1074335744 + i_byRecvPacket[2]);
		}
		else
		{
			m_error = ErrorCode.Modbus_Invalid_CRC;
		}
		return false;
	}

	public static void ConstructForceMultiCoils(int i_iAddr, int i_iCoilIndex, int i_iTotalPoint, int i_iTotalByte, byte[] i_byData, out byte[] o_byPacket)
	{
		int num = 15;
		m_error = ErrorCode.No_Error;
		string i_szHex = i_iAddr.ToString("X02") + num.ToString("X02") + (i_iCoilIndex - 1).ToString("X04") + i_iTotalPoint.ToString("X04") + i_iTotalByte.ToString("X02");
		Tool.HexStringToRTUArray(i_szHex, out var o_byRTU);
		int num2 = o_byRTU.Length + i_iTotalByte;
		o_byPacket = new byte[num2 + 2];
		o_byRTU.CopyTo(o_byPacket, 0);
		i_byData.CopyTo(o_byPacket, o_byRTU.Length);
		Tool.CRC16(o_byPacket, num2, out var o_byCRC);
		o_byPacket[num2] = o_byCRC[0];
		o_byPacket[num2 + 1] = o_byCRC[1];
	}

	public static bool VerifyForceMultiCoils(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket)
	{
		m_error = ErrorCode.No_Error;
		if (i_iRecvLen >= 8 && Tool.ValidateCRC(ref i_byRecvPacket, ref i_iRecvLen))
		{
			if (i_bySendPacket[1] == i_byRecvPacket[1])
			{
				return true;
			}
			m_error = (ErrorCode)(1074335744 + i_byRecvPacket[2]);
		}
		else
		{
			m_error = ErrorCode.Modbus_Invalid_CRC;
		}
		return false;
	}

	public static void ConstructPresetMultiRegs(int i_iAddr, int i_iStartReg, int i_iTotalReg, int i_iTotalByte, byte[] i_byData, out byte[] o_byPacket)
	{
		int num = 16;
		m_error = ErrorCode.No_Error;
		string i_szHex = i_iAddr.ToString("X02") + num.ToString("X02") + (i_iStartReg - 1).ToString("X04") + i_iTotalReg.ToString("X04") + i_iTotalByte.ToString("X02");
		Tool.HexStringToRTUArray(i_szHex, out var o_byRTU);
		int num2 = o_byRTU.Length + i_iTotalByte;
		o_byPacket = new byte[num2 + 2];
		o_byRTU.CopyTo(o_byPacket, 0);
		i_byData.CopyTo(o_byPacket, o_byRTU.Length);
		Tool.CRC16(o_byPacket, num2, out var o_byCRC);
		o_byPacket[num2] = o_byCRC[0];
		o_byPacket[num2 + 1] = o_byCRC[1];
	}

	public static bool VerifyPresetMultiRegs(int i_iSendLen, byte[] i_bySendPacket, int i_iRecvLen, byte[] i_byRecvPacket)
	{
		m_error = ErrorCode.No_Error;
		if (i_iRecvLen >= 8 && Tool.ValidateCRC(ref i_byRecvPacket, ref i_iRecvLen))
		{
			if (i_bySendPacket[1] == i_byRecvPacket[1])
			{
				return true;
			}
			m_error = (ErrorCode)(1074335744 + i_byRecvPacket[2]);
		}
		else
		{
			m_error = ErrorCode.Modbus_Invalid_CRC;
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
