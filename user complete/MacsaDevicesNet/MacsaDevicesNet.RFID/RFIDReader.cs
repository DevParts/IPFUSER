#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using MacsaDevicesNet.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Speedway.Mach1;
using SpeedwayReaderParamControl;

namespace MacsaDevicesNet.RFID;

public class RFIDReader
{
	public enum STATE_READER
	{
		IDLE,
		READING,
		WRITING,
		WAIT_NO_TAG_PRESENT
	}

	public enum ANTENAS
	{
		ANTENNA1 = 1,
		ANTENNA2,
		ANTENNA3,
		ANTENNA4
	}

	public enum OUT_STATES
	{
		LOW,
		HIGH
	}

	public struct ANTENNA_SETTINGS
	{
		public bool Enabled;

		public int Power;

		public short Rssi;
	}

	public struct ANTENNAS_PARAMETERS
	{
		public ANTENNA_SETTINGS Antena1;

		public ANTENNA_SETTINGS Antena2;

		public ANTENNA_SETTINGS Antena3;

		public ANTENNA_SETTINGS Antena4;
	}

	public delegate void Del1(OPERATION_NTF.INVENTORY_NTF Inv);

	public delegate void Del2(MANAGEMENT_NTF.SYSTEM_ERROR_NTF Inv);

	public delegate void OnStoppedEventHandler(object Sender);

	public delegate void OnNewTagReadEventHandler(object Sender, string TagValue, short Rssi, int Antenna);

	public delegate void OnTagWrittenEventHandler(object Sender, string TagValue);

	public delegate void OnTagWrittenExtendedEventHandler(object Sender, string TagBeforeWrite, string TagValue);

	public delegate void OnErrorEventHandler(object Sender, string StrError);

	public delegate void OnNoTagPresentEventHandler(object Sender);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	[AccessedThroughProperty("oRFID")]
	private SpeedwayReader _oRFID;

	private SpeedwayReaderSettings SettingsRFID;

	private bool _IsOn;

	private bool _FilterNews;

	private ushort[] _DataToWrite;

	private Dictionary<string, int> _RecTags;

	private STATE_READER _State;

	private Timer _TimerNoTagPresent;

	private int _TimeWithoutTag;

	private bool _IsInitialized;

	private Dictionary<OPERATION_NTF.INVENTORY_NTF.E_ANTENNA, int> AntenaPulse;

	private int AntenaMask;

	private int _TagsOnFilterMemory;

	private Queue<string> _TagsQueue;

	private virtual SpeedwayReader oRFID
	{
		[DebuggerNonUserCode]
		get
		{
			return _oRFID;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			delegateSystemErrorNtf value2 = oRFID_OnError;
			delegateInventoryNtf value3 = oRFID_OnNewTagReceived;
			if (_oRFID != null)
			{
				_oRFID.onSystemErrorNtfReceived -= value2;
				_oRFID.onInventoryNtfReceived -= value3;
			}
			_oRFID = value;
			if (_oRFID != null)
			{
				_oRFID.onSystemErrorNtfReceived += value2;
				_oRFID.onInventoryNtfReceived += value3;
			}
		}
	}

	public bool IsInitialized => _IsInitialized;

	public int TagsOnFilterMemory
	{
		get
		{
			return _TagsOnFilterMemory;
		}
		set
		{
			_TagsOnFilterMemory = value;
		}
	}

	[method: DebuggerNonUserCode]
	public event OnStoppedEventHandler OnStopped;

	[method: DebuggerNonUserCode]
	public event OnNewTagReadEventHandler OnNewTagRead;

	[method: DebuggerNonUserCode]
	public event OnTagWrittenEventHandler OnTagWritten;

	[method: DebuggerNonUserCode]
	public event OnTagWrittenExtendedEventHandler OnTagWrittenExtended;

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnNoTagPresentEventHandler OnNoTagPresent;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	public RFIDReader()
	{
		__ENCAddToList(this);
		oRFID = new SpeedwayReader();
		SettingsRFID = new SpeedwayReaderSettings();
		_RecTags = new Dictionary<string, int>();
		AntenaPulse = new Dictionary<OPERATION_NTF.INVENTORY_NTF.E_ANTENNA, int>();
		_TagsOnFilterMemory = 0;
		_TagsQueue = new Queue<string>();
	}

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

