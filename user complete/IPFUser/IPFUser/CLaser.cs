using System.Windows.Forms;
using IPFUser.My;
using Microsoft.VisualBasic;

namespace IPFUser;

internal class CLaser
{
	public enum LASER_STATES
	{
		STOPPED,
		MARKING,
		ERRORS
	}

	public enum ERROR_TYPES
	{
		LASER_ERROR,
		LASER_WARNING
	}

	public int TotalMark;

	public int TotalMarked;

	private LASER_STATES _State;

	internal int Sock;

	internal int Sock2;

	private LaserDLL.SimuString Txt;

	private ushort ErrorHigh;

	private ushort ErrorLow;

	public string ErrorDesc;

	private int FillStatus;

	private int Field;

	private int Actsize;

	private int Defsize;

	public char[] NameRenamed;

	public ERROR_TYPES ErrorType;

	public LASER_STATES State => _State;

	public bool GetState()
	{
		LaserDLL.PStatus Status = default(LaserDLL.PStatus);
		Status = default(LaserDLL.PStatus);
		int i = LaserDLL.MLaser_Status(Sock, ref Status);
		NameRenamed = Status.name_Renamed;
		if (i != 0)
		{
			LaserDLL.MGetLastError(Sock, ref Txt);
			ErrorDesc = "Laser Error: " + LaserDLL.SSView(Txt.str_Renamed);
			ErrorHigh = ushort.MaxValue;
			ErrorLow = ushort.MaxValue;
			return false;
		}
		if (Status.err_high != 0)
		{
			switch (Status.err_low)
			{
			case 2:
				ErrorDesc = AppCSIUser.Rm.GetString("String1");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 3:
				ErrorDesc = AppCSIUser.Rm.GetString("String2");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 4:
				ErrorDesc = AppCSIUser.Rm.GetString("String3");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 5:
				ErrorDesc = AppCSIUser.Rm.GetString("String4");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 6:
				ErrorDesc = AppCSIUser.Rm.GetString("String5");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 7:
				ErrorDesc = AppCSIUser.Rm.GetString("String6");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 8:
				ErrorDesc = AppCSIUser.Rm.GetString("String7");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 9:
				ErrorDesc = AppCSIUser.Rm.GetString("String8");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 10:
				ErrorDesc = AppCSIUser.Rm.GetString("String9");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 11:
				ErrorDesc = AppCSIUser.Rm.GetString("String10");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 12:
				ErrorDesc = AppCSIUser.Rm.GetString("String11");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 13:
				ErrorDesc = AppCSIUser.Rm.GetString("String12");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 16:
				ErrorDesc = AppCSIUser.Rm.GetString("String13");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 17:
				ErrorDesc = AppCSIUser.Rm.GetString("String14");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 18:
				ErrorDesc = AppCSIUser.Rm.GetString("String15");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 20:
				ErrorDesc = AppCSIUser.Rm.GetString("String16");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 21:
				ErrorDesc = AppCSIUser.Rm.GetString("String17");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 22:
				ErrorDesc = AppCSIUser.Rm.GetString("String18");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 37:
				ErrorDesc = AppCSIUser.Rm.GetString("String2");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 38:
				ErrorDesc = AppCSIUser.Rm.GetString("String19");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 48:
				ErrorDesc = AppCSIUser.Rm.GetString("String20");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 50:
				ErrorDesc = AppCSIUser.Rm.GetString("String21");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 51:
				ErrorDesc = "Barcode creation failure";
				ErrorDesc = AppCSIUser.Rm.GetString("String22");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 52:
				ErrorDesc = AppCSIUser.Rm.GetString("String23");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 53:
				ErrorDesc = AppCSIUser.Rm.GetString("String24");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 54:
				ErrorDesc = AppCSIUser.Rm.GetString("String25");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 55:
				ErrorDesc = AppCSIUser.Rm.GetString("String26");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 56:
				ErrorDesc = AppCSIUser.Rm.GetString("String27");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 57:
				ErrorDesc = AppCSIUser.Rm.GetString("String28");
				ErrorType = ERROR_TYPES.LASER_WARNING;
				break;
			case 65:
				ErrorDesc = AppCSIUser.Rm.GetString("String29");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 66:
				ErrorDesc = AppCSIUser.Rm.GetString("String30");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 67:
				ErrorDesc = AppCSIUser.Rm.GetString("String31");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			case 68:
				ErrorDesc = AppCSIUser.Rm.GetString("String32");
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			default:
				ErrorDesc = AppCSIUser.Rm.GetString("String33") + " " + Conversion.Hex(Status.err_low);
				ErrorType = ERROR_TYPES.LASER_ERROR;
				break;
			}
			return false;
		}
		ErrorDesc = "";
		return true;
	}

