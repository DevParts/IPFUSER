using System;
using Advantech.Common;
using Advantech.Protocol;

namespace Advantech.Adam;

public class PID : AdamBase
{
	private const int ADAM_MAX_MSGLEN = 1200;

	private AdamCom m_com;

	private AdamSocket m_socket;

	private int m_iPIDLoop0Start;

	private int m_iPIDLoop1Start;

	private int[] m_iPidLoop0Value;

	private int[] m_iPidLoop1Value;

	public PID(AdamCom i_com)
		: base(i_com)
	{
		m_iPidLoop0Value = null;
		m_iPidLoop1Value = null;
		m_com = i_com;
		m_socket = null;
		m_iPIDLoop0Start = 1000;
		m_iPIDLoop1Start = 1256;
	}

	public PID(AdamSocket i_socket)
		: base(i_socket)
	{
		m_iPidLoop0Value = null;
		m_iPidLoop1Value = null;
		m_com = null;
		m_socket = i_socket;
		m_iPIDLoop0Start = 1000;
		m_iPIDLoop1Start = 1256;
	}

	public bool ModbusRefreshBuffer(PID_Loop i_loop)
	{
		int num = 50;
		int num2 = 100;
		byte[] o_btData = new byte[1200];
		byte[] array = new byte[4];
		int o_iLen = 0;
		byte[] o_byPacket;
		byte[] o_byRecvData;
		if (i_loop == PID_Loop.Loop0)
		{
			if (m_iPidLoop0Value == null)
			{
				m_iPidLoop0Value = new int[128];
			}
			if (m_socket != null)
			{
				for (int i = 0; i < 2; i++)
				{
					int i_iStartIndex = m_iPIDLoop0Start + num2 * i;
					ModbusTCP.ConstructReadInputRegs(base.Address, i_iStartIndex, num2, out o_byPacket);
					int num3 = o_byPacket.Length;
					if (m_socket.Send(o_byPacket, num3))
					{
						if (m_socket.Receive(o_btData, out o_iLen))
						{
							if (ModbusTCP.VerifyReadInputRegs(num3, o_byPacket, o_iLen, o_btData, out o_byRecvData))
							{
								for (int j = 0; j < num; j++)
								{
									array[0] = o_byRecvData[j * 4 + 3];
									array[1] = o_byRecvData[j * 4 + 2];
									array[2] = o_byRecvData[j * 4 + 1];
									array[3] = o_byRecvData[j * 4];
									m_iPidLoop0Value[i * num + j] = BitConverter.ToInt32(array, 0);
								}
								continue;
							}
							base.LastError = ModbusTCP.LastError;
							return false;
						}
						base.LastError = ErrorCode.Socket_Recv_Fail;
						return false;
					}
					base.LastError = ErrorCode.Socket_Send_Fail;
					return false;
				}
			}
			else
			{
				for (int i = 0; i < 2; i++)
				{
					int i_iStartIndex = m_iPIDLoop0Start + num2 * i;
					ModbusRTU.ConstructReadInputRegs(base.Address, i_iStartIndex, num2, out o_byPacket);
					int num3 = o_byPacket.Length;
					m_com.SetPurge(Convert.ToInt32((Purge)12));
					if (m_com.Send(num3, o_byPacket) == num3)
					{
						o_iLen = m_com.Recv(num2 * 2 + 5, ref o_btData);
						if (o_iLen > 0)
						{
							if (ModbusRTU.VerifyReadInputRegs(num3, o_byPacket, o_iLen, o_btData, out o_byRecvData))
							{
								for (int j = 0; j < num; j++)
								{
									array[0] = o_byRecvData[j * 4 + 3];
									array[1] = o_byRecvData[j * 4 + 2];
									array[2] = o_byRecvData[j * 4 + 1];
									array[3] = o_byRecvData[j * 4];
									m_iPidLoop0Value[i * num + j] = BitConverter.ToInt32(array, 0);
								}
								continue;
							}
							base.LastError = ModbusRTU.LastError;
							return false;
						}
						base.LastError = ErrorCode.ComPort_Recv_Fail;
						return false;
					}
					base.LastError = ErrorCode.ComPort_Send_Fail;
					return false;
				}
			}
		}
		else
		{
			if (m_iPidLoop1Value == null)
			{
				m_iPidLoop1Value = new int[128];
			}
			if (m_socket != null)
			{
				for (int i = 0; i < 2; i++)
				{
					int i_iStartIndex = m_iPIDLoop1Start + num2 * i;
					ModbusTCP.ConstructReadInputRegs(base.Address, i_iStartIndex, num2, out o_byPacket);
					int num3 = o_byPacket.Length;
					if (m_socket.Send(o_byPacket, num3))
					{
						if (m_socket.Receive(o_btData, out o_iLen))
						{
							if (ModbusTCP.VerifyReadInputRegs(num3, o_byPacket, o_iLen, o_btData, out o_byRecvData))
							{
								for (int j = 0; j < num; j++)
								{
									array[0] = o_byRecvData[j * 4 + 3];
									array[1] = o_byRecvData[j * 4 + 2];
									array[2] = o_byRecvData[j * 4 + 1];
									array[3] = o_byRecvData[j * 4];
									m_iPidLoop1Value[i * num + j] = BitConverter.ToInt32(array, 0);
								}
								continue;
							}
							base.LastError = ModbusTCP.LastError;
							return false;
						}
						base.LastError = ErrorCode.Socket_Recv_Fail;
						return false;
					}
					base.LastError = ErrorCode.Socket_Send_Fail;
					return false;
				}
			}
			else
			{
				for (int i = 0; i < 2; i++)
				{
					int i_iStartIndex = m_iPIDLoop1Start + num2 * i;
					ModbusRTU.ConstructReadInputRegs(base.Address, i_iStartIndex, num2, out o_byPacket);
					int num3 = o_byPacket.Length;
					m_com.SetPurge(Convert.ToInt32((Purge)12));
					if (m_com.Send(num3, o_byPacket) == num3)
					{
						o_iLen = m_com.Recv(num2 * 2 + 5, ref o_btData);
						if (o_iLen > 0)
						{
							if (ModbusRTU.VerifyReadInputRegs(num3, o_byPacket, o_iLen, o_btData, out o_byRecvData))
							{
								for (int j = 0; j < num; j++)
								{
									array[0] = o_byRecvData[j * 4 + 3];
									array[1] = o_byRecvData[j * 4 + 2];
									array[2] = o_byRecvData[j * 4 + 1];
									array[3] = o_byRecvData[j * 4];
									m_iPidLoop1Value[i * num + j] = BitConverter.ToInt32(array, 0);
								}
								continue;
							}
							base.LastError = ModbusRTU.LastError;
							return false;
						}
						base.LastError = ErrorCode.ComPort_Recv_Fail;
						return false;
					}
					base.LastError = ErrorCode.ComPort_Send_Fail;
					return false;
				}
			}
		}
		return true;
	}

