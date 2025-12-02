using System;
using Advantech.Common;
using Advantech.Protocol;

namespace Advantech.Adam;

public class Modbus : AdamBase
{
	private const int ADAM_MAX_MSGLEN = 1200;

	private AdamCom m_com;

	private AdamSocket m_socket;

	public Modbus(AdamCom i_com)
		: base(i_com)
	{
		m_com = i_com;
		m_socket = null;
	}

	public Modbus(AdamSocket i_socket)
		: base(i_socket)
	{
		m_com = null;
		m_socket = i_socket;
	}

	public bool ReadCoilStatus(int i_iStartIndex, int i_iTotalPoint, out byte[] o_byteCoil)
	{
		byte[] o_btData = new byte[1200];
		int o_iLen = 0;
		int num = 0;
		base.LastError = ErrorCode.No_Error;
		byte[] o_byPacket;
		if (m_com != null)
		{
			ModbusRTU.ConstructReadCoilStatus(base.Address, i_iStartIndex, i_iTotalPoint, out o_byPacket);
			int num2 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num2, o_byPacket) == num2)
			{
				num = (i_iTotalPoint - 1) / 8 + 1 + 5;
				o_iLen = m_com.Recv(num, ref o_btData);
				if (ModbusRTU.VerifyReadCoilStatus(num2, o_byPacket, o_iLen, o_btData, out o_byteCoil))
				{
					return true;
				}
				base.LastError = ModbusRTU.LastError;
				return false;
			}
			base.LastError = ErrorCode.ComPort_Send_Fail;
		}
		else if (m_socket != null)
		{
			ModbusTCP.ConstructReadCoilStatus(base.Address, i_iStartIndex, i_iTotalPoint, out o_byPacket);
			int num2 = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num2))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyReadCoilStatus(num2, o_byPacket, o_iLen, o_btData, out o_byteCoil))
					{
						return true;
					}
					base.LastError = ModbusTCP.LastError;
					return false;
				}
				base.LastError = ErrorCode.Socket_Recv_Fail;
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		o_byteCoil = null;
		return false;
	}

	public bool ReadCoilStatus(int i_iStartIndex, int i_iTotalPoint, out bool[] o_bCoil)
	{
		if (ReadCoilStatus(i_iStartIndex, i_iTotalPoint, out byte[] o_byteCoil) && o_byteCoil.Length >= i_iTotalPoint / 8)
		{
			o_bCoil = new bool[i_iTotalPoint];
			for (int i = 0; i < i_iTotalPoint; i++)
			{
				int num = i / 8;
				int num2 = i % 8;
				if ((o_byteCoil[num] & (1 << num2)) > 0)
				{
					o_bCoil[i] = true;
				}
				else
				{
					o_bCoil[i] = false;
				}
			}
			return true;
		}
		o_bCoil = null;
		return false;
	}

	public bool ReadInputStatus(int i_iStartIndex, int i_iTotalPoint, out byte[] o_byteCoil)
	{
		byte[] o_btData = new byte[1200];
		int o_iLen = 0;
		int num = 0;
		base.LastError = ErrorCode.No_Error;
		byte[] o_byPacket;
		if (m_com != null)
		{
			ModbusRTU.ConstructReadInputStatus(base.Address, i_iStartIndex, i_iTotalPoint, out o_byPacket);
			int num2 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num2, o_byPacket) == num2)
			{
				num = (i_iTotalPoint - 1) / 8 + 1 + 5;
				o_iLen = m_com.Recv(num, ref o_btData);
				if (ModbusRTU.VerifyReadInputStatus(num2, o_byPacket, o_iLen, o_btData, out o_byteCoil))
				{
					return true;
				}
				base.LastError = ModbusRTU.LastError;
				return false;
			}
			base.LastError = ErrorCode.ComPort_Send_Fail;
		}
		else if (m_socket != null)
		{
			ModbusTCP.ConstructReadInputStatus(base.Address, i_iStartIndex, i_iTotalPoint, out o_byPacket);
			int num2 = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num2))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyReadInputStatus(num2, o_byPacket, o_iLen, o_btData, out o_byteCoil))
					{
						return true;
					}
					base.LastError = ModbusTCP.LastError;
					return false;
				}
				base.LastError = ErrorCode.Socket_Recv_Fail;
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		o_byteCoil = null;
		return false;
	}

	public bool ReadInputStatus(int i_iStartIndex, int i_iTotalPoint, out bool[] o_bCoil)
	{
		if (ReadInputStatus(i_iStartIndex, i_iTotalPoint, out byte[] o_byteCoil) && o_byteCoil.Length >= i_iTotalPoint / 8)
		{
			o_bCoil = new bool[i_iTotalPoint];
			for (int i = 0; i < i_iTotalPoint; i++)
			{
				int num = i / 8;
				int num2 = i % 8;
				if ((o_byteCoil[num] & (1 << num2)) > 0)
				{
					o_bCoil[i] = true;
				}
				else
				{
					o_bCoil[i] = false;
				}
			}
			return true;
		}
		o_bCoil = null;
		return false;
	}

	public bool ReadHoldingRegs(int i_iStartIndex, int i_iTotalPoint, out byte[] o_byteData)
	{
		byte[] o_btData = new byte[1200];
		int o_iLen = 0;
		int num = 0;
		base.LastError = ErrorCode.No_Error;
		byte[] o_byPacket;
		if (m_com != null)
		{
			ModbusRTU.ConstructReadHoldingRegs(base.Address, i_iStartIndex, i_iTotalPoint, out o_byPacket);
			int num2 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num2, o_byPacket) == num2)
			{
				num = i_iTotalPoint * 2 + 5;
				o_iLen = m_com.Recv(num, ref o_btData);
				if (ModbusRTU.VerifyReadHoldingRegs(num2, o_byPacket, o_iLen, o_btData, out o_byteData))
				{
					return true;
				}
				base.LastError = ModbusRTU.LastError;
				return false;
			}
			base.LastError = ErrorCode.ComPort_Send_Fail;
		}
		else if (m_socket != null)
		{
			ModbusTCP.ConstructReadHoldingRegs(base.Address, i_iStartIndex, i_iTotalPoint, out o_byPacket);
			int num2 = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num2))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyReadHoldingRegs(num2, o_byPacket, o_iLen, o_btData, out o_byteData))
					{
						return true;
					}
					base.LastError = ModbusTCP.LastError;
					return false;
				}
				base.LastError = ErrorCode.Socket_Recv_Fail;
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		o_byteData = null;
		return false;
	}

	public bool ReadHoldingRegs(int i_iStartIndex, int i_iTotalPoint, out int[] o_iData)
	{
		if (ReadHoldingRegs(i_iStartIndex, i_iTotalPoint, out byte[] o_byteData))
		{
			o_iData = new int[i_iTotalPoint];
			for (int i = 0; i < i_iTotalPoint; i++)
			{
				o_iData[i] = o_byteData[i * 2] * 256 + o_byteData[i * 2 + 1];
			}
			return true;
		}
		o_iData = null;
		return false;
	}

	public bool ReadInputRegs(int i_iStartIndex, int i_iTotalPoint, out byte[] o_byteData)
	{
		byte[] o_btData = new byte[1200];
		int o_iLen = 0;
		int num = 0;
		byte[] o_byPacket;
		if (m_com != null)
		{
			ModbusRTU.ConstructReadInputRegs(base.Address, i_iStartIndex, i_iTotalPoint, out o_byPacket);
			int num2 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num2, o_byPacket) == num2)
			{
				num = i_iTotalPoint * 2 + 5;
				o_iLen = m_com.Recv(num, ref o_btData);
				if (ModbusRTU.VerifyReadInputRegs(num2, o_byPacket, o_iLen, o_btData, out o_byteData))
				{
					return true;
				}
				base.LastError = ModbusRTU.LastError;
				return false;
			}
			base.LastError = ErrorCode.ComPort_Send_Fail;
		}
		else if (m_socket != null)
		{
			ModbusTCP.ConstructReadInputRegs(base.Address, i_iStartIndex, i_iTotalPoint, out o_byPacket);
			int num2 = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num2))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyReadInputRegs(num2, o_byPacket, o_iLen, o_btData, out o_byteData))
					{
						return true;
					}
					base.LastError = ModbusTCP.LastError;
					return false;
				}
				base.LastError = ErrorCode.Socket_Recv_Fail;
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		o_byteData = null;
		return false;
	}

	public bool ReadInputRegs(int i_iStartIndex, int i_iTotalPoint, out int[] o_iData)
	{
		if (ReadInputRegs(i_iStartIndex, i_iTotalPoint, out byte[] o_byteData))
		{
			o_iData = new int[i_iTotalPoint];
			for (int i = 0; i < i_iTotalPoint; i++)
			{
				o_iData[i] = o_byteData[i * 2] * 256 + o_byteData[i * 2 + 1];
			}
			return true;
		}
		o_iData = null;
		return false;
	}

	public bool ForceSingleCoil(int i_iCoilIndex, int i_iData)
	{
		byte[] o_btData = new byte[1200];
		int o_iLen = 0;
		int num = 0;
		byte[] o_byPacket;
		if (m_com != null)
		{
			ModbusRTU.ConstructForceSingleCoil(base.Address, i_iCoilIndex, i_iData, out o_byPacket);
			int num2 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num2, o_byPacket) == num2)
			{
				num = 8;
				o_iLen = m_com.Recv(num, ref o_btData);
				if (ModbusRTU.VerifyForceSingleCoil(num2, o_byPacket, o_iLen, o_btData))
				{
					return true;
				}
				base.LastError = ModbusRTU.LastError;
				return false;
			}
			base.LastError = ErrorCode.ComPort_Send_Fail;
		}
		else if (m_socket != null)
		{
			ModbusTCP.ConstructForceSingleCoil(base.Address, i_iCoilIndex, i_iData, out o_byPacket);
			int num2 = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num2))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyForceSingleCoil(num2, o_byPacket, o_iLen, o_btData))
					{
						return true;
					}
					base.LastError = ModbusTCP.LastError;
					return false;
				}
				base.LastError = ErrorCode.Socket_Recv_Fail;
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		return false;
	}

	public bool ForceSingleCoil(int i_iCoilIndex, bool i_bData)
	{
		if (i_bData)
		{
			return ForceSingleCoil(i_iCoilIndex, 1);
		}
		return ForceSingleCoil(i_iCoilIndex, 0);
	}

	public bool PresetSingleReg(int i_iRegIndex, int i_iData)
	{
		byte[] o_btData = new byte[1200];
		int o_iLen = 0;
		int num = 0;
		byte[] o_byPacket;
		if (m_com != null)
		{
			ModbusRTU.ConstructPresetSingleReg(base.Address, i_iRegIndex, i_iData, out o_byPacket);
			int num2 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num2, o_byPacket) == num2)
			{
				num = 8;
				o_iLen = m_com.Recv(num, ref o_btData);
				if (ModbusRTU.VerifyPresetSingleReg(num2, o_byPacket, o_iLen, o_btData))
				{
					return true;
				}
				base.LastError = ModbusRTU.LastError;
				return false;
			}
			base.LastError = ErrorCode.ComPort_Send_Fail;
		}
		else if (m_socket != null)
		{
			ModbusTCP.ConstructPresetSingleReg(base.Address, i_iRegIndex, i_iData, out o_byPacket);
			int num2 = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num2))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyPresetSingleReg(num2, o_byPacket, o_iLen, o_btData))
					{
						return true;
					}
					base.LastError = ModbusTCP.LastError;
					return false;
				}
				base.LastError = ErrorCode.Socket_Recv_Fail;
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		return false;
	}

	public bool ForceMultiCoils(int i_iStartCoil, int i_iTotalPoint, int i_iTotalByte, byte[] i_byData)
	{
		byte[] o_btData = new byte[1200];
		int o_iLen = 0;
		int num = 0;
		base.LastError = ErrorCode.No_Error;
		byte[] o_byPacket;
		if (m_com != null)
		{
			ModbusRTU.ConstructForceMultiCoils(base.Address, i_iStartCoil, i_iTotalPoint, i_iTotalByte, i_byData, out o_byPacket);
			int num2 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num2, o_byPacket) == num2)
			{
				num = 8;
				o_iLen = m_com.Recv(num, ref o_btData);
				if (ModbusRTU.VerifyForceMultiCoils(num2, o_byPacket, o_iLen, o_btData))
				{
					return true;
				}
				base.LastError = ModbusRTU.LastError;
				return false;
			}
			base.LastError = ErrorCode.ComPort_Send_Fail;
		}
		else if (m_socket != null)
		{
			ModbusTCP.ConstructForceMultiCoils(base.Address, i_iStartCoil, i_iTotalPoint, i_iTotalByte, i_byData, out o_byPacket);
			int num2 = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num2))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyForceSingleCoil(num2, o_byPacket, o_iLen, o_btData))
					{
						return true;
					}
					base.LastError = ModbusTCP.LastError;
					return false;
				}
				base.LastError = ErrorCode.Socket_Recv_Fail;
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		return false;
	}

	public bool ForceMultiCoils(int i_iStartCoil, bool[] i_bData)
	{
		int num = i_bData.Length;
		int num2 = ((num % 8 <= 0) ? (num / 8) : (num / 8 + 1));
		byte[] array = new byte[num2];
		for (int i = 0; i < num; i++)
		{
			int num3 = i / 8;
			int num4 = i % 8;
			if (i_bData[i])
			{
				array[num3] += Convert.ToByte(1 << num4);
			}
		}
		return ForceMultiCoils(i_iStartCoil, num, num2, array);
	}

	public bool PresetMultiRegs(int i_iStartReg, int i_iTotalReg, int i_iTotalByte, byte[] i_byData)
	{
		byte[] o_btData = new byte[1200];
		int o_iLen = 0;
		int num = 0;
		base.LastError = ErrorCode.No_Error;
		byte[] o_byPacket;
		if (m_com != null)
		{
			ModbusRTU.ConstructPresetMultiRegs(base.Address, i_iStartReg, i_iTotalReg, i_iTotalByte, i_byData, out o_byPacket);
			int num2 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num2, o_byPacket) == num2)
			{
				num = 8;
				o_iLen = m_com.Recv(num, ref o_btData);
				if (ModbusRTU.VerifyPresetMultiRegs(num2, o_byPacket, o_iLen, o_btData))
				{
					return true;
				}
				base.LastError = ModbusRTU.LastError;
				return false;
			}
			base.LastError = ErrorCode.ComPort_Send_Fail;
		}
		else if (m_socket != null)
		{
			ModbusTCP.ConstructPresetMultiRegs(base.Address, i_iStartReg, i_iTotalReg, i_iTotalByte, i_byData, out o_byPacket);
			int num2 = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num2))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyPresetMultiRegs(num2, o_byPacket, o_iLen, o_btData))
					{
						return true;
					}
					base.LastError = ModbusTCP.LastError;
					return false;
				}
				base.LastError = ErrorCode.Socket_Recv_Fail;
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		return false;
	}

	public bool PresetMultiRegs(int i_iStartReg, int[] i_iData)
	{
		int num = i_iData.Length;
		int num2 = num * 2;
		byte[] array = new byte[num2];
		for (int i = 0; i < num; i++)
		{
			int num3 = ((i_iData[i] <= 65535) ? ((i_iData[i] >= 0) ? i_iData[i] : 0) : 65535);
			array[i * 2] = Convert.ToByte(num3 / 256);
			array[i * 2 + 1] = Convert.ToByte(num3 % 256);
		}
		return PresetMultiRegs(i_iStartReg, num, num2, array);
	}

	public bool ReadAdvantechRegs(int i_iStartIndex, int i_iTotalPoint, out byte[] o_byteData)
	{
		byte[] o_btData = new byte[1200];
		int o_iLen = 0;
		int num = 0;
		base.LastError = ErrorCode.No_Error;
		byte[] o_byPacket;
		if (m_com != null)
		{
			ModbusRTU.ConstructReadAdvantechRegs(base.Address, i_iStartIndex, i_iTotalPoint, out o_byPacket);
			int num2 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num2, o_byPacket) == num2)
			{
				num = i_iTotalPoint * 2 + 5;
				o_iLen = m_com.Recv(num, ref o_btData);
				if (ModbusRTU.VerifyReadAdvantechRegs(num2, o_byPacket, o_iLen, o_btData, out o_byteData))
				{
					return true;
				}
				base.LastError = ModbusRTU.LastError;
				return false;
			}
			base.LastError = ErrorCode.ComPort_Send_Fail;
		}
		else if (m_socket != null)
		{
			ModbusTCP.ConstructReadAdvantechRegs(base.Address, i_iStartIndex, i_iTotalPoint, out o_byPacket);
			int num2 = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num2))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyReadAdvantechRegs(num2, o_byPacket, o_iLen, o_btData, out o_byteData))
					{
						return true;
					}
					base.LastError = ModbusTCP.LastError;
					return false;
				}
				base.LastError = ErrorCode.Socket_Recv_Fail;
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		o_byteData = null;
		return false;
	}

	public bool ReadAdvantechRegs(int i_iStartIndex, int i_iTotalPoint, out int[] o_iData)
	{
		if (ReadAdvantechRegs(i_iStartIndex, i_iTotalPoint, out byte[] o_byteData))
		{
			o_iData = new int[i_iTotalPoint];
			for (int i = 0; i < i_iTotalPoint; i++)
			{
				o_iData[i] = o_byteData[i * 2] * 256 + o_byteData[i * 2 + 1];
			}
			return true;
		}
		o_iData = null;
		return false;
	}
}
