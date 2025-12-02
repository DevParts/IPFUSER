using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser;

[StandardModule]
internal sealed class LaserDLL
{
	public struct PStatus
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

	public struct PStatusExt
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

	public struct SimuString
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		[VBFixedString(256)]
		public char[] str_Renamed;
	}

	public struct LargeSimuString
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
		[VBFixedString(1024)]
		public char[] largestr_Renamed;
	}

	public static int COPY_TO_RAMDISK = 0;

	public static int COPY_TO_HARDDISK = 1;

	public static int COPY_FROM_HARDDISK = 2;

	public static int COPY_FROM_RAMDISK = 4;

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MInit@16", ExactSpelling = true, SetLastError = true)]
	public static extern void MInit(ref int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string name_Renamed, [MarshalAs(UnmanagedType.VBByRefStr)] ref string IP, [MarshalAs(UnmanagedType.VBByRefStr)] ref string path);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MFinish@4", ExactSpelling = true, SetLastError = true)]
	public static extern void MFinish(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MStartClient@4", ExactSpelling = true, SetLastError = true)]
	public static extern int MStartClient(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Stop@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Stop(int socket, int timeout);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Knockout@4", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Knockout(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MShutdownClient@4", ExactSpelling = true, SetLastError = true)]
	public static extern int MShutdownClient(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_FastUsermessage@12", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_FastUsermessage(int socket, byte field, [MarshalAs(UnmanagedType.VBByRefStr)] ref string text_Renamed);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_GetFastUsermessage@16", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_GetFastUsermessage(int socket, byte field, ref LargeSimuString buf, ref int len);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_FastUTF8Usermessage@12", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_FastUTF8Usermessage(int socket, byte field, byte[] text_Renamed);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_GetFastUTF8Usermessage@16", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_GetFastUTF8Usermessage(int socket, byte field, byte[] buf, ref int len);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_FastDataString@16", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_FastDataString(int socket, byte field, byte[] text_Renamed, int len);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_GetFastDataString@16", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_GetFastDataString(int socket, byte field, byte[] buf, ref int len);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_EnableBufferedUM@16", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_EnableBufferedUM(int socket, int ge, ref int actsize, int defsize = 10);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_EnableBufferedUMExt@24", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_EnableBufferedUMExt(int socket, int ge, ref int actsize, ref int field, ref int fillstatus, int defsize);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_EnableBufferedDataString@24", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_EnableBufferedDataString(int socket, int ge, ref int actsize, ref int field, ref int fillstatus, int defsize);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Status@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Status(int socket, ref PStatus status);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_StatusExt@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_StatusExt(int socket, ref PStatusExt status);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MGetLastError@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MGetLastError(int socket, ref SimuString txt);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MIsConnected@4", ExactSpelling = true, SetLastError = true)]
	public static extern int MIsConnected(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MSetTimeout@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MSetTimeout(int socket, int timeout);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MGetVersion@4", ExactSpelling = true, SetLastError = true)]
	public static extern short MGetVersion(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Start@12", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Start(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string filename, int nr);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_StartExtended@16", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_StartExtended(int socket, int nr, int msg, int batch);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Reload@4", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Reload(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Settime@4", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Settime(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Delete@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Delete(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string name_Renamed);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_SetDefault@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_SetDefault(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string name_Renamed);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_CopyFile@16", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_CopyFile(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string filename, [MarshalAs(UnmanagedType.VBByRefStr)] ref string path, byte options);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Mode@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Mode(int socket, ref byte mode);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_TriggerPrint@4", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_TriggerPrint(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_CounterReset@4", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_CounterReset(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Offset@24", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Offset(int socket, ref int dx, ref int dy, int relative, int format_Renamed = 0, int reset_Renamed = 0);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_SetGlobalCounter@12", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_SetGlobalCounter(int socket, byte field, [MarshalAs(UnmanagedType.VBByRefStr)] ref string counter);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_GetGlobalCounter@12", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_GetGlobalCounter(int socket, byte field, ref SimuString counter);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Powerscale@16", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Powerscale(int socket, int setorget, int member, ref int value);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_SetPublicCounter@16", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_SetPublicCounter(int socket, byte field, int repeats, int prints);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_SetDynamic@12", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_SetDynamic(int socket, int var, ref int value);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_SetDynamic@12", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_GetDynamic(int socket, int var, ref int value);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_AsciiConfig@12", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_AsciiConfig(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string name_Renamed, int value);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_StartPrintSession@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_StartPrintSession(int socket, int ignorealarms);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_EndPrintSession@4", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_EndPrintSession(int socket);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_TestPointer@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_TestPointer(int socket, int turnon);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Eventhandler@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Eventhandler(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string filename);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_GetFilenames@20", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_GetFilenames(int socket, [MarshalAs(UnmanagedType.VBByRefStr)] ref string extension, int frame, byte[] buf, ref int len);

	[DllImport("SocketCommDll.dll", CharSet = CharSet.Ansi, EntryPoint = "_MLaser_Store@8", ExactSpelling = true, SetLastError = true)]
	public static extern int MLaser_Store(int socket, ref int value);

	public static string SSView(char[] Cadena)
	{
		string Texto = "";
		checked
		{
			int num = Cadena.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				Texto += Conversions.ToString(Cadena[i]);
			}
			return Texto;
		}
	}
}