	/// <summary>
	/// Stablish main socket to control laser (select message, start print ...)
	/// </summary>
	/// <param name="MessageName"></param>
	/// <param name="MessagePath"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public bool Run(string MessageName, string MessagePath, int UserFields, bool IsDataString = false, bool ResetQueues = true)
	{
		ref int sock = ref Sock;
		string name_Renamed = "anyword";
		MySettings settings;
		string IP = (settings = MySettingsProperty.Settings).LaserIP;
		string path = ".\\\\";
		LaserDLL.MInit(ref sock, ref name_Renamed, ref IP, ref path);
		settings.LaserIP = IP;
		if (LaserDLL.MStartClient(Sock) != 0)
		{
			LaserDLL.MGetLastError(Sock, ref Txt);
			MessageBox.Show("Laser Error: " + LaserDLL.SSView(Txt.str_Renamed), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			_State = LASER_STATES.ERRORS;
			return false;
		}
		int i = LaserDLL.MLaser_CopyFile(Sock, ref MessageName, ref MessagePath, 0);
		if (LaserDLL.MLaser_SetDefault(Sock, ref MessageName) != 0)
		{
			LaserDLL.MGetLastError(Sock, ref Txt);
			MessageBox.Show("Laser Error: " + LaserDLL.SSView(Txt.str_Renamed), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return false;
		}
		checked
		{
			if (ResetQueues)
			{
				Field = 0;
				FillStatus = 0;
				Actsize = 0;
				Defsize = MySettingsProperty.Settings.LaserBufferSize;
				int num = UserFields - 1;
				for (i = 0; i <= num; i++)
				{
					Field = i;
					if (!IsDataString)
					{
						LaserDLL.MLaser_EnableBufferedUMExt(Sock, 0, ref Actsize, ref Field, ref FillStatus, Defsize);
					}
					else
					{
						LaserDLL.MLaser_EnableBufferedDataString(Sock, 0, ref Actsize, ref Field, ref FillStatus, Defsize);
					}
				}
			}
			_State = LASER_STATES.MARKING;
			return true;
		}
	}

	/// <summary>
	/// Returns item number in laser buffer
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public int InBufferCount(bool IsDataString = false)
	{
		if (Sock > 0)
		{
			if (!IsDataString)
			{
				int i = LaserDLL.MLaser_EnableBufferedUMExt(Sock, 1, ref Actsize, ref Field, ref FillStatus, Defsize);
			}
			else
			{
				int i = LaserDLL.MLaser_EnableBufferedDataString(Sock, 1, ref Actsize, ref Field, ref FillStatus, Defsize);
			}
			return FillStatus;
		}
		return 0;
	}

	/// <summary>
	/// Reset Laser Buffer 
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public int ResetBuffer(int UserFields, bool IsDataString = false)
	{
		checked
		{
			if (Sock > 0)
			{
				int num = UserFields - 1;
				for (int i = 0; i <= num; i++)
				{
					Field = i;
					if (!IsDataString)
					{
						LaserDLL.MLaser_EnableBufferedUMExt(Sock, 2, ref Actsize, ref Field, ref FillStatus, Defsize);
					}
					else
					{
						LaserDLL.MLaser_EnableBufferedDataString(Sock, 2, ref Actsize, ref Field, ref FillStatus, Defsize);
					}
				}
				return Actsize;
			}
			return 0;
		}
	}

	/// <summary>
	/// Stablish secondary socket to send codes
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public bool RunThread()
	{
		ref int sock = ref Sock2;
		string name_Renamed = "anyword";
		MySettings settings;
		string IP = (settings = MySettingsProperty.Settings).LaserIP;
		string path = ".\\\\";
		LaserDLL.MInit(ref sock, ref name_Renamed, ref IP, ref path);
		settings.LaserIP = IP;
		if (LaserDLL.MStartClient(Sock2) != 0)
		{
			LaserDLL.MGetLastError(Sock2, ref Txt);
			MessageBox.Show(AppCSIUser.Rm.GetString("String33") + " " + new string(Txt.str_Renamed), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			_State = LASER_STATES.ERRORS;
			return false;
		}
		return true;
	}

	public bool Stop()
	{
		LaserDLL.MLaser_Stop(Sock, 2000);
		if (LaserDLL.MLaser_Knockout(Sock) != 0)
		{
			LaserDLL.MGetLastError(Sock, ref Txt);
			MessageBox.Show(AppCSIUser.Rm.GetString("String33") + " " + new string(Txt.str_Renamed), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		if (LaserDLL.MShutdownClient(Sock) != 0)
		{
			LaserDLL.MGetLastError(Sock, ref Txt);
			MessageBox.Show(AppCSIUser.Rm.GetString("String33") + " " + new string(Txt.str_Renamed), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		LaserDLL.MFinish(Sock);
		Sock = 0;
		if (LaserDLL.MLaser_Knockout(Sock2) != 0)
		{
			LaserDLL.MGetLastError(Sock2, ref Txt);
			MessageBox.Show(AppCSIUser.Rm.GetString("String33") + " " + new string(Txt.str_Renamed), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		if (LaserDLL.MShutdownClient(Sock2) != 0)
		{
			LaserDLL.MGetLastError(Sock2, ref Txt);
			MessageBox.Show(AppCSIUser.Rm.GetString("String33") + " " + new string(Txt.str_Renamed), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		LaserDLL.MFinish(Sock2);
		Sock2 = 0;
		_State = LASER_STATES.STOPPED;
		return true;
	}
}