	public bool Init(string Ip, int Region, short PowerAnt1, short PowerAnt2, short PowerAnt3, short PowerAnt4)
	{
		if (!MyProject.Computer.Network.Ping(Ip))
		{
			return false;
		}
		SpeedwayReaderParams speedwayReaderParams = new SpeedwayReaderParams();
		SettingsRFID = speedwayReaderParams.GetSettings();
		SettingsRFID.reader_information.reader_name = Ip;
		SettingsRFID.reader_information.region = Region;
		SettingsRFID.antennas[0].power = 30f;
		if (PowerAnt1 == 0)
		{
			SettingsRFID.antennas[0].enabled = false;
		}
		else
		{
			SettingsRFID.antennas[0].enabled = true;
		}
		SettingsRFID.antennas[1].power = 30f;
		if (PowerAnt2 == 0)
		{
			SettingsRFID.antennas[1].enabled = false;
		}
		else
		{
			SettingsRFID.antennas[1].enabled = true;
		}
		SettingsRFID.antennas[2].power = 30f;
		if (PowerAnt3 == 0)
		{
			SettingsRFID.antennas[2].enabled = false;
		}
		else
		{
			SettingsRFID.antennas[2].enabled = true;
		}
		SettingsRFID.antennas[3].power = 30f;
		if (PowerAnt4 == 0)
		{
			SettingsRFID.antennas[3].enabled = false;
		}
		else
		{
			SettingsRFID.antennas[3].enabled = true;
		}
		SettingsRFID.antennas[0].rssi = Conversions.ToShort(Interaction.IIf(PowerAnt1 == 0, -40, PowerAnt1));
		SettingsRFID.antennas[1].rssi = Conversions.ToShort(Interaction.IIf(PowerAnt2 == 0, -40, PowerAnt2));
		SettingsRFID.antennas[2].rssi = Conversions.ToShort(Interaction.IIf(PowerAnt3 == 0, -40, PowerAnt3));
		SettingsRFID.antennas[3].rssi = Conversions.ToShort(Interaction.IIf(PowerAnt4 == 0, -40, PowerAnt4));
		_State = STATE_READER.IDLE;
		if (!oRFID.Initialize(SettingsRFID.reader_information.reader_name, (REGULATORY_REGION)SettingsRFID.reader_information.region))
		{
			oRFID = null;
			return false;
		}
		SpeedwayReaderSettings settingsRFID = SettingsRFID;
		settingsRFID.gen2_params.mode_id = 1;
		settingsRFID.gen2_params.session = 1;
		settingsRFID.gen2_params.auto_set_mode = 2;
		settingsRFID.gen2_params.inventory_mode = 0;
		settingsRFID.reader_information.frequency_mode = 1;
		settingsRFID.reader_information.lbt_time = -1;
		settingsRFID = null;
		oRFID.ApplyReaderSetting(SettingsRFID);
		_IsInitialized = true;
		return true;
	}

	public bool Init(string Ip, int Region, ANTENNAS_PARAMETERS AntennaSettings)
	{
		if (!MyProject.Computer.Network.Ping(Ip))
		{
			return false;
		}
		SpeedwayReaderParams speedwayReaderParams = new SpeedwayReaderParams();
		SettingsRFID = speedwayReaderParams.GetSettings();
		SettingsRFID.reader_information.reader_name = Ip;
		SettingsRFID.reader_information.region = Region;
		SettingsRFID.antennas[0].power = AntennaSettings.Antena1.Power;
		SettingsRFID.antennas[0].rssi = AntennaSettings.Antena1.Rssi;
		SettingsRFID.antennas[0].enabled = AntennaSettings.Antena1.Enabled;
		SettingsRFID.antennas[1].power = AntennaSettings.Antena2.Power;
		SettingsRFID.antennas[1].rssi = AntennaSettings.Antena2.Rssi;
		SettingsRFID.antennas[1].enabled = AntennaSettings.Antena2.Enabled;
		SettingsRFID.antennas[2].power = AntennaSettings.Antena3.Power;
		SettingsRFID.antennas[2].rssi = AntennaSettings.Antena3.Rssi;
		SettingsRFID.antennas[2].enabled = AntennaSettings.Antena3.Enabled;
		SettingsRFID.antennas[3].power = AntennaSettings.Antena4.Power;
		SettingsRFID.antennas[3].rssi = AntennaSettings.Antena4.Rssi;
		SettingsRFID.antennas[3].enabled = AntennaSettings.Antena4.Enabled;
		_State = STATE_READER.IDLE;
		if (!oRFID.Initialize(SettingsRFID.reader_information.reader_name, (REGULATORY_REGION)SettingsRFID.reader_information.region))
		{
			oRFID = null;
			return false;
		}
		SpeedwayReaderSettings settingsRFID = SettingsRFID;
		settingsRFID.gen2_params.mode_id = 1;
		settingsRFID.gen2_params.session = 1;
		settingsRFID.gen2_params.auto_set_mode = 2;
		settingsRFID.gen2_params.inventory_mode = 0;
		settingsRFID.reader_information.frequency_mode = 1;
		settingsRFID.reader_information.lbt_time = -1;
		settingsRFID = null;
		oRFID.ApplyReaderSetting(SettingsRFID);
		_IsInitialized = true;
		return true;
	}

