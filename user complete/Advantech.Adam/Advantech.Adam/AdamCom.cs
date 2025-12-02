using System;
using System.Text;
using Advantech.Common;

namespace Advantech.Adam;

public class AdamCom : ComPort
{
	private bool _bChkEnable;

	private ErrorCode m_error;

	private Configuration m_Config;

	private DigitalInput m_DI;

	private DigitalOutput m_DO;

	private AnalogInput m_AI;

	private AnalogOutput m_AO;

	private Modbus m_Modbus;

	private Alarm m_Alarm;

	private Counter m_Counter;

	private PID m_Pid;

	public ErrorCode LastError => m_error;

	public bool Checksum
	{
		get
		{
			return _bChkEnable;
		}
		set
		{
			_bChkEnable = value;
		}
	}

	protected uint ChecksumNumber(string i_szData)
	{
		uint num = 0u;
		byte[] bytes = Encoding.ASCII.GetBytes(i_szData);
		byte[] array = bytes;
		foreach (byte b in array)
		{
			num += b;
		}
		return num % 256;
	}

	protected void AppendChecksum(ref string io_szData)
	{
		string text = string.Copy(io_szData);
		int num = text.Length - 1;
		if (num >= 0 && text.Substring(num, 1) == "\r")
		{
			text = text.Remove(num, 1);
		}
		io_szData = text + ChecksumNumber(text).ToString("X02") + "\r";
	}

	protected bool VfyTrimChecksum(ref string io_szData)
	{
		bool result = false;
		string text = string.Copy(io_szData);
		int length = text.Length;
		if (length >= 4)
		{
			string value = text.Substring(length - 3, 2);
			io_szData = text.Substring(0, length - 3);
			uint num = (uint)Convert.ToInt32(value, 16);
			uint num2 = ChecksumNumber(io_szData);
			if (num == num2)
			{
				io_szData += "\r";
				result = true;
			}
			else
			{
				m_error = ErrorCode.Adam_Invalid_Checksum;
			}
		}
		else
		{
			m_error = ErrorCode.Adam_Invalid_Checksum;
		}
		return result;
	}

	public AdamCom(int i_i32Port)
		: base(i_i32Port)
	{
	}

	public Configuration Configuration(int i_iAddr)
	{
		if (m_Config == null)
		{
			m_Config = new Configuration(this);
		}
		m_Config.Address = i_iAddr;
		return m_Config;
	}

	public DigitalInput DigitalInput(int i_iAddr)
	{
		if (m_DI == null)
		{
			m_DI = new DigitalInput(this);
		}
		m_DI.Address = i_iAddr;
		return m_DI;
	}

	public DigitalOutput DigitalOutput(int i_iAddr)
	{
		if (m_DO == null)
		{
			m_DO = new DigitalOutput(this);
		}
		m_DO.Address = i_iAddr;
		return m_DO;
	}

	public AnalogInput AnalogInput(int i_iAddr)
	{
		if (m_AI == null)
		{
			m_AI = new AnalogInput(this);
		}
		m_AI.Address = i_iAddr;
		return m_AI;
	}

	public AnalogOutput AnalogOutput(int i_iAddr)
	{
		if (m_AO == null)
		{
			m_AO = new AnalogOutput(this);
		}
		m_AO.Address = i_iAddr;
		return m_AO;
	}

	public Modbus Modbus(int i_iAddr)
	{
		if (m_Modbus == null)
		{
			m_Modbus = new Modbus(this);
		}
		m_Modbus.Address = i_iAddr;
		return m_Modbus;
	}

	public Alarm Alarm(int i_iAddr)
	{
		if (m_Alarm == null)
		{
			m_Alarm = new Alarm(this);
		}
		m_Alarm.Address = i_iAddr;
		return m_Alarm;
	}

	public Counter Counter(int i_iAddr)
	{
		if (m_Counter == null)
		{
			m_Counter = new Counter(this);
		}
		m_Counter.Address = i_iAddr;
		return m_Counter;
	}

	public PID Pid(int i_iAddr)
	{
		if (m_Pid == null)
		{
			m_Pid = new PID(this);
		}
		m_Pid.Address = i_iAddr;
		return m_Pid;
	}

	public bool AdamTransaction(string i_szSend, out string o_szRecv)
	{
		string io_szData = string.Copy(i_szSend);
		bool result = false;
		m_error = ErrorCode.No_Error;
		o_szRecv = "";
		if (Checksum)
		{
			AppendChecksum(ref io_szData);
		}
		SetPurge(8);
		if (Send(io_szData) > 0)
		{
			if (Recv(out o_szRecv) > 0)
			{
				if (o_szRecv.Substring(0, 1) == "!" || o_szRecv.Substring(0, 1) == ">")
				{
					if (o_szRecv.Substring(o_szRecv.Length - 1, 1) == "\r")
					{
						result = !Checksum || VfyTrimChecksum(ref o_szRecv);
					}
					else
					{
						m_error = ErrorCode.Adam_Invalid_End;
					}
				}
				else
				{
					m_error = ErrorCode.Adam_Invalid_Head;
				}
			}
			else
			{
				m_error = ErrorCode.ComPort_Error;
			}
		}
		else
		{
			m_error = ErrorCode.ComPort_Error;
		}
		return result;
	}
}
