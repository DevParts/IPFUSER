using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Lasers;

public class LaserDLL : MacsaDevice
{
	private struct PStatus
	{
		public int d_counter;

		public int s_counter;

		public int n_messageport;

		public byte Start;

		public byte request;

		public byte option_Renamed;

		public byte res;

		public int t_counter;

		public int m_copies;

		public short err_high;

		public short err_low;

		public int time;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		[VBFixedString(8)]
		public char[] name_Renamed;

		public int reserved1;

		public int reserved2;
	}

	private struct PStatusExt
	{
		public int d_counter;

		public int s_counter;

		public int n_messageport;

		public byte Start;

		public byte request;

		public byte option_Renamed;

		public byte res;

		public int t_counter;

		public int m_copies;

		public short err_high;

		public short err_low;

		public int time;

		public int alarmmask1;

		public int alarmmask2;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[VBFixedString(16)]
		public char[] messagename_Renamed;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[VBFixedString(16)]
		public char[] eventfile_Renamed;
	}

	private struct SimuString
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		[VBFixedString(256)]
		public char[] str_Renamed;
	}

	private struct LargeSimuString
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
		[VBFixedString(1024)]
		public char[] largestr_Renamed;
	}

	public enum LASER_STATES
	{
		STOPPED,
		MARKING,
		ERRORS
	}

	public enum LASER_MODES
	{
		LM_STATIC,
		LM_DYNAMIC,
		LM_DYNAMIC_DISTANCE,
		LM_DYNAMIC_STATIC
	}

	public enum UPDATE_CONFIG_TYPE
	{
		UC_TOTAL,
		UC_PARTIAL
	}

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	public delegate void OnlineEventHandler(object sender);

	public delegate void OnReadyToReceiveDataEventHandler(object sender);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private int COPY_TO_RAMDISK;

	private int COPY_TO_HARDDISK;

	private int COPY_FROM_HARDDISK;

	private int COPY_FROM_RAMDISK;

	private LASER_STATES _State;

	private int Sock;

	private int Sock2;

	private SimuString Txt;

	public ushort ErrorHigh;

	public ushort ErrorLow;

	public string ErrorDesc;

	public char[] NameRenamed;

	private string LaserIp;

	private int LaserBufferSize;

	private int _CounterGoodPrint;

	private int _CounterBadPrint;

	private string _lastMessage;

	public LASER_STATES State => _State;

	public int UserFieldsBufferSize
	{
		get
		{
			return LaserBufferSize;
		}
		set
		{
			LaserBufferSize = value;
		}
	}

	public bool IsDataSendRequested
	{
		[DebuggerNonUserCode]
		get;
		[DebuggerNonUserCode]
		set;
	}

	public int CounterGoodPrint => _CounterGoodPrint;

	public int CounterBadPrint => _CounterBadPrint;

	public int FieldCounter
	{
		get
		{
			if (Sock == 0)
			{
				return 0;
			}
			int num = 0;
			SimuString counter = default(SimuString);
			MLaser_GetGlobalCounter(Sock, IdField, ref counter);
			checked
			{
				int num2 = counter.str_Renamed.Length - 1;
				int num3 = 0;
				while (true)
				{
					int num4 = num3;
					int num5 = num2;
					if (num4 <= num5 && counter.str_Renamed[num3] != 0)
					{
						num *= 10;
						num += Strings.Asc(counter.str_Renamed[num3]) - 48;
						num3++;
						continue;
					}
					break;
				}
				return num;
			}
		}
		set
		{
			int sock = Sock;
			string counter = value.ToString();
			MLaser_SetGlobalCounter(sock, IdField, ref counter);
		}
	}

	public LASER_MODES SetMode
	{
		set
		{
			if (Sock != 0)
			{
				int sock = Sock;
				byte mode = checked((byte)value);
				MLaser_Mode(sock, ref mode);
			}
		}
	}

	public bool TestPointer
	{
		set
		{
			if (Sock != 0)
			{
				if (value)
				{
					MLaser_TestPointer(Sock, 1);
				}
				else
				{
					MLaser_TestPointer(Sock, 0);
				}
			}
		}
	}

	public bool GetSecondarySock
	{
		get
		{
			if (Operators.CompareString(LaserIp, "", TextCompare: false) == 0)
			{
				return false;
			}
			ref int sock = ref Sock2;
			string name_Renamed = "anyword";
			ref string laserIp = ref LaserIp;
			string path = ".\\\\";
			MInit(ref sock, ref name_Renamed, ref laserIp, ref path);
			if (MStartClient(Sock2) != 0)
			{
				return false;
			}
			return true;
		}
	}

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnlineEventHandler Online;

	[method: DebuggerNonUserCode]
	public event OnReadyToReceiveDataEventHandler OnReadyToReceiveData;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	[DebuggerNonUserCode]
	private static void __ENCAddToList(object value)
	{
		checked
		{
			lock (__ENCList)
			{
				if (__ENCList.Count == __ENCList.Capacity)
				{
					int num = 0;
					int num2 = __ENCList.Count - 1;
					int num3 = 0;
					while (true)
					{
						int num4 = num3;
						int num5 = num2;
						if (num4 > num5)
						{
							break;
						}
						WeakReference weakReference = __ENCList[num3];
						if (weakReference.IsAlive)
						{
							if (num3 != num)
							{
								__ENCList[num] = __ENCList[num3];
							}
							num++;
						}
						num3++;
					}
					__ENCList.RemoveRange(num, __ENCList.Count - num);
					__ENCList.Capacity = __ENCList.Count;
				}
				__ENCList.Add(new WeakReference(RuntimeHelpers.GetObjectValue(value)));
			}
		}
	}

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MInit@16", ExactSpelling = true, SetLastError = true)]
	private static extern void MInit(ref int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string name_Renamed, [MarshalAs(UnmanagedType.VBByRefStr)] ref string IP, [MarshalAs(UnmanagedType.VBByRefStr)] ref string path);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MFinish@4", ExactSpelling = true, SetLastError = true)]
	private static extern void MFinish(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MStartClient@4", ExactSpelling = true, SetLastError = true)]
	private static extern int MStartClient(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Stop@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Stop(int socket, int timeout);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Knockout@4", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Knockout(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MShutdownClient@4", ExactSpelling = true, SetLastError = true)]
	private static extern int MShutdownClient(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_FastUsermessage@12", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_FastUsermessage(int socket, byte field, [MarshalAs(UnmanagedType.VBByRefStr)] ref string text_Renamed);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_GetFastUsermessage@16", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_GetFastUsermessage(int socket, byte field, ref LargeSimuString buf, ref int len);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_FastUTF8Usermessage@12", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_FastUTF8Usermessage(int socket, byte field, byte[] text_Renamed);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_GetFastUTF8Usermessage@16", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_GetFastUTF8Usermessage(int socket, byte field, byte[] buf, ref int len);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_FastDataString@16", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_FastDataString(int socket, byte field, byte[] text_Renamed, int len);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_GetFastDataString@16", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_GetFastDataString(int socket, byte field, byte[] buf, ref int len);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_EnableBufferedUM@16", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_EnableBufferedUM(int socket, int ge, ref int actsize, int defsize = 10);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_EnableBufferedUMExt@24", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_EnableBufferedUMExt(int socket, int ge, ref int actsize, ref int field, ref int fillstatus, int defsize);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_EnableBufferedDataString@24", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_EnableBufferedDataString(int socket, int ge, ref int actsize, ref int field, ref int fillstatus, int defsize);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Status@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Status(int socket, ref PStatus status);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_StatusExt@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_StatusExt(int socket, ref PStatusExt status);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MGetLastError@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MGetLastError(int socket, ref SimuString txt);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MIsConnected@4", ExactSpelling = true, SetLastError = true)]
	private static extern int MIsConnected(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MSetTimeout@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MSetTimeout(int socket, int timeout);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MGetVersion@4", ExactSpelling = true, SetLastError = true)]
	private static extern short MGetVersion(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Start@12", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Start(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string filename, int nr);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_StartExtended@16", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_StartExtended(int socket, int nr, int msg, int batch);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Reload@4", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Reload(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Settime@4", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Settime(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Delete@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Delete(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string name_Renamed);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_SetDefault@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_SetDefault(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string name_Renamed);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_CopyFile@16", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_CopyFile(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string filename, [MarshalAs(UnmanagedType.VBByRefStr)] ref string path, byte options);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Mode@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Mode(int socket, ref byte mode);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_TriggerPrint@4", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_TriggerPrint(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_CounterReset@4", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_CounterReset(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Offset@24", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Offset(int socket, ref int dx, ref int dy, int relative, int format_Renamed = 0, int reset_Renamed = 0);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_SetGlobalCounter@12", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_SetGlobalCounter(int socket, byte field, [MarshalAs(UnmanagedType.VBByRefStr)] ref string counter);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_GetGlobalCounter@12", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_GetGlobalCounter(int socket, byte field, ref SimuString counter);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Powerscale@16", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Powerscale(int socket, int setorget, int member, ref int value);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_SetPrivateCounter@16", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_SetPrivateCounter(int socket, byte field, int repeats, int prints);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_SetDynamic@12", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_SetDynamic(int socket, int var, ref int value);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_SetDynamic@12", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_GetDynamic(int socket, int var, ref int value);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_AsciiConfig@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_AsciiConfig(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string name_Renamed);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_StartPrintSession@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_StartPrintSession(int socket, int ignorealarms);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_EndPrintSession@4", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_EndPrintSession(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_TestPointer@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_TestPointer(int socket, int turnon);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Eventhandler@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Eventhandler(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string filename);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_GetFilenames@20", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_GetFilenames(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string extension, int frame, byte[] buf, ref int len);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Store@8", ExactSpelling = true, SetLastError = true)]
	private static extern int MLaser_Store(int socket, ref int value);

	public LaserDLL()
	{
		__ENCAddToList(this);
		COPY_TO_RAMDISK = 0;
		COPY_TO_HARDDISK = 1;
		COPY_FROM_HARDDISK = 2;
		COPY_FROM_RAMDISK = 4;
		LaserBufferSize = 20;
		_lastMessage = "";
		_State = LASER_STATES.STOPPED;
	}

	public bool Init(string Ip)
	{
		LaserIp = Ip;
		ref int sock = ref Sock;
		string name_Renamed = "anyword";
		ref string laserIp = ref LaserIp;
		string path = ".\\\\";
		MInit(ref sock, ref name_Renamed, ref laserIp, ref path);
		if (MStartClient(Sock) != 0)
		{
			return false;
		}
		return true;
	}

	public bool RequestToSendData()
	{
		IsDataSendRequested = true;
		return GetStatus();
	}

	public bool GetStatus()
	{
		PStatus pStatus = default(PStatus);
		pStatus = default(PStatus);
		int num = MLaser_Status(Sock, ref pStatus);
		_CounterGoodPrint = pStatus.d_counter;
		_CounterBadPrint = pStatus.s_counter;
		NameRenamed = pStatus.name_Renamed;
		if (num != 0)
		{
			MGetLastError(Sock, ref Txt);
			ErrorDesc = "Laser Error: " + SSView(Txt.str_Renamed);
			ErrorHigh = ushort.MaxValue;
			ErrorLow = ushort.MaxValue;
			OnError?.Invoke(this, "0", Common.Rm.GetString("LaserDLL0"), Common.ERROR_TYPE.Error);
			return false;
		}
		checked
		{
			ErrorHigh = (ushort)pStatus.err_high;
			ErrorLow = (ushort)pStatus.err_low;
			if ((ushort)pStatus.err_high > 0)
			{
				Common.DATA_ERROR dataError = GetDataError(pStatus.err_low);
				OnError?.Invoke(this, dataError.Id, dataError.Desc, dataError.Type);
				return false;
			}
			Online?.Invoke(this);
			ErrorDesc = "";
			if (IsDataSendRequested)
			{
				OnReadyToReceiveData?.Invoke(this);
				IsDataSendRequested = false;
			}
			return true;
		}
	}

	public bool StartPrint(int PrintCount = 0)
	{
		if ((Operators.CompareString(_lastMessage, "", TextCompare: false) != 0) & (Sock > 0))
		{
			MLaser_Start(Sock, ref _lastMessage, PrintCount);
			return true;
		}
		return false;
	}

	public bool StopPrint()
	{
		if (Sock > 0)
		{
			MLaser_Stop(Sock, 2000);
			return true;
		}
		return false;
	}

	public bool SendFile(string FileName, int PrintCount = 0)
	{
		if (Sock == 0)
		{
			return false;
		}
		string filename = Path.GetFileName(FileName);
		string path = Path.GetDirectoryName(FileName) + "\\";
		if (MLaser_CopyFile(Sock, ref filename, ref path, 1) != 0)
		{
			MGetLastError(Sock, ref Txt);
			OnError?.Invoke(this, "", Common.Rm.GetString("LaserDLL36") + SSView(Txt.str_Renamed), Common.ERROR_TYPE.Error);
			_State = LASER_STATES.ERRORS;
			return false;
		}
		int num = MLaser_SetDefault(Sock, ref filename);
		_lastMessage = Path.GetFileNameWithoutExtension(filename);
		int sock = Sock;
		string filename2 = Path.GetFileNameWithoutExtension(filename);
		num = MLaser_Start(sock, ref filename2, PrintCount);
		_State = LASER_STATES.MARKING;
		return true;
	}

	public bool SelectFile(string FileName, int PrintCount = 0)
	{
		int num = MLaser_SetDefault(Sock, ref FileName);
		_lastMessage = Path.GetFileNameWithoutExtension(FileName);
		int sock = Sock;
		string filename = Path.GetFileNameWithoutExtension(FileName);
		num = MLaser_Start(sock, ref filename, PrintCount);
		_State = LASER_STATES.MARKING;
		if (num == 0)
		{
			return true;
		}
		return false;
	}

	public int SendRemoteField(byte UserField, string Value, bool UseSecondarySock = false)
	{
		if (UseSecondarySock)
		{
			return MLaser_FastUTF8Usermessage(Sock2, UserField, Encoding.UTF8.GetBytes(Value));
		}
		return MLaser_FastUTF8Usermessage(Sock, UserField, Encoding.UTF8.GetBytes(Value));
	}

	public int FUItemsInQueue(int UserField)
	{
		if (Sock > 0)
		{
			int actsize = default(int);
			int fillstatus = default(int);
			int defsize = default(int);
			int num = MLaser_EnableBufferedUMExt(Sock, 1, ref actsize, ref UserField, ref fillstatus, defsize);
			return fillstatus;
		}
		return 0;
	}

	public bool FURangeEnableQueues(int UserFields, int QueueSize)
	{
		int fillstatus = 0;
		int actsize = 0;
		checked
		{
			int num = UserFields - 1;
			int field = 0;
			int num4 = default(int);
			while (true)
			{
				int num2 = field;
				int num3 = num;
				if (num2 > num3)
				{
					break;
				}
				num4 = MLaser_EnableBufferedUMExt(Sock, 0, ref actsize, ref field, ref fillstatus, QueueSize);
				field++;
			}
			if (num4 == 0)
			{
				return true;
			}
			return false;
		}
	}

	public bool FUEnableQueue(int IdUserField, int QueueSize)
	{
		int fillstatus = 0;
		int actsize = 0;
		if (MLaser_EnableBufferedUMExt(Sock, 0, ref actsize, ref IdUserField, ref fillstatus, QueueSize) == 0)
		{
			return true;
		}
		return false;
	}

	public int FUResetQueue(int UserField)
	{
		if (Sock > 0)
		{
			int actsize = default(int);
			int fillstatus = default(int);
			int defsize = default(int);
			int num = MLaser_EnableBufferedUMExt(Sock, 2, ref actsize, ref UserField, ref fillstatus, defsize);
			return actsize;
		}
		return 0;
	}

	public bool CloseConnection(bool StopPrint = true)
	{
		if (_State == LASER_STATES.STOPPED)
		{
			return true;
		}
		_State = LASER_STATES.STOPPED;
		if (StopPrint)
		{
			MLaser_Stop(Sock, 2000);
		}
		if (MLaser_Knockout(Sock) != 0)
		{
			MGetLastError(Sock, ref Txt);
			OnError?.Invoke(this, "", Common.Rm.GetString("LaserDLL36") + SSView(Txt.str_Renamed), Common.ERROR_TYPE.Error);
		}
		if (MShutdownClient(Sock) != 0)
		{
			MGetLastError(Sock, ref Txt);
			OnError?.Invoke(this, "", Common.Rm.GetString("LaserDLL36") + SSView(Txt.str_Renamed), Common.ERROR_TYPE.Error);
		}
		MFinish(Sock);
		Sock = 0;
		if (Sock2 != 0)
		{
			if (MLaser_Knockout(Sock2) != 0)
			{
				MGetLastError(Sock2, ref Txt);
				OnError?.Invoke(this, "", Common.Rm.GetString("LaserDLL36") + SSView(Txt.str_Renamed), Common.ERROR_TYPE.Error);
			}
			if (MShutdownClient(Sock2) != 0)
			{
				MGetLastError(Sock2, ref Txt);
				OnError?.Invoke(this, "", Common.Rm.GetString("LaserDLL36") + SSView(Txt.str_Renamed), Common.ERROR_TYPE.Error);
			}
			MFinish(Sock2);
			Sock2 = 0;
		}
		_State = LASER_STATES.STOPPED;
		return true;
	}

	public bool SinchroDataTime()
	{
		if (Sock == 0)
		{
			return false;
		}
		MLaser_Settime(Sock);
		return true;
	}

	public bool ResetCounter()
	{
		if (Sock == 0)
		{
			return false;
		}
		MLaser_CounterReset(Sock);
		return true;
	}

	public int SendAsciiConfig(string FileName, UPDATE_CONFIG_TYPE UpdateMode)
	{
		if (Sock == 0)
		{
			return -1;
		}
		return MLaser_AsciiConfig(Sock, ref FileName);
	}

	protected Common.DATA_ERROR GetDataError(int ErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		result.Id = ErrorId.ToString();
		switch (ErrorId)
		{
		case 2:
			result.Desc = Common.Rm.GetString("LaserDLL1");
			result.Type = Common.ERROR_TYPE.NoError;
			break;
		case 3:
			result.Desc = Common.Rm.GetString("LaserDLL2");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 4:
			result.Desc = Common.Rm.GetString("LaserDLL3");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 5:
			result.Desc = Common.Rm.GetString("LaserDLL4");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 6:
			result.Desc = Common.Rm.GetString("LaserDLL5");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 7:
			result.Desc = Common.Rm.GetString("LaserDLL6");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 8:
			result.Desc = Common.Rm.GetString("LaserDLL7");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 9:
			result.Desc = Common.Rm.GetString("LaserDLL8");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 10:
			result.Desc = Common.Rm.GetString("LaserDLL9");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 11:
			result.Desc = Common.Rm.GetString("LaserDLL10");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 12:
			result.Desc = Common.Rm.GetString("LaserDLL11");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 13:
			result.Desc = Common.Rm.GetString("LaserDLL12");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 16:
			result.Desc = Common.Rm.GetString("LaserDLL13");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 17:
			result.Desc = Common.Rm.GetString("LaserDLL14");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 18:
			result.Desc = Common.Rm.GetString("LaserDLL15");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 20:
			result.Desc = Common.Rm.GetString("LaserDLL16");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 21:
			result.Desc = Common.Rm.GetString("LaserDLL17");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 22:
			result.Desc = Common.Rm.GetString("LaserDLL18");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 37:
			result.Desc = Common.Rm.GetString("LaserDLL19");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 38:
			result.Desc = Common.Rm.GetString("LaserDLL20");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 39:
			result.Desc = Common.Rm.GetString("LaserDLL37");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 48:
			result.Desc = Common.Rm.GetString("LaserDLL21");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 50:
			result.Desc = Common.Rm.GetString("LaserDLL22");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 51:
			result.Desc = Common.Rm.GetString("LaserDLL23");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 52:
			result.Desc = Common.Rm.GetString("LaserDLL24");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 53:
			result.Desc = Common.Rm.GetString("LaserDLL25");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 54:
			ErrorDesc = Common.Rm.GetString("LaserDLL26");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 55:
			result.Desc = Common.Rm.GetString("LaserDLL27");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 56:
			result.Desc = Common.Rm.GetString("LaserDLL28");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 57:
			result.Desc = Common.Rm.GetString("LaserDLL29");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 65:
			result.Desc = Common.Rm.GetString("LaserDLL30");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 66:
			result.Desc = Common.Rm.GetString("LaserDLL31");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 67:
			result.Desc = Common.Rm.GetString("LaserDLL32");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 68:
			result.Desc = Common.Rm.GetString("LaserDLL33");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 84:
			result.Desc = Common.Rm.GetString("LaserDLL34");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		default:
			result.Desc = Common.Rm.GetString("LaserDLL35") + " " + Conversion.Hex(ErrorId);
			result.Type = Common.ERROR_TYPE.Error;
			break;
		}
		return result;
	}

	protected override void TcpComm_DataReceived(string sData)
	{
	}

	public string SSView(char[] Cadena)
	{
		string text = "";
		checked
		{
			int num = Cadena.Length - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				text += Conversions.ToString(Cadena[num2]);
				num2++;
			}
			return text;
		}
	}
}