	public void CloseConnection()
	{
		if (_State != STATE_READER.IDLE)
		{
			Stop();
		}
		try
		{
			oRFID.Disconnect();
		}
		catch (Exception projectError)
		{
			ProjectData.SetProjectError(projectError);
			ProjectData.ClearProjectError();
		}
		_IsInitialized = false;
		_State = STATE_READER.IDLE;
	}

	public bool Stop()
	{
		bool result;
		try
		{
			if (oRFID.ModemStop() != CMD_RETURN.COMMAND_SUCESS)
			{
				result = false;
				goto IL_0069;
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			result = false;
			ProjectData.ClearProjectError();
			goto IL_0069;
		}
		finally
		{
			OnStopped?.Invoke(this);
		}
		Thread.Sleep(500);
		_State = STATE_READER.IDLE;
		result = true;
		goto IL_0069;
		IL_0069:
		return result;
	}

	public bool StartRead(bool FilterNews)
	{
		OPERATION_CMD.INVENTORY_PRAMA iNVENTORY_PRAMA = new OPERATION_CMD.INVENTORY_PRAMA();
		iNVENTORY_PRAMA.enable_halt_filter = false;
		iNVENTORY_PRAMA.enable_inventory_filter = false;
		_FilterNews = FilterNews;
		_RecTags.Clear();
		if ((oRFID.Inventory(iNVENTORY_PRAMA, out bool err_occur) == CMD_RETURN.COMMAND_SUCESS && !err_occur) ? true : false)
		{
			_State = STATE_READER.READING;
			return true;
		}
		return false;
	}

	public bool StartWrite(string Value)
	{
		if (Value.Length > 12)
		{
			return false;
		}
		_RecTags.Clear();
		_DataToWrite = EncodeText(Value);
		return StartWrite();
	}

	public bool StartWrite(byte[] Value)
	{
		if (Value.Length > 12)
		{
			return false;
		}
		_RecTags.Clear();
		_DataToWrite = EncodeBytes(Value);
		return StartWrite();
	}

	public void SetPulseOnRead(ANTENAS Antena, int PulseTime)
	{
		try
		{
			switch ((int)Antena)
			{
			case 1:
				AntenaPulse.Add(OPERATION_NTF.INVENTORY_NTF.E_ANTENNA.ANTENNA1, PulseTime);
				break;
			case 2:
				AntenaPulse.Add(OPERATION_NTF.INVENTORY_NTF.E_ANTENNA.ANTENNA2, PulseTime);
				break;
			case 3:
				AntenaPulse.Add(OPERATION_NTF.INVENTORY_NTF.E_ANTENNA.ANTENNA3, PulseTime);
				break;
			case 4:
				AntenaPulse.Add(OPERATION_NTF.INVENTORY_NTF.E_ANTENNA.ANTENNA4, PulseTime);
				break;
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			ProjectData.ClearProjectError();
		}
	}

	public int SetOut(int Output, OUT_STATES State)
	{
		MANAGEMENT_CMD.GPO_CONFIG[] array = new MANAGEMENT_CMD.GPO_CONFIG[1]
		{
			new MANAGEMENT_CMD.GPO_CONFIG()
		};
		switch (Output)
		{
		case 0:
			array[0].id = MANAGEMENT_CMD.GPO_CONFIG.GPO_ID.GPO0;
			break;
		case 1:
			array[0].id = MANAGEMENT_CMD.GPO_CONFIG.GPO_ID.GPO1;
			break;
		case 2:
			array[0].id = MANAGEMENT_CMD.GPO_CONFIG.GPO_ID.GPO2;
			break;
		case 3:
			array[0].id = MANAGEMENT_CMD.GPO_CONFIG.GPO_ID.GPO3;
			break;
		}
		if (State == OUT_STATES.HIGH)
		{
			array[0].configuration = MANAGEMENT_CMD.GPO_CONFIG.CONFIG.HIGH;
		}
		else
		{
			array[0].configuration = MANAGEMENT_CMD.GPO_CONFIG.CONFIG.LOW;
		}
		MANAGEMENT_CMD.SET_GPO_RSP rsp;
		CMD_RETURN cMD_RETURN = oRFID.SetGPO(array, out rsp);
		Trace.TraceInformation("Set GPO {0} to level {1} with result {2}", Output, array[0].configuration.ToString(), cMD_RETURN);
		return (int)cMD_RETURN;
	}

	public void ClearTagsInMemory()
	{
		_TagsQueue.Clear();
		_RecTags.Clear();
	}

	public bool StartWrite()
	{
		OPERATION_CMD.INVENTORY_PRAMA iNVENTORY_PRAMA = new OPERATION_CMD.INVENTORY_PRAMA();
		iNVENTORY_PRAMA.enable_inventory_filter = false;
		iNVENTORY_PRAMA.enable_halt_filter = true;
		iNVENTORY_PRAMA.inventory_halt_condition.halt_operation = OPERATION_CMD.INVENTORY_PRAMA.INVENTORY_HALT_OPERATION.HALT_EVERY_TAG;
		if ((oRFID.Inventory(iNVENTORY_PRAMA, out bool err_occur) == CMD_RETURN.COMMAND_SUCESS && !err_occur) ? true : false)
		{
			_State = STATE_READER.WRITING;
			return true;
		}
		return false;
	}

	public ushort[] EncodeText(string Data)
	{
		string text = "";
		checked
		{
			int num = Data.Length - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				text += Conversion.Hex(Strings.Asc(Data[num2])).PadLeft(2, '0');
				num2++;
			}
			int num5 = num2;
			while (true)
			{
				int num6 = num5;
				int num4 = 11;
				if (num6 > num4)
				{
					break;
				}
				text += "00";
				num5++;
			}
			ushort[] array = new ushort[6];
			num5 = 0;
			int num7 = text.Length - 1;
			num2 = 0;
			while (true)
			{
				int num8 = num2;
				int num4 = num7;
				if (num8 > num4)
				{
					break;
				}
				array[num5] = (ushort)Math.Round(Conversion.Val("&h" + text.Substring(num2, 4)));
				num5++;
				num2 += 4;
			}
			return array;
		}
	}

	public ushort[] EncodeBytes(byte[] Data)
	{
		string text = "";
		checked
		{
			int num = Data.Length - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				text += Conversion.Hex(Data[num2]).PadLeft(2, '0');
				num2++;
			}
			int num5 = num2;
			while (true)
			{
				int num6 = num5;
				int num4 = 11;
				if (num6 > num4)
				{
					break;
				}
				text += "00";
				num5++;
			}
			ushort[] array = new ushort[6];
			num5 = 0;
			int num7 = text.Length - 1;
			num2 = 0;
			while (true)
			{
				int num8 = num2;
				int num4 = num7;
				if (num8 > num4)
				{
					break;
				}
				array[num5] = (ushort)Math.Round(Conversion.Val("&h" + text.Substring(num2, 4)));
				num5++;
				num2 += 4;
			}
			return array;
		}
	}

