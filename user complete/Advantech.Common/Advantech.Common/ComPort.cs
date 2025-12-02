using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Advantech.Common;

public class ComPort
{
	internal struct DCB
	{
		internal int dcbLength;

		internal int baudRate;

		internal int dwFlag;

		internal short wReserved;

		internal short xonLim;

		internal short xoffLim;

		internal byte byteSize;

		internal byte prtyByte;

		internal byte stopBits;

		internal byte xonChar;

		internal byte xoffChar;

		internal byte errorChar;

		internal byte eofChar;

		internal byte evtChar;

		internal short wReserved1;
	}

	protected internal struct COMMTIMEOUTS
	{
		internal uint readIntervalTimeout;

		internal uint readTotalTimeoutMultiplier;

		internal uint readTotalTimeoutConstant;

		internal uint writeTotalTimeoutMultiplier;

		internal uint writeTotalTimeoutConstant;
	}

	private const string KernelDll = "kernel32.dll";

	internal const int INVALID_HANDLE_VALUE = -1;

	internal const uint GENERIC_READ = 2147483648u;

	internal const uint GENERIC_WRITE = 1073741824u;

	internal const uint OPEN_EXISTING = 3u;

	private DCB _dcb;

	private COMMTIMEOUTS _struTimeout;

	private IntPtr _hdlCom;

	private string _szPort;

	private int _iPort;

	private byte _cCR;

	public bool IsOpen => _hdlCom != (IntPtr)(-1);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