	public bool ModbusBufferToModule(PID_Loop i_loop)
	{
		int num = 50;
		int num2 = 100;
		byte[] o_btData = new byte[1200];
		byte[] array = new byte[num2 * 2];
		int o_iLen = 0;
		byte[] o_byPacket;
		if (i_loop == PID_Loop.Loop0)
		{
			if (m_iPidLoop0Value == null)
			{
				return false;
			}
			if (m_socket != null)
			{
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < num; j++)
					{
						byte[] bytes = BitConverter.GetBytes(m_iPidLoop0Value[i * num + j]);
						array[j * 4] = bytes[3];
						array[j * 4 + 1] = bytes[2];
						array[j * 4 + 2] = bytes[1];
						array[j * 4 + 3] = bytes[0];
					}
					int i_iStartReg = m_iPIDLoop0Start + num2 * i;
					ModbusTCP.ConstructPresetMultiRegs(base.Address, i_iStartReg, num2, num2 * 2, array, out o_byPacket);
					int num3 = o_byPacket.Length;
					if (m_socket.Send(o_byPacket, num3))
					{
						if (m_socket.Receive(o_btData, out o_iLen))
						{
							if (!ModbusTCP.VerifyPresetMultiRegs(num3, o_byPacket, o_iLen, o_btData))
							{
								base.LastError = ModbusTCP.LastError;
								return false;
							}
							continue;
						}
						base.LastError = ErrorCode.Socket_Recv_Fail;
						return false;
					}
					base.LastError = ErrorCode.Socket_Send_Fail;
					return false;
				}
			}
			else
			{
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < num; j++)
					{
						byte[] bytes = BitConverter.GetBytes(m_iPidLoop0Value[i * num + j]);
						array[j * 4] = bytes[3];
						array[j * 4 + 1] = bytes[2];
						array[j * 4 + 2] = bytes[1];
						array[j * 4 + 3] = bytes[0];
					}
					int i_iStartReg = m_iPIDLoop0Start + num2 * i;
					ModbusRTU.ConstructPresetMultiRegs(base.Address, i_iStartReg, num2, num2 * 2, array, out o_byPacket);
					int num3 = o_byPacket.Length;
					m_com.SetPurge(Convert.ToInt32((Purge)12));
					if (m_com.Send(num3, o_byPacket) == num3)
					{
						o_iLen = m_com.Recv(8, ref o_btData);
						if (o_iLen > 0)
						{
							if (!ModbusRTU.VerifyPresetMultiRegs(num3, o_byPacket, o_iLen, o_btData))
							{
								base.LastError = ModbusRTU.LastError;
								return false;
							}
							continue;
						}
						base.LastError = ErrorCode.ComPort_Recv_Fail;
						return false;
					}
					base.LastError = ErrorCode.ComPort_Send_Fail;
					return false;
				}
			}
		}
		else
		{
			if (m_iPidLoop1Value == null)
			{
				return false;
			}
			if (m_socket != null)
			{
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < num; j++)
					{
						byte[] bytes = BitConverter.GetBytes(m_iPidLoop1Value[i * num + j]);
						array[j * 4] = bytes[3];
						array[j * 4 + 1] = bytes[2];
						array[j * 4 + 2] = bytes[1];
						array[j * 4 + 3] = bytes[0];
					}
					int i_iStartReg = m_iPIDLoop1Start + num2 * i;
					ModbusTCP.ConstructPresetMultiRegs(base.Address, i_iStartReg, num2, num2 * 2, array, out o_byPacket);
					int num3 = o_byPacket.Length;
					if (m_socket.Send(o_byPacket, num3))
					{
						if (m_socket.Receive(o_btData, out o_iLen))
						{
							if (!ModbusTCP.VerifyPresetMultiRegs(num3, o_byPacket, o_iLen, o_btData))
							{
								base.LastError = ModbusTCP.LastError;
								return false;
							}
							continue;
						}
						base.LastError = ErrorCode.Socket_Recv_Fail;
						return false;
					}
					base.LastError = ErrorCode.Socket_Send_Fail;
					return false;
				}
			}
			else
			{
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < num; j++)
					{
						byte[] bytes = BitConverter.GetBytes(m_iPidLoop1Value[i * num + j]);
						array[j * 4] = bytes[3];
						array[j * 4 + 1] = bytes[2];
						array[j * 4 + 2] = bytes[1];
						array[j * 4 + 3] = bytes[0];
					}
					int i_iStartReg = m_iPIDLoop1Start + num2 * i;
					ModbusRTU.ConstructPresetMultiRegs(base.Address, i_iStartReg, num2, num2 * 2, array, out o_byPacket);
					int num3 = o_byPacket.Length;
					m_com.SetPurge(Convert.ToInt32((Purge)12));
					if (m_com.Send(num3, o_byPacket) == num3)
					{
						o_iLen = m_com.Recv(8, ref o_btData);
						if (o_iLen > 0)
						{
							if (!ModbusRTU.VerifyPresetMultiRegs(num3, o_byPacket, o_iLen, o_btData))
							{
								base.LastError = ModbusRTU.LastError;
								return false;
							}
							continue;
						}
						base.LastError = ErrorCode.ComPort_Recv_Fail;
						return false;
					}
					base.LastError = ErrorCode.ComPort_Send_Fail;
					return false;
				}
			}
		}
		return true;
	}

	public bool AsciiRefreshBuffer(PID_Loop i_loop)
	{
		int num = 50;
		string o_szRecv;
		if (i_loop == PID_Loop.Loop0)
		{
			if (m_iPidLoop0Value == null)
			{
				m_iPidLoop0Value = new int[128];
			}
			for (int i = 0; i < 2; i++)
			{
				int num2 = num * i;
				string i_szSend = "#" + base.Address.ToString("X02") + "PR" + num2.ToString("X02") + num.ToString("X02") + "\r";
				if (ASCIISendRecv(i_szSend, out o_szRecv) && o_szRecv.Length > 2)
				{
					try
					{
						string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
						if (text.Length == num * 8)
						{
							for (int j = 0; j < num; j++)
							{
								string value = text.Substring(j * 8, 8);
								m_iPidLoop0Value[num * i + j] = Convert.ToInt32(value, 16);
							}
							continue;
						}
						base.LastError = ErrorCode.Adam_Invalid_Length;
						return false;
					}
					catch
					{
						base.LastError = ErrorCode.Adam_Invalid_Data;
						return false;
					}
				}
				return false;
			}
		}
		else
		{
			if (m_iPidLoop1Value == null)
			{
				m_iPidLoop1Value = new int[128];
			}
			for (int i = 0; i < 2; i++)
			{
				int num2 = num * i + 128;
				string i_szSend = "#" + base.Address.ToString("X02") + "PR" + num2.ToString("X02") + num.ToString("X02") + "\r";
				if (ASCIISendRecv(i_szSend, out o_szRecv) && o_szRecv.Length > 2)
				{
					try
					{
						string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
						if (text.Length == num * 8)
						{
							for (int j = 0; j < num; j++)
							{
								string value = text.Substring(j * 8, 8);
								m_iPidLoop1Value[num * i + j] = Convert.ToInt32(value, 16);
							}
							continue;
						}
						base.LastError = ErrorCode.Adam_Invalid_Length;
						return false;
					}
					catch
					{
						base.LastError = ErrorCode.Adam_Invalid_Data;
						return false;
					}
				}
				return false;
			}
		}
		return true;
	}

	public bool AsciiBufferToModule(PID_Loop i_loop)
	{
		int num = 50;
		string o_szRecv;
		if (i_loop == PID_Loop.Loop0)
		{
			if (m_iPidLoop0Value == null)
			{
				base.LastError = ErrorCode.Adam_Null_Error;
				return false;
			}
			for (int i = 0; i < 2; i++)
			{
				int num2 = num * i;
				for (int j = 0; j < num; j++)
				{
					int num3 = num2 + j;
					int num4 = m_iPidLoop0Value[i * num + j];
					string i_szSend = "#" + base.Address.ToString("X02") + "PW" + num3.ToString("X02") + num4.ToString("X08") + "\r";
					if (!ASCIISendRecv(i_szSend, out o_szRecv))
					{
						return false;
					}
				}
			}
		}
		else
		{
			if (m_iPidLoop1Value == null)
			{
				base.LastError = ErrorCode.Adam_Null_Error;
				return false;
			}
			for (int i = 0; i < 2; i++)
			{
				int num2 = num * i + 128;
				for (int j = 0; j < num; j++)
				{
					int num3 = num2 + j;
					int num4 = m_iPidLoop1Value[i * num + j];
					string i_szSend = "#" + base.Address.ToString("X02") + "PW" + num3.ToString("X02") + num.ToString("X08") + "\r";
					if (!ASCIISendRecv(i_szSend, out o_szRecv))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public int GetBufferInt(PID_Loop i_loop, PID_Addr i_addr)
	{
		if (i_loop == PID_Loop.Loop0 && m_iPidLoop0Value != null)
		{
			return m_iPidLoop0Value[Convert.ToInt32(i_addr)];
		}
		if (i_loop == PID_Loop.Loop1 && m_iPidLoop1Value != null)
		{
			return m_iPidLoop1Value[Convert.ToInt32(i_addr)];
		}
		return 0;
	}

	public void SetBufferInt(PID_Loop i_loop, PID_Addr i_addr, int i_iValue)
	{
		if (i_loop == PID_Loop.Loop0)
		{
			if (m_iPidLoop0Value == null)
			{
				m_iPidLoop0Value = new int[128];
			}
			m_iPidLoop0Value[Convert.ToInt32(i_addr)] = i_iValue;
		}
		else
		{
			if (m_iPidLoop1Value == null)
			{
				m_iPidLoop1Value = new int[128];
			}
			m_iPidLoop1Value[Convert.ToInt32(i_addr)] = i_iValue;
		}
	}

	public float GetBufferFloat(PID_Loop i_loop, PID_Addr i_addr)
	{
		int value = ((i_loop == PID_Loop.Loop0 && m_iPidLoop0Value != null) ? m_iPidLoop0Value[Convert.ToInt32(i_addr)] : ((i_loop == PID_Loop.Loop1 && m_iPidLoop1Value != null) ? m_iPidLoop1Value[Convert.ToInt32(i_addr)] : 0));
		float num = Convert.ToSingle(value);
		return num / 1000f;
	}

	public void SetBufferFloat(PID_Loop i_loop, PID_Addr i_addr, float i_fValue)
	{
		if (i_loop == PID_Loop.Loop0)
		{
			if (m_iPidLoop0Value == null)
			{
				m_iPidLoop0Value = new int[128];
			}
			m_iPidLoop0Value[Convert.ToInt32(i_addr)] = Convert.ToInt32(i_fValue * 1000f);
		}
		else
		{
			if (m_iPidLoop1Value == null)
			{
				m_iPidLoop1Value = new int[128];
			}
			m_iPidLoop1Value[Convert.ToInt32(i_addr)] = Convert.ToInt32(i_fValue * 1000f);
		}
	}

	public bool ModbusSetValue(PID_Loop i_loop, PID_Addr i_addr, int i_iValue)
	{
		int i_iTotalReg = 2;
		int i_iTotalByte = 4;
		byte[] o_btData = new byte[1200];
		byte[] array = new byte[4];
		int o_iLen = 0;
		byte[] o_byPacket;
		if (m_socket != null)
		{
			int i_iStartReg = ((i_loop != PID_Loop.Loop0) ? (m_iPIDLoop1Start + Convert.ToInt32(i_addr) * 2) : (m_iPIDLoop0Start + Convert.ToInt32(i_addr) * 2));
			byte[] bytes = BitConverter.GetBytes(i_iValue);
			array[0] = bytes[3];
			array[1] = bytes[2];
			array[2] = bytes[1];
			array[3] = bytes[0];
			ModbusTCP.ConstructPresetMultiRegs(base.Address, i_iStartReg, i_iTotalReg, i_iTotalByte, array, out o_byPacket);
			int num = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyPresetMultiRegs(num, o_byPacket, o_iLen, o_btData))
					{
						return true;
					}
					base.LastError = ModbusTCP.LastError;
				}
				else
				{
					base.LastError = ErrorCode.Socket_Recv_Fail;
				}
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		else
		{
			int i_iStartReg = ((i_loop != PID_Loop.Loop0) ? (m_iPIDLoop1Start + Convert.ToInt32(i_addr) * 2) : (m_iPIDLoop0Start + Convert.ToInt32(i_addr) * 2));
			byte[] bytes = BitConverter.GetBytes(i_iValue);
			array[0] = bytes[3];
			array[1] = bytes[2];
			array[2] = bytes[1];
			array[3] = bytes[0];
			ModbusRTU.ConstructPresetMultiRegs(base.Address, i_iStartReg, i_iTotalReg, i_iTotalByte, array, out o_byPacket);
			int num = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num, o_byPacket) == num)
			{
				int i_i32Len = 8;
				o_iLen = m_com.Recv(i_i32Len, ref o_btData);
				if (o_iLen > 0)
				{
					if (ModbusRTU.VerifyPresetMultiRegs(num, o_byPacket, o_iLen, o_btData))
					{
						return true;
					}
					base.LastError = ModbusRTU.LastError;
				}
				else
				{
					base.LastError = ErrorCode.ComPort_Recv_Fail;
				}
			}
			else
			{
				base.LastError = ErrorCode.ComPort_Send_Fail;
			}
		}
		return false;
	}

	public bool ModbusGetValue(PID_Loop i_loop, PID_Addr i_addr, out int o_iValue)
	{
		int num = 2;
		byte[] o_btData = new byte[1200];
		byte[] array = new byte[4];
		int o_iLen = 0;
		int num2 = 0;
		byte[] o_byPacket;
		byte[] o_byRecvData;
		if (m_socket != null)
		{
			ModbusTCP.ConstructReadInputRegs(i_iStartIndex: (i_loop != PID_Loop.Loop0) ? (m_iPIDLoop1Start + Convert.ToInt32(i_addr) * 2) : (m_iPIDLoop0Start + Convert.ToInt32(i_addr) * 2), i_iAddr: base.Address, i_iTotalPoint: num, o_byPacket: out o_byPacket);
			int num3 = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num3))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyReadInputRegs(num3, o_byPacket, o_iLen, o_btData, out o_byRecvData))
					{
						array[0] = o_byRecvData[3];
						array[1] = o_byRecvData[2];
						array[2] = o_byRecvData[1];
						array[3] = o_byRecvData[0];
						o_iValue = BitConverter.ToInt32(array, 0);
						return true;
					}
					base.LastError = ModbusTCP.LastError;
				}
				else
				{
					base.LastError = ErrorCode.Socket_Recv_Fail;
				}
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		else
		{
			ModbusRTU.ConstructReadInputRegs(i_iStartIndex: (i_loop != PID_Loop.Loop0) ? (m_iPIDLoop1Start + Convert.ToInt32(i_addr) * 2) : (m_iPIDLoop0Start + Convert.ToInt32(i_addr) * 2), i_iAddr: base.Address, i_iTotalPoint: num, o_byPacket: out o_byPacket);
			int num3 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num3, o_byPacket) == num3)
			{
				num2 = num * 2 + 5;
				o_iLen = m_com.Recv(num2, ref o_btData);
				if (o_iLen > 0)
				{
					if (ModbusRTU.VerifyReadInputRegs(num3, o_byPacket, o_iLen, o_btData, out o_byRecvData))
					{
						array[0] = o_byRecvData[3];
						array[1] = o_byRecvData[2];
						array[2] = o_byRecvData[1];
						array[3] = o_byRecvData[0];
						o_iValue = BitConverter.ToInt32(array, 0);
						return true;
					}
					base.LastError = ModbusRTU.LastError;
				}
				else
				{
					base.LastError = ErrorCode.ComPort_Recv_Fail;
				}
			}
			else
			{
				base.LastError = ErrorCode.ComPort_Send_Fail;
			}
		}
		o_iValue = 0;
		return false;
	}

	public bool ModbusSetValue(PID_Loop i_loop, PID_Addr i_addr, float i_fValue)
	{
		int i_iTotalReg = 2;
		int i_iTotalByte = 4;
		byte[] o_btData = new byte[1200];
		byte[] array = new byte[4];
		int o_iLen = 0;
		int num = 0;
		byte[] o_byPacket;
		if (m_socket != null)
		{
			int i_iStartReg = ((i_loop != PID_Loop.Loop0) ? (m_iPIDLoop1Start + Convert.ToInt32(i_addr) * 2) : (m_iPIDLoop0Start + Convert.ToInt32(i_addr) * 2));
			i_fValue *= 1000f;
			int value = Convert.ToInt32(i_fValue);
			byte[] bytes = BitConverter.GetBytes(value);
			array[0] = bytes[3];
			array[1] = bytes[2];
			array[2] = bytes[1];
			array[3] = bytes[0];
			ModbusTCP.ConstructPresetMultiRegs(base.Address, i_iStartReg, i_iTotalReg, i_iTotalByte, array, out o_byPacket);
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
				}
				else
				{
					base.LastError = ErrorCode.Socket_Recv_Fail;
				}
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		else
		{
			int i_iStartReg = ((i_loop != PID_Loop.Loop0) ? (m_iPIDLoop1Start + Convert.ToInt32(i_addr) * 2) : (m_iPIDLoop0Start + Convert.ToInt32(i_addr) * 2));
			i_fValue *= 1000f;
			int value = Convert.ToInt32(i_fValue);
			byte[] bytes = BitConverter.GetBytes(value);
			array[0] = bytes[3];
			array[1] = bytes[2];
			array[2] = bytes[1];
			array[3] = bytes[0];
			ModbusRTU.ConstructPresetMultiRegs(base.Address, i_iStartReg, i_iTotalReg, i_iTotalByte, array, out o_byPacket);
			int num2 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num2, o_byPacket) == num2)
			{
				num = 8;
				o_iLen = m_com.Recv(num, ref o_btData);
				if (o_iLen > 0)
				{
					if (ModbusRTU.VerifyPresetMultiRegs(num2, o_byPacket, o_iLen, o_btData))
					{
						return true;
					}
					base.LastError = ModbusRTU.LastError;
				}
				else
				{
					base.LastError = ErrorCode.ComPort_Recv_Fail;
				}
			}
			else
			{
				base.LastError = ErrorCode.ComPort_Send_Fail;
			}
		}
		return false;
	}

	public bool ModbusGetValue(PID_Loop i_loop, PID_Addr i_addr, out float o_fValue)
	{
		int num = 2;
		byte[] o_btData = new byte[1200];
		byte[] array = new byte[4];
		int o_iLen = 0;
		int num2 = 0;
		byte[] o_byPacket;
		byte[] o_byRecvData;
		if (m_socket != null)
		{
			ModbusTCP.ConstructReadInputRegs(i_iStartIndex: (i_loop != PID_Loop.Loop0) ? (m_iPIDLoop1Start + Convert.ToInt32(i_addr) * 2) : (m_iPIDLoop0Start + Convert.ToInt32(i_addr) * 2), i_iAddr: base.Address, i_iTotalPoint: num, o_byPacket: out o_byPacket);
			int num3 = o_byPacket.Length;
			if (m_socket.Send(o_byPacket, num3))
			{
				if (m_socket.Receive(o_btData, out o_iLen))
				{
					if (ModbusTCP.VerifyReadInputRegs(num3, o_byPacket, o_iLen, o_btData, out o_byRecvData))
					{
						array[0] = o_byRecvData[3];
						array[1] = o_byRecvData[2];
						array[2] = o_byRecvData[1];
						array[3] = o_byRecvData[0];
						int value = BitConverter.ToInt32(array, 0);
						o_fValue = Convert.ToSingle(value);
						o_fValue /= 1000f;
						return true;
					}
					base.LastError = ModbusTCP.LastError;
				}
				else
				{
					base.LastError = ErrorCode.Socket_Recv_Fail;
				}
			}
			else
			{
				base.LastError = ErrorCode.Socket_Send_Fail;
			}
		}
		else
		{
			ModbusRTU.ConstructReadInputRegs(i_iStartIndex: (i_loop != PID_Loop.Loop0) ? (m_iPIDLoop1Start + Convert.ToInt32(i_addr) * 2) : (m_iPIDLoop0Start + Convert.ToInt32(i_addr) * 2), i_iAddr: base.Address, i_iTotalPoint: num, o_byPacket: out o_byPacket);
			int num3 = o_byPacket.Length;
			m_com.SetPurge(Convert.ToInt32((Purge)12));
			if (m_com.Send(num3, o_byPacket) == num3)
			{
				num2 = num * 2 + 5;
				o_iLen = m_com.Recv(num2, ref o_btData);
				if (o_iLen > 0)
				{
					if (ModbusRTU.VerifyReadInputRegs(num3, o_byPacket, o_iLen, o_btData, out o_byRecvData))
					{
						array[0] = o_byRecvData[3];
						array[1] = o_byRecvData[2];
						array[2] = o_byRecvData[1];
						array[3] = o_byRecvData[0];
						int value = BitConverter.ToInt32(array, 0);
						o_fValue = Convert.ToSingle(value);
						o_fValue /= 1000f;
						return true;
					}
					base.LastError = ModbusRTU.LastError;
				}
				else
				{
					base.LastError = ErrorCode.ComPort_Recv_Fail;
				}
			}
			else
			{
				base.LastError = ErrorCode.ComPort_Send_Fail;
			}
		}
		o_fValue = 0f;
		return false;
	}

	public bool AsciiSetValue(PID_Loop i_loop, PID_Addr i_addr, int i_iValue)
	{
		int num = ((i_loop != PID_Loop.Loop0) ? (128 + Convert.ToInt32(i_addr)) : Convert.ToInt32(i_addr));
		string i_szSend = "#" + base.Address.ToString("X02") + "PW" + num.ToString("X02") + i_iValue.ToString("X08") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool AsciiGetValue(PID_Loop i_loop, PID_Addr i_addr, out int o_iValue)
	{
		int num = ((i_loop != PID_Loop.Loop0) ? (128 + Convert.ToInt32(i_addr)) : Convert.ToInt32(i_addr));
		int num2 = 1;
		string i_szSend = "#" + base.Address.ToString("X02") + "PR" + num.ToString("X02") + num2.ToString("X02") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string value = o_szRecv.Substring(1, o_szRecv.Length - 2);
			try
			{
				o_iValue = Convert.ToInt32(value, 16);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_iValue = 0;
		return false;
	}

	public bool AsciiSetValue(PID_Loop i_loop, PID_Addr i_addr, float i_fValue)
	{
		int num = ((i_loop != PID_Loop.Loop0) ? (128 + Convert.ToInt32(i_addr)) : Convert.ToInt32(i_addr));
		i_fValue *= 1000f;
		int num2 = Convert.ToInt32(i_fValue);
		string i_szSend = "#" + base.Address.ToString("X02") + "PW" + num.ToString("X02") + num2.ToString("X08") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool AsciiGetValue(PID_Loop i_loop, PID_Addr i_addr, out float o_fValue)
	{
		int num = ((i_loop != PID_Loop.Loop0) ? (128 + Convert.ToInt32(i_addr)) : Convert.ToInt32(i_addr));
		int num2 = 1;
		string i_szSend = "#" + base.Address.ToString("X02") + "PR" + num.ToString("X02") + num2.ToString("X02") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string value = o_szRecv.Substring(1, o_szRecv.Length - 2);
			try
			{
				int value2 = Convert.ToInt32(value, 16);
				o_fValue = Convert.ToSingle(value2);
				o_fValue /= 1000f;
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_fValue = 0f;
		return false;
	}
}