	public string DecodeToString(ushort[] Data)
	{
		string text = "";
		checked
		{
			int num = Data.Length - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				byte[] bytes = BitConverter.GetBytes(Data[num2]);
				text += Conversion.Hex(bytes[1]).PadLeft(2, '0');
				text += Conversion.Hex(bytes[0]).PadLeft(2, '0');
				num2++;
			}
			return text;
		}
	}

	private void DoPulse(object State)
	{
		OPERATION_NTF.INVENTORY_NTF.E_ANTENNA e_ANTENNA = (OPERATION_NTF.INVENTORY_NTF.E_ANTENNA)Conversions.ToInteger(State);
		MANAGEMENT_CMD.GPO_CONFIG[] array = new MANAGEMENT_CMD.GPO_CONFIG[1]
		{
			new MANAGEMENT_CMD.GPO_CONFIG()
		};
		switch ((int)e_ANTENNA)
		{
		case 1:
			array[0].id = MANAGEMENT_CMD.GPO_CONFIG.GPO_ID.GPO0;
			break;
		case 2:
			array[0].id = MANAGEMENT_CMD.GPO_CONFIG.GPO_ID.GPO1;
			break;
		case 4:
			array[0].id = MANAGEMENT_CMD.GPO_CONFIG.GPO_ID.GPO2;
			break;
		case 8:
			array[0].id = MANAGEMENT_CMD.GPO_CONFIG.GPO_ID.GPO3;
			break;
		}
		array[0].configuration = MANAGEMENT_CMD.GPO_CONFIG.CONFIG.HIGH;
		CMD_RETURN cMD_RETURN = oRFID.SetGPO(array, out var rsp);
		Trace.TraceInformation("Set GPO {0} with result {1}", array[0].id, cMD_RETURN);
		long num = MyProject.Computer.Clock.TickCount;
		while (checked(MyProject.Computer.Clock.TickCount - num) < AntenaPulse[OPERATION_NTF.INVENTORY_NTF.E_ANTENNA.ANTENNA1])
		{
		}
		array[0].configuration = MANAGEMENT_CMD.GPO_CONFIG.CONFIG.LOW;
		cMD_RETURN = oRFID.SetGPO(array, out rsp);
		Trace.TraceInformation("Set GPO {0} with result {1}", array[0].id, cMD_RETURN);
	}

	public void oRFID_OnNewTagReceived(OPERATION_NTF.INVENTORY_NTF Inv)
	{
		switch ((int)_State)
		{
		case 1:
			if (!_FilterNews)
			{
				OnNewTagRead?.Invoke(this, Inv.EPC, Inv.rssi, (int)Inv.antenna);
				break;
			}
			if (!_RecTags.ContainsKey(Inv.EPC))
			{
				AntenaMask = (int)Inv.antenna;
				_RecTags.Add(Inv.EPC, AntenaMask);
				if (_TagsOnFilterMemory > 0)
				{
					_TagsQueue.Enqueue(Inv.EPC);
				}
				if (AntenaPulse.ContainsKey(Inv.antenna))
				{
					Trace.TraceInformation("Invoca a DoPulse");
					ThreadPool.QueueUserWorkItem(DoPulse, Inv.antenna);
				}
				OnNewTagRead?.Invoke(this, Inv.EPC, Inv.rssi, (int)Inv.antenna);
			}
			else
			{
				AntenaMask = _RecTags[Inv.EPC];
				if (((uint)AntenaMask & (uint)Inv.antenna) == 0)
				{
					_RecTags[Inv.EPC] = _RecTags[Inv.EPC] | (int)Inv.antenna;
					if (AntenaPulse.ContainsKey(Inv.antenna))
					{
						Trace.TraceInformation("Invoca a DoPulse");
						ThreadPool.QueueUserWorkItem(DoPulse, Inv.antenna);
					}
					OnNewTagRead?.Invoke(this, Inv.EPC, Inv.rssi, (int)Inv.antenna);
				}
			}
			if ((_TagsOnFilterMemory > 0) & (_TagsQueue.Count > _TagsOnFilterMemory))
			{
				_RecTags.Remove(_TagsQueue.Dequeue());
			}
			break;
		case 2:
		{
			OPERATION_NTF.TAG_WRITE_NTF rsp = new OPERATION_NTF.TAG_WRITE_NTF(new byte[0]);
			oRFID.TagWrite(OPERATION_CMD.MEMORY_BANK.EPC, 2, _DataToWrite, disable_block_write: true, out rsp);
			if (rsp.result_code == OPERATION_NTF.TAG_ACCESS_RESULT_CODE.SUCCEEDED)
			{
				OnTagWritten?.Invoke(this, DecodeToString(_DataToWrite));
				OnTagWrittenExtended?.Invoke(this, Inv.EPC, DecodeToString(_DataToWrite));
				Stop();
				_State = STATE_READER.WAIT_NO_TAG_PRESENT;
				_TimeWithoutTag = 0;
				StartRead(FilterNews: false);
				_TimerNoTagPresent = new Timer(IsTagNoPresent, null, 200, 300);
			}
			break;
		}
		case 3:
			_TimeWithoutTag = 0;
			break;
		}
		oRFID.InventoryContinue(out var _);
	}

	public void oRFID_OnError(MANAGEMENT_NTF.SYSTEM_ERROR_NTF Sen)
	{
		OnError?.Invoke(this, Sen.err_reason.ToString());
	}

	public void IsTagNoPresent(object State)
	{
		checked
		{
			_TimeWithoutTag++;
			if (_TimeWithoutTag >= 2)
			{
				_TimerNoTagPresent.Change(-1, -1);
				_TimerNoTagPresent.Dispose();
				OnNoTagPresent?.Invoke(this);
			}
		}
	}
}