	[DllImport("kernel32.dll")]
	private static extern bool CloseHandle(IntPtr hObject);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint nNumberOfBytesRead, IntPtr lpOverlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool WriteFile(IntPtr fFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

	[DllImport("kernel32.dll")]
	private static extern bool PurgeComm(IntPtr hFile, uint flags);

	[DllImport("kernel32.dll")]
	private static extern bool GetCommMask(IntPtr hFile, out uint dwEvtMask);

	[DllImport("kernel32.dll")]
	private static extern bool SetCommMask(IntPtr hFile, uint dwEvtMask);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool WaitCommEvent(IntPtr hFile, out uint dwEvtMask, IntPtr lpOverlapped);

	[DllImport("kernel32.dll")]
	private static extern bool GetCommState(IntPtr hFile, ref DCB lpDCB);

	[DllImport("kernel32.dll")]
	private static extern bool SetCommState(IntPtr hFile, [In] ref DCB lpDCB);

	[DllImport("kernel32.dll")]
	private static extern bool GetCommTimeouts(IntPtr hFile, ref COMMTIMEOUTS lpCommTimeouts);

	[DllImport("kernel32.dll")]
	internal static extern bool SetCommTimeouts(IntPtr hFile, [In] ref COMMTIMEOUTS lpCommTimeouts);

	public ComPort(int i_i32Port)
	{
		_dcb = default(DCB);
		_struTimeout = default(COMMTIMEOUTS);
		_hdlCom = (IntPtr)(-1);
		if (i_i32Port < 1 || i_i32Port > 256)
		{
			i_i32Port = 1;
		}
		_iPort = i_i32Port;
		_szPort = "\\\\.\\COM" + _iPort;
		_cCR = 13;
	}

	public ComPort(string i_szPort)
	{
		_dcb = default(DCB);
		_struTimeout = default(COMMTIMEOUTS);
		_hdlCom = (IntPtr)(-1);
		_iPort = 0;
		_szPort = i_szPort;
		_cCR = 13;
	}

	public bool OpenComPort()
	{
		_hdlCom = CreateFile(_szPort, 3221225472u, 0u, IntPtr.Zero, 3u, 0u, IntPtr.Zero);
		if (_hdlCom == (IntPtr)(-1))
		{
			return false;
		}
		if (!GetCommState(_hdlCom, ref _dcb))
		{
			return false;
		}
		if (!GetCommTimeouts(_hdlCom, ref _struTimeout))
		{
			return false;
		}
		return true;
	}

	public bool CloseComPort()
	{
		if (_hdlCom != (IntPtr)(-1) && !CloseHandle(_hdlCom))
		{
			return false;
		}
		return true;
	}

	public bool SetComPortState(Baudrate i_i32Baudrate, Databits i_btDataBits, Parity i_btParity, Stopbits i_btStop)
	{
		_dcb.baudRate = (int)i_i32Baudrate;
		_dcb.byteSize = (byte)i_btDataBits;
		_dcb.prtyByte = (byte)i_btParity;
		_dcb.stopBits = (byte)i_btStop;
		return SetCommState(_hdlCom, ref _dcb);
	}

	public void GetComPortState(out Baudrate o_i32Baudrate, out Databits o_btDataBits, out Parity o_btParity, out Stopbits o_btStop)
	{
		o_i32Baudrate = (Baudrate)_dcb.baudRate;
		o_btDataBits = (Databits)_dcb.byteSize;
		o_btParity = (Parity)_dcb.prtyByte;
		o_btStop = (Stopbits)_dcb.stopBits;
	}

	public bool SetComPortTimeout(int i_i32ReadInterval, int i_i32ReadTotalConstant, int i_i32ReadTotalMultiplier, int i_i32WriteTotalConstant, int i_i32WriteTotalMultiplier)
	{
		_struTimeout.readIntervalTimeout = (uint)i_i32ReadInterval;
		_struTimeout.readTotalTimeoutConstant = (uint)i_i32ReadTotalConstant;
		_struTimeout.readTotalTimeoutMultiplier = (uint)i_i32ReadTotalMultiplier;
		_struTimeout.writeTotalTimeoutConstant = (uint)i_i32WriteTotalConstant;
		_struTimeout.writeTotalTimeoutMultiplier = (uint)i_i32WriteTotalMultiplier;
		return SetCommTimeouts(_hdlCom, ref _struTimeout);
	}

	public void GetComPortTimeout(out int o_i32ReadInterval, out int o_i32ReadTotalConstant, out int o_i32ReadTotalMultiplier, out int o_i32WriteTotalConstant, out int o_i32WriteTotalMultiplier)
	{
		o_i32ReadInterval = (int)_struTimeout.readIntervalTimeout;
		o_i32ReadTotalConstant = (int)_struTimeout.readTotalTimeoutConstant;
		o_i32ReadTotalMultiplier = (int)_struTimeout.readTotalTimeoutMultiplier;
		o_i32WriteTotalConstant = (int)_struTimeout.writeTotalTimeoutConstant;
		o_i32WriteTotalMultiplier = (int)_struTimeout.writeTotalTimeoutMultiplier;
	}

	public bool SetComPortEventMask(int i_i32Event)
	{
		return SetCommMask(_hdlCom, (uint)i_i32Event);
	}

	public bool GetComPortEventMask(out int o_i32Event)
	{
		uint dwEvtMask;
		bool commMask = GetCommMask(_hdlCom, out dwEvtMask);
		o_i32Event = (int)dwEvtMask;
		return commMask;
	}

	public bool WaitComPortEvent(out int o_i32Event)
	{
		uint dwEvtMask;
		bool result = WaitCommEvent(_hdlCom, out dwEvtMask, IntPtr.Zero);
		o_i32Event = (int)dwEvtMask;
		return result;
	}

	public bool SetPurge(int i_iFlags)
	{
		return PurgeComm(_hdlCom, (uint)i_iFlags);
	}

	public int GetPortNum()
	{
		return _iPort;
	}

	public int Send(int i_i32Len, byte[] i_btData)
	{
		uint lpNumberOfBytesWritten = 0u;
		if (!WriteFile(_hdlCom, i_btData, (uint)i_i32Len, out lpNumberOfBytesWritten, IntPtr.Zero) && CloseComPort())
		{
			OpenComPort();
		}
		return (int)lpNumberOfBytesWritten;
	}

	public int Send(string i_szData)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(i_szData);
		return Send(i_szData.Length, bytes);
	}

	public int Recv(int i_i32Len, ref byte[] o_btData)
	{
		uint nNumberOfBytesRead = 0u;
		if (!ReadFile(_hdlCom, o_btData, (uint)i_i32Len, out nNumberOfBytesRead, IntPtr.Zero) && CloseComPort())
		{
			OpenComPort();
		}
		return (int)nNumberOfBytesRead;
	}

	public int Recv(out string o_szData)
	{
		bool flag = false;
		byte[] array = new byte[2];
		byte[] array2 = new byte[1025];
		int num = 0;
		o_szData = "";
		do
		{
			uint nNumberOfBytesRead = 0u;
			if (!ReadFile(_hdlCom, array, 1u, out nNumberOfBytesRead, IntPtr.Zero) && CloseComPort())
			{
				OpenComPort();
			}
			if (nNumberOfBytesRead == 1)
			{
				if (array[0] <= 0)
				{
					continue;
				}
				if (num < 1024)
				{
					array2[num] = array[0];
					if (array[0] == _cCR)
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				num++;
			}
			else
			{
				flag = true;
			}
		}
		while (!flag);
		o_szData = Encoding.ASCII.GetString(array2, 0, num);
		return num;
	}

	public int GetComPortLastError()
	{
		return Marshal.GetLastWin32Error();
	}
}
