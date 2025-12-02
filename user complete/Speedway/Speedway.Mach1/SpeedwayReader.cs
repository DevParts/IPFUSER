using System;
using System.Collections;
using System.Data;
using System.Net.Sockets;
using System.Threading;

namespace Speedway.Mach1;

[Serializable]
public class SpeedwayReader : MarshalByRefObject, IDisposable
{
	public const ushort buffer_size = 2048;

	private const int port = 49380;

	public int MODEM_INIT_TIME_OUT = 8000;

	public int CMD_RESPONSE_TIME_OUT = 300;

	public int MODEM_STOP_TIME_OUT = 2000;

	public int NOTIFICTION_TIME_OUT = 300;

	public bool include_timestamp;

	public SpeedwayReaderSettings reader_setting = new SpeedwayReaderSettings();

	private bool connected;

	private TcpClient client;

	private NetworkStream ns;

	private AsynReadState read_state;

	private InvalidCommandNtf lastError;

	private byte[] reserved_data;

	private ArrayList inventoryBuffer;

	private TIME_STAMP inventory_start_time = new TIME_STAMP();

	private TIME_STAMP inventory_stop_time = new TIME_STAMP();

	private SpeedwayManualResetEvent bootModemNtfEvent;

	private SpeedwayManualResetEvent bootModemRspEvent;

	private SpeedwayManualResetEvent getMCSVersionRspEvent;

	private SpeedwayManualResetEvent getReaderInfoRspEvent;

	private SpeedwayManualResetEvent getStateRspEvent;

	private SpeedwayManualResetEvent shutdownModemRspEvent;

	private SpeedwayManualResetEvent getTemperatureAlarmRspEvent;

	private SpeedwayManualResetEvent setTemperatureAlarmRspEvent;

	private SpeedwayManualResetEvent setGPORspEvent;

	private SpeedwayManualResetEvent setGPIRspEvent;

	private SpeedwayManualResetEvent getGPIRspEvent;

	private SpeedwayManualResetEvent setStatusReportRspEvent;

	private SpeedwayManualResetEvent setTcpConnectionOptionsRspEvent;

	private SpeedwayManualResetEvent getTcpConnectionOptionsRspEvent;

	private SpeedwayManualResetEvent getOCSVersionRspEvent;

	private SpeedwayManualResetEvent loadFromProfileRspEvent;

	private SpeedwayManualResetEvent storeToProfileRspEvent;

	private SpeedwayManualResetEvent setTxPowerRspEvent;

	private SpeedwayManualResetEvent getTxPowerRspEvent;

	private SpeedwayManualResetEvent getSupportedGen2ParamsRspEvent;

	private SpeedwayManualResetEvent setAntennaRspEvent;

	private SpeedwayManualResetEvent getAntennaRspEvent;

	private SpeedwayManualResetEvent setTxFrequencyRspEvent;

	private SpeedwayManualResetEvent getTxFrequencyRspEvent;

	private SpeedwayManualResetEvent setGen2ParamsRspEvent;

	private SpeedwayManualResetEvent getGen2ParamsRspEvent;

	private SpeedwayManualResetEvent checkAntennaRspEvent;

	private SpeedwayManualResetEvent checkAntennaNtfEvent;

	private SpeedwayManualResetEvent setInventoryReportRspEvent;

	private SpeedwayManualResetEvent setLBTParamsRspEvent;

	private SpeedwayManualResetEvent getLBTParamRspEvent;

	private SpeedwayManualResetEvent inventoryContinueRspEvent;

	private SpeedwayManualResetEvent inventoryRspEvent;

	private SpeedwayManualResetEvent modemStopRspEvent;

	private SpeedwayManualResetEvent rfSurveyRspEvent;

	private SpeedwayManualResetEvent tagReadRspEvent;

	private SpeedwayManualResetEvent tagWriteRspEvent;

	private SpeedwayManualResetEvent tagLockRspEvent;

	private SpeedwayManualResetEvent tagKillRspEvent;

	private SpeedwayManualResetEvent tagCustomRspEvent;

	private SpeedwayManualResetEvent tagReadNtfEvent;

	private SpeedwayManualResetEvent tagWriteNtfEvent;

	private SpeedwayManualResetEvent tagLockNtfEvent;

	private SpeedwayManualResetEvent tagKillNtfEvent;

	private SpeedwayManualResetEvent invalidCommandNtfEvent;

	private SpeedwayManualResetEvent setRxConfigRspEvent;

	private SpeedwayManualResetEvent getRxConfigRspEvent;

	private SpeedwayManualResetEvent setRegionNtfEvent;

	private SpeedwayManualResetEvent setRegionRspEvent;

	private SpeedwayManualResetEvent reportInventoryRspEvent;

	private SpeedwayManualResetEvent getCapabilitiesRspEvent;

	private SpeedwayManualResetEvent testWriteRspEvent;

	private SpeedwayManualResetEvent testWriteNtfEvent;

	private SpeedwayManualResetEvent setProfileSequenceRspEvent;

	public bool Connected => connected;

	public SpeedwayReaderSettings Settings => reader_setting;

	public TIME_STAMP InventoryStartTime => inventory_start_time;

	public TIME_STAMP InventoryStopTime => inventory_stop_time;

	public InvalidCommandNtf GetLastInvalidCommandNotification
	{
		get
		{
			try
			{
				return lastError;
			}
			catch
			{
				return null;
			}
		}
	}

	public event delegateBootModemNtf onBootModemNtfReceived;

	public event delegateGPIAlertNtf onGPIAlertNtfReceived;

	public event delegateSocketConnectionStatusNtf onSocketConnectionStatusNtfReceived;

	public event delegateStatusReportNtf onStatusReportNtfReceived;

	public event delegateSystemErrorNtf onSystemErrorNtfReceived;

	public event delegateTemperatureAlarmNtf onTemperatureAlarmNtfReceived;

	public event delegateInventoryNtf onInventoryNtfReceived;

	public event delegateAntennaAlertNtf onAntennaAlertNtfReceived;

	public event delegateInventoryStatusNtf onInventoryStatusNtfReceived;

	public event delegateRfSurveyNtf onRFSurveyNtfReceived;

	public event delegateModemStoppedNtf onModemStopNtfReceived;

	public event delegateAccumulateStatusNtf onAccumulationStatusNtfReceived;

	public event delegateTagRead onTagRead;

	private void TriggerAsynInventory(OPERATION_NTF.INVENTORY_NTF inv)
	{
		if (this.onInventoryNtfReceived != null)
		{
			this.onInventoryNtfReceived(inv);
		}
	}

	private void TriggerAsynAntennaAlert(OPERATION_NTF.ANTENNA_ALERT_NTF aan)
	{
		if (this.onAntennaAlertNtfReceived != null)
		{
			this.onAntennaAlertNtfReceived(aan);
		}
	}

	private void TriggerAsynRfSurvey(OPERATION_NTF.RF_SURVEY_NTF rsn)
	{
		if (this.onRFSurveyNtfReceived != null)
		{
			this.onRFSurveyNtfReceived(rsn);
		}
	}

	private void TriggerAsynModemStopped(OPERATION_NTF.MODEM_STOPPED_NTF msn)
	{
		if (this.onModemStopNtfReceived != null)
		{
			this.onModemStopNtfReceived(msn);
		}
	}

	private void TriggerAsynInventoryStatus(OPERATION_NTF.INVENTORY_STATUS_NTF isn)
	{
		if (this.onInventoryStatusNtfReceived != null)
		{
			this.onInventoryStatusNtfReceived(isn);
		}
	}

	private void TriggerAsynAccumulationStatus(OPERATION_NTF.ACCUMULATION_STATUS_NTF asn)
	{
		if (this.onAccumulationStatusNtfReceived != null)
		{
			this.onAccumulationStatusNtfReceived(asn);
		}
	}

	private void TriggerAsynTagRead(Tag tag)
	{
		if (this.onTagRead != null)
		{
			this.onTagRead(tag);
		}
	}

	private void TriggerAsynTemperatureAlarm(MANAGEMENT_NTF.TEMPERETURE_ALARM_NTF tan)
	{
		if (this.onTemperatureAlarmNtfReceived != null)
		{
			this.onTemperatureAlarmNtfReceived(tan);
		}
	}

	private void TriggerAsynSystemError(MANAGEMENT_NTF.SYSTEM_ERROR_NTF sen)
	{
		if (this.onSystemErrorNtfReceived != null)
		{
			this.onSystemErrorNtfReceived(sen);
		}
	}

	private void TriggerAsynStatusReport(MANAGEMENT_NTF.STATUS_REPORT_NTF srn)
	{
		if (this.onStatusReportNtfReceived != null)
		{
			this.onStatusReportNtfReceived(srn);
		}
	}

	private void TriggerAsynGPIAlert(MANAGEMENT_NTF.GPI_ALERT_NTF gan)
	{
		if (this.onGPIAlertNtfReceived != null)
		{
			this.onGPIAlertNtfReceived(gan);
		}
	}

	private void TriggerAsynModemBootStatus(MANAGEMENT_NTF.BOOT_MODEM_NTF bmn)
	{
		if (this.onBootModemNtfReceived != null)
		{
			this.onBootModemNtfReceived(bmn);
		}
	}

	private void TriggerSocketConnectionStatus(MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS_NTF ssn)
	{
		if (this.onSocketConnectionStatusNtfReceived != null)
		{
			this.onSocketConnectionStatusNtfReceived(ssn);
		}
	}

	public override object InitializeLifetimeService()
	{
		return null;
	}

	public SpeedwayReader()
	{
		inventoryBuffer = new ArrayList();
	}

	private void FlushInventoryBuffer()
	{
		inventoryBuffer = new ArrayList();
	}

	public DataSet GetBufferedInventoryData()
	{
		DataSet dataSet = new DataSet();
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("EPC");
		dataTable.Columns.Add("Antenna");
		dataTable.Columns.Add("Rssi");
		dataTable.Columns.Add("TimeStamp_Sec");
		dataTable.Columns.Add("TimeStamp_uSec");
		dataSet.Tables.Add(dataTable);
		foreach (Tag item in inventoryBuffer)
		{
			dataSet.Tables[0].Rows.Add(item.epc, item.antenna.ToString(), item.rssi.ToString(), item.timeStamp.seconds.ToString(), item.timeStamp.u_seconds.ToString());
		}
		return dataSet;
	}

	public bool ApplyReaderSetting(SpeedwayReaderSettings setting)
	{
		if (!connected && !Initialize(setting.reader_information.reader_name, (REGULATORY_REGION)setting.reader_information.region))
		{
			return false;
		}
		byte b = 0;
		for (int i = 0; i < 4; i++)
		{
			b |= (byte)(setting.antennas[i].enabled ? (1 << i) : 0);
		}
		if (SetAntenna(b, out var _) != CMD_RETURN.COMMAND_SUCESS)
		{
			return false;
		}
		OPERATION_CMD.GEN2_PARAM gEN2_PARAM = new OPERATION_CMD.GEN2_PARAM();
		gEN2_PARAM.auto_set_mode = (OPERATION_CMD.GEN2_PARAM.GEN2_LINK_MODE)setting.gen2_params.auto_set_mode;
		gEN2_PARAM.inv_search_mode = (OPERATION_CMD.GEN2_PARAM.INVENTORY_SEARCH_MODE)setting.gen2_params.inventory_mode;
		gEN2_PARAM.mode_id = (OPERATION_CMD.GEN2_PARAM.MODE_ID)setting.gen2_params.mode_id;
		gEN2_PARAM.session = (OPERATION_CMD.GEN2_PARAM.SESSION)setting.gen2_params.session;
		if (SetGen2Params(gEN2_PARAM, out var _) != CMD_RETURN.COMMAND_SUCESS)
		{
			return false;
		}
		bool err_ocurr;
		if (!setting.maximum_sensitivity)
		{
			short[] sensitivities = new short[4]
			{
				setting.antennas[0].rssi,
				setting.antennas[1].rssi,
				setting.antennas[2].rssi,
				setting.antennas[3].rssi
			};
			if (SetRxConfig(OPERATION_CMD.SET_RX_SENSITIVITY_MODE.FIXED_PER_ANTENNA, sensitivities, out err_ocurr) != CMD_RETURN.COMMAND_SUCESS)
			{
				return false;
			}
		}
		float[] power = new float[5]
		{
			setting.antennas[0].power,
			setting.antennas[0].power,
			setting.antennas[1].power,
			setting.antennas[2].power,
			setting.antennas[3].power
		};
		OPERATION_CMD.SET_TX_POWER_RESULT result_code;
		CMD_RETURN cMD_RETURN = SetTxPower(power, out result_code);
		if (cMD_RETURN != CMD_RETURN.COMMAND_SUCESS)
		{
			return false;
		}
		OPERATION_CMD.SET_FREQUENCY_RESULT result3;
		switch ((OPERATION_CMD.FREQUENCY_SET_MODE)setting.reader_information.frequency_mode)
		{
		case OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY:
			cMD_RETURN = SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY, setting.reader_information.frequency, null, out result3);
			break;
		case OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST:
			cMD_RETURN = SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST, 0, setting.reader_information.frequencies, out result3);
			break;
		default:
			cMD_RETURN = SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY, 0, null, out result3);
			break;
		case OPERATION_CMD.FREQUENCY_SET_MODE.REDUCED_POWER_FREQUENCY_LIST:
			break;
		}
		if (cMD_RETURN != CMD_RETURN.COMMAND_SUCESS)
		{
			return false;
		}
		if (setting.reader_information.lbt_time >= 0 && SetLBTParams(setting.reader_information.lbt_time == 1, out err_ocurr) != CMD_RETURN.COMMAND_SUCESS)
		{
			return false;
		}
		reader_setting = setting;
		return true;
	}

	public void GetReaderSettings(out SpeedwayReaderSettings setting)
	{
		setting = new SpeedwayReaderSettings();
		setting.reader_information.reader_name = reader_setting.reader_information.reader_name;
		setting.reader_information.region = reader_setting.reader_information.region;
		bool[] array = new bool[4];
		bool[] antenna_port_status = array;
		if (GetAntenna(out antenna_port_status) == CMD_RETURN.COMMAND_SUCESS)
		{
			for (int i = 0; i < 4; i++)
			{
				setting.antennas[i].enabled = antenna_port_status[i];
			}
		}
		if (GetGen2Params(report_search_mode: true, out var gen2_param) == CMD_RETURN.COMMAND_SUCESS)
		{
			setting.gen2_params.session = (int)gen2_param.session;
			setting.gen2_params.mode_id = (int)gen2_param.mode_id;
			setting.gen2_params.inventory_mode = (int)gen2_param.inv_search_mode;
			setting.gen2_params.auto_set_mode = (int)gen2_param.auto_set_mode;
		}
		if (GetLBTParams(out var lbt_time_mode, out var _) == CMD_RETURN.COMMAND_SUCESS)
		{
			setting.reader_information.lbt_time = lbt_time_mode;
		}
		if (GetReaderInfo(out var rsp) == CMD_RETURN.COMMAND_SUCESS)
		{
			setting.reader_information.software_ver = rsp.software_version;
			setting.reader_information.firmware_ver = rsp.firmware_verison;
			setting.reader_information.fpga_ver = rsp.FPGA_version;
		}
		short[] sensitivities = new short[4];
		if (GetRxConfig(out var mode, out sensitivities) == CMD_RETURN.COMMAND_SUCESS)
		{
			if (mode == OPERATION_CMD.SET_RX_SENSITIVITY_MODE.MAXIMUM_SENSITIVITY)
			{
				setting.maximum_sensitivity = true;
			}
			else
			{
				setting.maximum_sensitivity = false;
				for (int j = 0; j < 4; j++)
				{
					setting.antennas[j].rssi = sensitivities[j];
				}
			}
		}
		float[] powers = new float[4];
		if (GetTxPower(out powers) == CMD_RETURN.COMMAND_SUCESS)
		{
			for (int k = 0; k < 4; k++)
			{
				if (powers.Length == 4)
				{
					setting.antennas[k].power = powers[k];
				}
				else
				{
					setting.antennas[k].power = powers[0];
				}
			}
		}
		OPERATION_CMD.FREQUENCY_SET_MODE mode2;
		CMD_RETURN txFrequency = GetTxFrequency(out mode2, out setting.reader_information.frequency, out setting.reader_information.frequencies, out setting.reader_information.reduced_power_frequencies);
		setting.reader_information.frequency_mode = (int)mode2;
	}

	public bool Initialize(string deviceName, REGULATORY_REGION region)
	{
		try
		{
			client = new TcpClient(deviceName, 49380);
			ns = client.GetStream();
			if (ns == null)
			{
				return false;
			}
		}
		catch
		{
			return false;
		}
		read_state = new AsynReadState(2048);
		ns.BeginRead(read_state.data, 0, read_state.data.Length, OnDataRead, read_state);
		if (BootModem() == CMD_RETURN.COMMAND_SUCESS)
		{
			if (SetRegulatoryRegion(region) == CMD_RETURN.COMMAND_SUCESS)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public bool Disconnect()
	{
		try
		{
			try
			{
				ModemStop();
			}
			catch
			{
			}
			connected = false;
			client.Close();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public void Dispose()
	{
		Disconnect();
	}

	private void ProcessRSPMach1Frame(MACH1_FRAME mf)
	{
		switch (mf.header.category)
		{
		case CATEGORY.MODEM_MANAGEMENT:
			switch (mf.header.message_id)
			{
			case 4:
				if (bootModemRspEvent != null)
				{
					bootModemRspEvent.evt.Set();
				}
				break;
			case 0:
				getMCSVersionRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getMCSVersionRspEvent.data, mf.payload_len);
				}
				getMCSVersionRspEvent.evt.Set();
				break;
			case 5:
				shutdownModemRspEvent.evt.Set();
				break;
			case 7:
				setTemperatureAlarmRspEvent.evt.Set();
				break;
			case 1:
				getReaderInfoRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getReaderInfoRspEvent.data, mf.payload_len);
				}
				getReaderInfoRspEvent.evt.Set();
				break;
			case 2:
				getStateRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getStateRspEvent.data, mf.payload_len);
				}
				getStateRspEvent.evt.Set();
				break;
			case 8:
				getTemperatureAlarmRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getTemperatureAlarmRspEvent.data, mf.payload_len);
				}
				getTemperatureAlarmRspEvent.evt.Set();
				break;
			case 9:
				setGPORspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, setGPORspEvent.data, mf.payload_len);
				}
				setGPORspEvent.evt.Set();
				break;
			case 10:
				setGPIRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, setGPIRspEvent.data, mf.payload_len);
				}
				setGPIRspEvent.evt.Set();
				break;
			case 11:
				getGPIRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getGPIRspEvent.data, mf.payload_len);
				}
				getGPIRspEvent.evt.Set();
				break;
			case 12:
				setStatusReportRspEvent.evt.Set();
				break;
			case 13:
				setTcpConnectionOptionsRspEvent.evt.Set();
				break;
			case 14:
				getTcpConnectionOptionsRspEvent.data = new byte[mf.packet_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getTcpConnectionOptionsRspEvent.data, mf.payload_len);
				}
				getTcpConnectionOptionsRspEvent.evt.Set();
				break;
			case 3:
			case 6:
				break;
			}
			break;
		case CATEGORY.OPERATION_MODEL:
			switch (mf.header.message_id)
			{
			case 9:
				if (setRegionRspEvent != null)
				{
					setRegionRspEvent.evt.Set();
				}
				break;
			case 19:
				inventoryRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, inventoryRspEvent.data, mf.payload_len);
				}
				inventory_start_time.seconds = mf.timestamp_second;
				inventory_start_time.u_seconds = mf.timestamp_us;
				inventoryRspEvent.evt.Set();
				break;
			case 21:
				inventoryContinueRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, inventoryContinueRspEvent.data, mf.payload_len);
				}
				inventoryContinueRspEvent.evt.Set();
				break;
			case 0:
				getOCSVersionRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getOCSVersionRspEvent.data, mf.payload_len);
				}
				getOCSVersionRspEvent.evt.Set();
				break;
			case 1:
				loadFromProfileRspEvent.evt.Set();
				break;
			case 2:
				storeToProfileRspEvent.evt.Set();
				break;
			case 10:
				getCapabilitiesRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getCapabilitiesRspEvent.data, mf.packet_len);
				}
				getCapabilitiesRspEvent.evt.Set();
				break;
			case 5:
				setTxPowerRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, setTxPowerRspEvent.data, mf.payload_len);
				}
				setTxPowerRspEvent.evt.Set();
				break;
			case 6:
				getTxPowerRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getTxPowerRspEvent.data, mf.payload_len);
				}
				getTxPowerRspEvent.evt.Set();
				break;
			case 7:
				setAntennaRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, setAntennaRspEvent.data, mf.payload_len);
				}
				setAntennaRspEvent.evt.Set();
				break;
			case 8:
				getAntennaRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getAntennaRspEvent.data, mf.payload_len);
				}
				getAntennaRspEvent.evt.Set();
				break;
			case 11:
				setTxFrequencyRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, setTxFrequencyRspEvent.data, mf.payload_len);
				}
				setTxFrequencyRspEvent.evt.Set();
				break;
			case 12:
				getTxFrequencyRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getTxFrequencyRspEvent.data, mf.payload_len);
				}
				getTxFrequencyRspEvent.evt.Set();
				break;
			case 13:
				setGen2ParamsRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, setGen2ParamsRspEvent.data, mf.payload_len);
				}
				setGen2ParamsRspEvent.evt.Set();
				break;
			case 14:
				getGen2ParamsRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getGen2ParamsRspEvent.data, mf.payload_len);
				}
				getGen2ParamsRspEvent.evt.Set();
				break;
			case 29:
				checkAntennaRspEvent.evt.Set();
				break;
			case 30:
				setInventoryReportRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, setInventoryReportRspEvent.data, mf.payload_len);
				}
				setInventoryReportRspEvent.evt.Set();
				break;
			case 31:
				setLBTParamsRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, setLBTParamsRspEvent.data, mf.payload_len);
				}
				setLBTParamsRspEvent.evt.Set();
				break;
			case 32:
				getLBTParamRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getLBTParamRspEvent.data, mf.payload_len);
				}
				getLBTParamRspEvent.evt.Set();
				break;
			case 33:
				reportInventoryRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, reportInventoryRspEvent.data, mf.payload_len);
				}
				reportInventoryRspEvent.evt.Set();
				break;
			case 22:
				modemStopRspEvent.evt.Set();
				inventory_stop_time.seconds = mf.timestamp_second;
				inventory_stop_time.u_seconds = mf.timestamp_us;
				break;
			case 20:
				rfSurveyRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, rfSurveyRspEvent.data, mf.payload_len);
				}
				rfSurveyRspEvent.evt.Set();
				break;
			case 23:
				tagReadRspEvent.evt.Set();
				break;
			case 27:
				tagCustomRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, tagCustomRspEvent.data, mf.payload_len);
				}
				tagCustomRspEvent.evt.Set();
				break;
			case 26:
				tagKillRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, tagKillRspEvent.data, mf.payload_len);
				}
				tagKillRspEvent.evt.Set();
				break;
			case 25:
				tagLockRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, tagLockRspEvent.data, mf.payload_len);
				}
				tagLockRspEvent.evt.Set();
				break;
			case 24:
				tagWriteRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, tagWriteRspEvent.data, mf.payload_len);
				}
				tagWriteRspEvent.evt.Set();
				break;
			case 34:
				setRxConfigRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, setRxConfigRspEvent.data, mf.payload_len);
				}
				setRxConfigRspEvent.evt.Set();
				break;
			case 35:
				getRxConfigRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getRxConfigRspEvent.data, mf.payload_len);
				}
				getRxConfigRspEvent.evt.Set();
				break;
			case 28:
				getSupportedGen2ParamsRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, getSupportedGen2ParamsRspEvent.data, mf.payload_len);
				}
				getSupportedGen2ParamsRspEvent.evt.Set();
				break;
			case 36:
				setProfileSequenceRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, setProfileSequenceRspEvent.data, mf.payload_len);
				}
				setProfileSequenceRspEvent.evt.Set();
				break;
			case 3:
			case 4:
			case 15:
			case 16:
			case 17:
			case 18:
				break;
			}
			break;
		case CATEGORY.TEST:
		{
			byte message_id = mf.header.message_id;
			if (message_id == 3)
			{
				testWriteRspEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, testWriteRspEvent.data, mf.payload_len);
				}
				testWriteRspEvent.evt.Set();
			}
			break;
		}
		case CATEGORY.MACH1_PROTOCOL_ERROR:
		case CATEGORY.HARDWARE_INTERFACE:
		case CATEGORY.PRODUCTION:
		case CATEGORY.LOGGING:
			break;
		}
	}

	private void ProcessNTFMach1Frame(MACH1_FRAME mf)
	{
		switch (mf.header.category)
		{
		case CATEGORY.MODEM_MANAGEMENT:
			switch (mf.header.message_id)
			{
			case 2:
				try
				{
					MANAGEMENT_NTF.BOOT_MODEM_NTF bOOT_MODEM_NTF = new MANAGEMENT_NTF.BOOT_MODEM_NTF(mf.PAYLOAD);
					if (bootModemNtfEvent != null && bOOT_MODEM_NTF.boot_result_code == MANAGEMENT_NTF.BOOT_MODEM_NTF.BOOT_RESULT_CODE.BOOT_SUCESSFUL)
					{
						bootModemNtfEvent.evt.Set();
					}
					if (bOOT_MODEM_NTF != null && this.onBootModemNtfReceived != null)
					{
						bOOT_MODEM_NTF.timestamp_us = mf.timestamp_us;
						bOOT_MODEM_NTF.timestamp_second = mf.timestamp_second;
						bOOT_MODEM_NTF.reader_name = reader_setting.reader_information.reader_name;
						delegateBootModemNtf delegateBootModemNtf2 = TriggerAsynModemBootStatus;
						delegateBootModemNtf2.BeginInvoke(bOOT_MODEM_NTF, null, null);
					}
					break;
				}
				catch
				{
					break;
				}
			case 0:
				try
				{
					MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS_NTF sOCKET_CONNECTION_STATUS_NTF = new MANAGEMENT_NTF.SOCKET_CONNECTION_STATUS_NTF(mf.PAYLOAD);
					if (sOCKET_CONNECTION_STATUS_NTF.socket_status == SOCKET_STATUS.CONNECTION_SUCCESS)
					{
						connected = true;
					}
					if (sOCKET_CONNECTION_STATUS_NTF != null && this.onSocketConnectionStatusNtfReceived != null)
					{
						sOCKET_CONNECTION_STATUS_NTF.timestamp_us = mf.timestamp_us;
						sOCKET_CONNECTION_STATUS_NTF.timestamp_second = mf.timestamp_second;
						sOCKET_CONNECTION_STATUS_NTF.reader_name = reader_setting.reader_information.reader_name;
						delegateSocketConnectionStatusNtf delegateSocketConnectionStatusNtf2 = TriggerSocketConnectionStatus;
						delegateSocketConnectionStatusNtf2.BeginInvoke(sOCKET_CONNECTION_STATUS_NTF, null, null);
					}
					break;
				}
				catch
				{
					break;
				}
			case 4:
				try
				{
					MANAGEMENT_NTF.TEMPERETURE_ALARM_NTF tEMPERETURE_ALARM_NTF = new MANAGEMENT_NTF.TEMPERETURE_ALARM_NTF(mf.PAYLOAD);
					if (tEMPERETURE_ALARM_NTF != null && this.onTemperatureAlarmNtfReceived != null)
					{
						tEMPERETURE_ALARM_NTF.timestamp_us = mf.timestamp_us;
						tEMPERETURE_ALARM_NTF.timestamp_second = mf.timestamp_second;
						tEMPERETURE_ALARM_NTF.reader_name = reader_setting.reader_information.reader_name;
						delegateTemperatureAlarmNtf delegateTemperatureAlarmNtf2 = TriggerAsynTemperatureAlarm;
						delegateTemperatureAlarmNtf2.BeginInvoke(tEMPERETURE_ALARM_NTF, null, null);
					}
					break;
				}
				catch
				{
					break;
				}
			case 5:
				try
				{
					MANAGEMENT_NTF.GPI_ALERT_NTF gPI_ALERT_NTF = new MANAGEMENT_NTF.GPI_ALERT_NTF(mf.PAYLOAD);
					if (gPI_ALERT_NTF != null && this.onGPIAlertNtfReceived != null)
					{
						gPI_ALERT_NTF.timestamp_us = mf.timestamp_us;
						gPI_ALERT_NTF.timestamp_second = mf.timestamp_second;
						gPI_ALERT_NTF.reader_name = reader_setting.reader_information.reader_name;
						delegateGPIAlertNtf delegateGPIAlertNtf2 = TriggerAsynGPIAlert;
						delegateGPIAlertNtf2.BeginInvoke(gPI_ALERT_NTF, null, null);
					}
					break;
				}
				catch
				{
					break;
				}
			case 1:
				try
				{
					MANAGEMENT_NTF.SYSTEM_ERROR_NTF sYSTEM_ERROR_NTF = new MANAGEMENT_NTF.SYSTEM_ERROR_NTF(mf.PAYLOAD);
					if (sYSTEM_ERROR_NTF != null && this.onSystemErrorNtfReceived != null)
					{
						sYSTEM_ERROR_NTF.timestamp_us = mf.timestamp_us;
						sYSTEM_ERROR_NTF.timestamp_second = mf.timestamp_second;
						sYSTEM_ERROR_NTF.reader_name = reader_setting.reader_information.reader_name;
						delegateSystemErrorNtf delegateSystemErrorNtf2 = TriggerAsynSystemError;
						delegateSystemErrorNtf2.BeginInvoke(sYSTEM_ERROR_NTF, null, null);
					}
					break;
				}
				catch
				{
					break;
				}
			case 6:
				try
				{
					MANAGEMENT_NTF.STATUS_REPORT_NTF sTATUS_REPORT_NTF = new MANAGEMENT_NTF.STATUS_REPORT_NTF(mf.PAYLOAD);
					if (sTATUS_REPORT_NTF != null && this.onStatusReportNtfReceived != null)
					{
						sTATUS_REPORT_NTF.timestamp_us = mf.timestamp_us;
						sTATUS_REPORT_NTF.timestamp_second = mf.timestamp_second;
						sTATUS_REPORT_NTF.reader_name = reader_setting.reader_information.reader_name;
						delegateStatusReportNtf delegateStatusReportNtf2 = TriggerAsynStatusReport;
						delegateStatusReportNtf2.BeginInvoke(sTATUS_REPORT_NTF, null, null);
					}
					break;
				}
				catch
				{
					break;
				}
			case 3:
				break;
			}
			break;
		case CATEGORY.OPERATION_MODEL:
			switch (mf.header.message_id)
			{
			case 10:
				if (setRegionNtfEvent != null)
				{
					setRegionNtfEvent.evt.Set();
				}
				break;
			case 1:
			{
				OPERATION_NTF.INVENTORY_NTF iNVENTORY_NTF = new OPERATION_NTF.INVENTORY_NTF(mf.PAYLOAD);
				Tag tag = new Tag();
				switch (iNVENTORY_NTF.antenna)
				{
				case OPERATION_NTF.INVENTORY_NTF.E_ANTENNA.ANTENNA1:
					tag.antenna = 1;
					break;
				case OPERATION_NTF.INVENTORY_NTF.E_ANTENNA.ANTENNA2:
					tag.antenna = 2;
					break;
				case OPERATION_NTF.INVENTORY_NTF.E_ANTENNA.ANTENNA3:
					tag.antenna = 3;
					break;
				case OPERATION_NTF.INVENTORY_NTF.E_ANTENNA.ANTENNA4:
					tag.antenna = 4;
					break;
				default:
					tag.antenna = 1;
					break;
				}
				tag.epc = iNVENTORY_NTF.EPC;
				tag.rssi = iNVENTORY_NTF.rssi;
				tag.phaseI = iNVENTORY_NTF.phaseI;
				tag.phaseQ = iNVENTORY_NTF.phaseQ;
				tag.reader_name = Settings.reader_information.reader_name;
				try
				{
					if (tag.reader_name == null || tag.reader_name.Length == 0)
					{
						tag.reader_name = "default";
					}
				}
				catch
				{
					tag.reader_name = "default";
				}
				tag.timeStamp.seconds = mf.timestamp_second;
				tag.timeStamp.u_seconds = mf.timestamp_us;
				if (this.onTagRead != null)
				{
					try
					{
						delegateTagRead delegateTagRead2 = TriggerAsynTagRead;
						delegateTagRead2.BeginInvoke(tag, null, null);
					}
					catch
					{
					}
				}
				if (reader_setting.enable_buffering)
				{
					bool flag = false;
					foreach (Tag item in inventoryBuffer)
					{
						if (item.epc == tag.epc)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						inventoryBuffer.Add(tag);
					}
				}
				if (iNVENTORY_NTF != null && this.onInventoryNtfReceived != null)
				{
					iNVENTORY_NTF.timestamp_us = mf.timestamp_us;
					iNVENTORY_NTF.timestamp_second = mf.timestamp_second;
					iNVENTORY_NTF.reader_name = reader_setting.reader_information.reader_name;
					try
					{
						delegateInventoryNtf delegateInventoryNtf2 = TriggerAsynInventory;
						delegateInventoryNtf2.BeginInvoke(iNVENTORY_NTF, null, null);
						break;
					}
					catch
					{
						break;
					}
				}
				break;
			}
			case 0:
			{
				OPERATION_NTF.ANTENNA_ALERT_NTF aNTENNA_ALERT_NTF = new OPERATION_NTF.ANTENNA_ALERT_NTF(mf.PAYLOAD);
				if (aNTENNA_ALERT_NTF != null && this.onAntennaAlertNtfReceived != null)
				{
					try
					{
						aNTENNA_ALERT_NTF.timestamp_us = mf.timestamp_us;
						aNTENNA_ALERT_NTF.timestamp_second = mf.timestamp_second;
						aNTENNA_ALERT_NTF.reader_name = reader_setting.reader_information.reader_name;
						delegateAntennaAlertNtf delegateAntennaAlertNtf2 = TriggerAsynAntennaAlert;
						delegateAntennaAlertNtf2.BeginInvoke(aNTENNA_ALERT_NTF, null, null);
						break;
					}
					catch
					{
						break;
					}
				}
				break;
			}
			case 11:
				checkAntennaNtfEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, checkAntennaNtfEvent.data, mf.payload_len);
				}
				checkAntennaNtfEvent.evt.Set();
				break;
			case 2:
			{
				OPERATION_NTF.INVENTORY_STATUS_NTF iNVENTORY_STATUS_NTF = new OPERATION_NTF.INVENTORY_STATUS_NTF(mf.PAYLOAD);
				if (iNVENTORY_STATUS_NTF != null && this.onInventoryStatusNtfReceived != null)
				{
					iNVENTORY_STATUS_NTF.timestamp_us = mf.timestamp_us;
					iNVENTORY_STATUS_NTF.timestamp_second = mf.timestamp_second;
					iNVENTORY_STATUS_NTF.reader_name = reader_setting.reader_information.reader_name;
					try
					{
						delegateInventoryStatusNtf delegateInventoryStatusNtf2 = TriggerAsynInventoryStatus;
						delegateInventoryStatusNtf2.BeginInvoke(iNVENTORY_STATUS_NTF, null, null);
						break;
					}
					catch
					{
						break;
					}
				}
				break;
			}
			case 12:
			{
				OPERATION_NTF.ACCUMULATION_STATUS_NTF aCCUMULATION_STATUS_NTF = new OPERATION_NTF.ACCUMULATION_STATUS_NTF();
				aCCUMULATION_STATUS_NTF.code = (OPERATION_NTF.ACCUMULATION_STATUS_NTF.NTF_CODE)mf.PAYLOAD[0];
				if (aCCUMULATION_STATUS_NTF != null && this.onAccumulationStatusNtfReceived != null)
				{
					aCCUMULATION_STATUS_NTF.timestamp_us = mf.timestamp_us;
					aCCUMULATION_STATUS_NTF.timestamp_second = mf.timestamp_second;
					aCCUMULATION_STATUS_NTF.reader_name = reader_setting.reader_information.reader_name;
					try
					{
						delegateAccumulateStatusNtf delegateAccumulateStatusNtf2 = TriggerAsynAccumulationStatus;
						delegateAccumulateStatusNtf2.BeginInvoke(aCCUMULATION_STATUS_NTF, null, null);
						break;
					}
					catch
					{
						break;
					}
				}
				break;
			}
			case 4:
			{
				OPERATION_NTF.MODEM_STOPPED_NTF mODEM_STOPPED_NTF = new OPERATION_NTF.MODEM_STOPPED_NTF();
				mODEM_STOPPED_NTF.code = (OPERATION_NTF.MODEM_STOPPED_NTF.NTF_CODE)mf.PAYLOAD[0];
				if (mODEM_STOPPED_NTF != null && this.onModemStopNtfReceived != null)
				{
					mODEM_STOPPED_NTF.timestamp_us = mf.timestamp_us;
					mODEM_STOPPED_NTF.timestamp_second = mf.timestamp_second;
					mODEM_STOPPED_NTF.reader_name = reader_setting.reader_information.reader_name;
					try
					{
						delegateModemStoppedNtf delegateModemStoppedNtf2 = TriggerAsynModemStopped;
						delegateModemStoppedNtf2.BeginInvoke(mODEM_STOPPED_NTF, null, null);
						break;
					}
					catch
					{
						break;
					}
				}
				break;
			}
			case 3:
			{
				OPERATION_NTF.RF_SURVEY_NTF rF_SURVEY_NTF = new OPERATION_NTF.RF_SURVEY_NTF(mf.PAYLOAD);
				if (rF_SURVEY_NTF != null && this.onRFSurveyNtfReceived != null)
				{
					rF_SURVEY_NTF.timestamp_us = mf.timestamp_us;
					rF_SURVEY_NTF.timestamp_second = mf.timestamp_second;
					rF_SURVEY_NTF.reader_name = reader_setting.reader_information.reader_name;
					try
					{
						delegateRfSurveyNtf delegateRfSurveyNtf2 = TriggerAsynRfSurvey;
						delegateRfSurveyNtf2.BeginInvoke(rF_SURVEY_NTF, null, null);
						break;
					}
					catch
					{
						break;
					}
				}
				break;
			}
			case 5:
				tagReadNtfEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, tagReadNtfEvent.data, mf.payload_len);
				}
				tagReadNtfEvent.evt.Set();
				break;
			case 6:
				tagWriteNtfEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, tagWriteNtfEvent.data, mf.payload_len);
				}
				tagWriteNtfEvent.evt.Set();
				break;
			case 7:
				tagLockNtfEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, tagLockNtfEvent.data, mf.payload_len);
				}
				tagLockNtfEvent.evt.Set();
				break;
			case 8:
				tagKillNtfEvent.data = new byte[mf.payload_len];
				if (mf.PAYLOAD != null)
				{
					Array.Copy(mf.PAYLOAD, tagKillNtfEvent.data, mf.payload_len);
				}
				tagKillNtfEvent.evt.Set();
				break;
			case 9:
				break;
			}
			break;
		case CATEGORY.MACH1_PROTOCOL_ERROR:
			invalidCommandNtfEvent.data = new byte[mf.payload_len];
			if (mf.PAYLOAD != null)
			{
				Array.Copy(mf.PAYLOAD, invalidCommandNtfEvent.data, mf.payload_len);
			}
			invalidCommandNtfEvent.evt.Set();
			try
			{
				lastError = new InvalidCommandNtf(mf.PAYLOAD);
				break;
			}
			catch
			{
				break;
			}
		case CATEGORY.HARDWARE_INTERFACE:
		case CATEGORY.PRODUCTION:
		case CATEGORY.LOGGING:
		case CATEGORY.TEST:
			break;
		}
	}

	private void ProcessMach1Frame(MACH1_FRAME mf)
	{
		if (mf.header.is_ntf)
		{
			ProcessNTFMach1Frame(mf);
		}
		else
		{
			ProcessRSPMach1Frame(mf);
		}
	}

	private void OnDataRead(IAsyncResult ar)
	{
		int num = 0;
		AsynReadState asynReadState = (AsynReadState)ar.AsyncState;
		try
		{
			byte[] array;
			if (reserved_data != null)
			{
				array = new byte[asynReadState.data.Length + reserved_data.Length];
				Array.Copy(reserved_data, array, reserved_data.Length);
				Array.Copy(asynReadState.data, 0, array, reserved_data.Length, asynReadState.data.Length);
				reserved_data = null;
			}
			else
			{
				array = new byte[asynReadState.data.Length];
				Array.Copy(asynReadState.data, array, asynReadState.data.Length);
			}
			while (num < array.Length && (array[num] == 238 || array[num] == 239))
			{
				byte[] array2 = new byte[array.Length - num];
				Array.Copy(array, num, array2, 0, array.Length - num);
				try
				{
					MACH1_FRAME mACH1_FRAME = MACH1_FRAME.ParseMachData(array2, out reserved_data);
					if (mACH1_FRAME != null)
					{
						num += mACH1_FRAME.packet_len;
						ProcessMach1Frame(mACH1_FRAME);
						continue;
					}
				}
				catch
				{
				}
				break;
			}
			if (connected && ns != null)
			{
				try
				{
					ns.Flush();
					read_state = new AsynReadState(2048);
					ns.BeginRead(read_state.data, 0, read_state.data.Length, OnDataRead, read_state);
					return;
				}
				catch
				{
					return;
				}
			}
		}
		catch
		{
		}
	}

	public CMD_RETURN BootModem()
	{
		bootModemRspEvent = new SpeedwayManualResetEvent(status: false);
		bootModemNtfEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { bootModemRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_BOOT_MODEM_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, MODEM_INIT_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!bootModemNtfEvent.evt.WaitOne(MODEM_INIT_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.COMMAND_FAILED;
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN GetMCSVersion(out MANAGEMENT_CMD.MCS_VERSION_RSP rsp)
	{
		getMCSVersionRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getMCSVersionRspEvent.evt, invalidCommandNtfEvent.evt };
		rsp = null;
		byte[] array = MANAGEMENT_CMD.GENERATE_GET_MCS_VERSION_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new MANAGEMENT_CMD.MCS_VERSION_RSP(getMCSVersionRspEvent.data);
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN GetReaderInfo(out MANAGEMENT_CMD.READER_INFO_RSP rsp)
	{
		getReaderInfoRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getReaderInfoRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_GET_READER_INFO_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		rsp = null;
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new MANAGEMENT_CMD.READER_INFO_RSP(getReaderInfoRspEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN GetState(out MANAGEMENT_CMD.STATE_RSP rsp)
	{
		getStateRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getStateRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_GET_STATE_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		rsp = null;
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new MANAGEMENT_CMD.STATE_RSP(getStateRspEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN ShutDownModem()
	{
		shutdownModemRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { shutdownModemRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_SHUTDOWN_MODEM_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN SetTempretureAlarm(MANAGEMENT_CMD.TERMPERETURE_ALARM_MODE mode, ushort periodic, short alert_threshold)
	{
		setTemperatureAlarmRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setTemperatureAlarmRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_SET_TEMPERETURE_ALARM_CMD(include_timestamp, mode, periodic, alert_threshold);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN GetTemperatureAlarm(out MANAGEMENT_CMD.TEMPERETURE_ALARM_RSP rsp)
	{
		getTemperatureAlarmRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getTemperatureAlarmRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_GET_TEMPERETURE_ALARM_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		rsp = null;
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new MANAGEMENT_CMD.TEMPERETURE_ALARM_RSP(getTemperatureAlarmRspEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN SetGPO(MANAGEMENT_CMD.GPO_CONFIG[] gpo_config, out MANAGEMENT_CMD.SET_GPO_RSP rsp)
	{
		setGPORspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setGPORspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_SET_GPO_CMD(include_timestamp, gpo_config);
		ns.Write(array, 0, array.Length);
		rsp = null;
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new MANAGEMENT_CMD.SET_GPO_RSP(setGPORspEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN SetGPI(MANAGEMENT_CMD.GPI_CONFIG[] gpi_config, out MANAGEMENT_CMD.SET_GPI_RSP rsp)
	{
		setGPIRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setGPIRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_SET_GPI_CMD(include_timestamp, gpi_config);
		ns.Write(array, 0, array.Length);
		rsp = null;
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new MANAGEMENT_CMD.SET_GPI_RSP(setGPIRspEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN GetGPI(out MANAGEMENT_CMD.GET_GPI_RSP rsp)
	{
		getGPIRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getGPIRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_GET_GPI_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		rsp = null;
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new MANAGEMENT_CMD.GET_GPI_RSP(getGPIRspEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN SetStatusReport(bool enable)
	{
		setStatusReportRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setStatusReportRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_SET_STATUS_REPORT_CMD(include_timestamp, enable);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN SetTCPConnectionOptions(MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR behavior)
	{
		setTcpConnectionOptionsRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setTcpConnectionOptionsRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_SET_TCP_CONNECTION_OPTIONS_CMD(include_timestamp, behavior);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN GetTCPConnectionOptions(bool report_behavior, out MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR behavior)
	{
		getTcpConnectionOptionsRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		behavior = MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR.DEFAULT;
		WaitHandle[] waitHandles = new WaitHandle[2] { getTcpConnectionOptionsRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = MANAGEMENT_CMD.GENERATE_GET_TCP_CONNECTION_OPTIONS_CMD(include_timestamp, report_behavior);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (getTcpConnectionOptionsRspEvent.data.Length > 2)
		{
			behavior = (MANAGEMENT_CMD.MESSAGE_FLUSH_BEHAVIOR)getTcpConnectionOptionsRspEvent.data[2];
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN SetRegulatoryRegion(REGULATORY_REGION region)
	{
		setRegionNtfEvent = new SpeedwayManualResetEvent(status: false);
		setRegionRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setRegionRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_SET_REGULATORY_REGION(include_timestamp, region);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!setRegionNtfEvent.evt.WaitOne(MODEM_INIT_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.COMMAND_FAILED;
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN Inventory(OPERATION_CMD.INVENTORY_PRAMA prama, out OPERATION_CMD.INVENTORY_RESULT result)
	{
		inventoryRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { inventoryRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_INVENTORY_CMD(include_timestamp, prama);
		result = OPERATION_CMD.INVENTORY_RESULT.FAIL_CONFIGURATION_ERROR;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			result = (OPERATION_CMD.INVENTORY_RESULT)inventoryRspEvent.data[0];
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN Inventory(OPERATION_CMD.INVENTORY_PRAMA prama, out bool err_occur)
	{
		inventoryRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { inventoryRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_INVENTORY_CMD(include_timestamp, prama);
		err_occur = true;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			err_occur = inventoryRspEvent.data[0] == 1;
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN InventoryContinue(out bool err_ocurr)
	{
		inventoryContinueRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { inventoryContinueRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_INVENTORY_CONTINUE_CMD(include_timestamp);
		err_ocurr = true;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		err_ocurr = inventoryContinueRspEvent.data[0] == 1;
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN GetOCSVersion(out OPERATION_CMD.OCS_VERSION_RSP rsp)
	{
		getOCSVersionRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getOCSVersionRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_GET_OCS_VERSION_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		rsp = null;
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new OPERATION_CMD.OCS_VERSION_RSP(getOCSVersionRspEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN LoadFromProfile(ushort profile_index)
	{
		loadFromProfileRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { loadFromProfileRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_LOAD_FROM_PROFILE_CMD(include_timestamp, profile_index);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN StoreToProfile(ushort profile_index, OPERATION_CMD.SOURCE dst)
	{
		storeToProfileRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { storeToProfileRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_STORE_TO_PROFILE_CMD(include_timestamp, profile_index, dst);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN SetTxPower(float[] power, out OPERATION_CMD.SET_TX_POWER_RESULT result_code)
	{
		byte[] array = new byte[power.Length];
		for (int i = 0; i < power.Length; i++)
		{
			array[i] = (byte)(68f + power[i] * 4f);
		}
		setTxPowerRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setTxPowerRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array2 = OPERATION_CMD.GENERATE_SET_TX_POWER_CMD(include_timestamp, array);
		result_code = OPERATION_CMD.SET_TX_POWER_RESULT.SUCESS;
		ns.Write(array2, 0, array2.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		result_code = (OPERATION_CMD.SET_TX_POWER_RESULT)setTxPowerRspEvent.data[0];
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN GetTxPower(out float[] powers)
	{
		getTxPowerRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getTxPowerRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_GET_TX_POWER_CMD(include_timestamp);
		powers = null;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (getTxPowerRspEvent.data.Length == 1)
		{
			powers = new float[1];
			powers[0] = (float)((double)(getTxPowerRspEvent.data[0] - 68) / 4.0);
		}
		else if (getTxPowerRspEvent.data.Length >= 4)
		{
			powers = new float[getTxPowerRspEvent.data.Length - 3];
			powers[0] = (float)((double)(getTxPowerRspEvent.data[0] - 68) / 4.0);
			for (int i = 1; i < powers.Length; i++)
			{
				powers[i] = (float)((double)(getTxPowerRspEvent.data[3 + i] - 68) / 4.0);
			}
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN SetAntenna(byte antennas, out OPERATION_CMD.SET_ANTENNA_RESULT result)
	{
		setAntennaRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setAntennaRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_SET_ANTENNA_CMD(include_timestamp, antennas);
		result = OPERATION_CMD.SET_ANTENNA_RESULT.PORT_NOT_AVAILABLE;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		result = (OPERATION_CMD.SET_ANTENNA_RESULT)setAntennaRspEvent.data[0];
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN GetAntenna(out bool[] antenna_port_status)
	{
		antenna_port_status = new bool[4];
		getAntennaRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getAntennaRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_GET_ANTENNA_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		antenna_port_status[3] = (getAntennaRspEvent.data[0] & 8) == 8;
		antenna_port_status[2] = (getAntennaRspEvent.data[0] & 4) == 4;
		antenna_port_status[1] = (getAntennaRspEvent.data[0] & 2) == 2;
		antenna_port_status[0] = (getAntennaRspEvent.data[0] & 1) == 1;
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN GetCapacities(bool reportPower, bool reportFrequencyList, out float min_support_power, out float max_support_power, out ArrayList frequencies)
	{
		frequencies = null;
		getCapabilitiesRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getCapabilitiesRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_GET_CAPABILITIES_CMD(include_timestamp, reportPower, reportFrequencyList);
		min_support_power = 0f;
		max_support_power = 0f;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			int num2 = 0;
			while (num2 < getCapabilitiesRspEvent.data.Length)
			{
				switch (getCapabilitiesRspEvent.data[num2++])
				{
				case 1:
					min_support_power = (float)((double)(getCapabilitiesRspEvent.data[num2++] - 68) * 0.25);
					break;
				case 2:
					max_support_power = (float)((double)(getCapabilitiesRspEvent.data[num2++] - 68) * 0.25);
					break;
				case 3:
				{
					frequencies = new ArrayList();
					int num3 = getCapabilitiesRspEvent.data[num2++] << 8 + getCapabilitiesRspEvent.data[num2++];
					for (int i = 0; i < num3 / 2; i++)
					{
						frequencies.Add(getCapabilitiesRspEvent.data[num2++] << 8 + getCapabilitiesRspEvent.data[num2++]);
					}
					break;
				}
				default:
					num2++;
					break;
				}
			}
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE mode, ushort center_frequency_index, ushort[] frequency_list, out OPERATION_CMD.SET_FREQUENCY_RESULT result)
	{
		setTxFrequencyRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_SET_TX_FREQUENCY(include_timestamp, mode, center_frequency_index, frequency_list, new ushort[0]);
		ns.Write(array, 0, array.Length);
		result = OPERATION_CMD.SET_FREQUENCY_RESULT.ERROR;
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		result = (OPERATION_CMD.SET_FREQUENCY_RESULT)setTxFrequencyRspEvent.data[0];
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN SetTxFrequency(OPERATION_CMD.FREQUENCY_SET_MODE mode, ushort center_frequency_index, ushort[] frequency_list, ushort[] reduced_power_frequency_list, out OPERATION_CMD.SET_FREQUENCY_RESULT result)
	{
		setTxFrequencyRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_SET_TX_FREQUENCY(include_timestamp, mode, center_frequency_index, frequency_list, reduced_power_frequency_list);
		ns.Write(array, 0, array.Length);
		result = OPERATION_CMD.SET_FREQUENCY_RESULT.ERROR;
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		result = (OPERATION_CMD.SET_FREQUENCY_RESULT)setTxFrequencyRspEvent.data[0];
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN SetGen2Params(OPERATION_CMD.GEN2_PARAM gen2_param, out OPERATION_CMD.SET_GEN2_PARAMS_RESULT result)
	{
		setGen2ParamsRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_SET_GEN2_PARAMS_CMD(include_timestamp, gen2_param);
		result = OPERATION_CMD.SET_GEN2_PARAMS_RESULT.SESSION_AND_INVENTORY_MODE_COMBINATION_NOT_SUPPORTED;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		result = (OPERATION_CMD.SET_GEN2_PARAMS_RESULT)setGen2ParamsRspEvent.data[0];
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN GetGen2Params(out OPERATION_CMD.GEN2_PARAM gen2_param)
	{
		getGen2ParamsRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_GET_GEN2_PARAMS_CMD(include_timestamp);
		gen2_param = null;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			gen2_param = new OPERATION_CMD.GEN2_PARAM(getGen2ParamsRspEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN GetGen2Params(bool report_search_mode, out OPERATION_CMD.GEN2_PARAM gen2_param)
	{
		getGen2ParamsRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_GET_GEN2_PARAMS_CMD(report_search_mode, include_timestamp);
		gen2_param = null;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			gen2_param = new OPERATION_CMD.GEN2_PARAM(getGen2ParamsRspEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN SetRxConfig(OPERATION_CMD.SET_RX_SENSITIVITY_MODE mode, short[] sensitivities, out bool err_ocurr)
	{
		setRxConfigRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setRxConfigRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_SET_RX_CONFIG_CMD(mode, sensitivities, include_timestamp);
		err_ocurr = true;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			err_ocurr = setRxConfigRspEvent.data[0] == 1;
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN GetRxConfig(out OPERATION_CMD.SET_RX_SENSITIVITY_MODE mode, out short[] sensitivities)
	{
		getRxConfigRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getRxConfigRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_GET_RX_CONFIG_CMD(include_timestamp);
		mode = OPERATION_CMD.SET_RX_SENSITIVITY_MODE.MAXIMUM_SENSITIVITY;
		sensitivities = null;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			mode = (OPERATION_CMD.SET_RX_SENSITIVITY_MODE)getRxConfigRspEvent.data[0];
			if (getRxConfigRspEvent.data.Length > 3)
			{
				int num2 = getRxConfigRspEvent.data.Length - 4;
				sensitivities = new short[num2];
				for (int i = 0; i < num2; i++)
				{
					sensitivities[i] = (short)(-((getRxConfigRspEvent.data[4 + i] ^ 0xFF) + 1));
				}
			}
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN GetTxFrequency(out OPERATION_CMD.FREQUENCY_SET_MODE mode, out ushort center_frequency, out ushort[] frequency_list)
	{
		getTxFrequencyRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt };
		mode = OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY;
		center_frequency = 0;
		frequency_list = null;
		byte[] array = OPERATION_CMD.GENERATE_GET_TX_FREQUENCY(include_timestamp);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			mode = (OPERATION_CMD.FREQUENCY_SET_MODE)getTxFrequencyRspEvent.data[0];
			if (mode == OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY)
			{
				center_frequency = (ushort)(getTxFrequencyRspEvent.data[2] * 256 + getTxFrequencyRspEvent.data[3]);
			}
			else if (mode == OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST)
			{
				int num2 = getTxFrequencyRspEvent.data.Length - 4;
				frequency_list = new ushort[num2];
				for (int i = 0; i < num2 / 2; i++)
				{
					try
					{
						frequency_list[i] = (ushort)(getTxFrequencyRspEvent.data[4 + 2 * i] * 256 + getTxFrequencyRspEvent.data[5 + 2 * i]);
					}
					catch
					{
					}
				}
			}
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN GetTxFrequency(out OPERATION_CMD.FREQUENCY_SET_MODE mode, out ushort center_frequency, out ushort[] frequency_list, out ushort[] reducedPowerFrequency_list)
	{
		getTxFrequencyRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getTxFrequencyRspEvent.evt, invalidCommandNtfEvent.evt };
		mode = OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_REGULATORY;
		center_frequency = 0;
		frequency_list = null;
		reducedPowerFrequency_list = null;
		byte[] array = OPERATION_CMD.GENERATE_GET_TX_FREQUENCY(include_timestamp);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			mode = (OPERATION_CMD.FREQUENCY_SET_MODE)getTxFrequencyRspEvent.data[0];
			if (mode == OPERATION_CMD.FREQUENCY_SET_MODE.CENTER_FREQUENCY)
			{
				center_frequency = (ushort)(getTxFrequencyRspEvent.data[2] * 256 + getTxFrequencyRspEvent.data[3]);
			}
			else if (mode == OPERATION_CMD.FREQUENCY_SET_MODE.CHOOSE_FROM_LIST)
			{
				int num2 = getTxFrequencyRspEvent.data[2] >> 8 + getTxFrequencyRspEvent.data[3];
				frequency_list = new ushort[num2];
				for (int i = 0; i < num2 / 2; i++)
				{
					try
					{
						frequency_list[i] = (ushort)(getTxFrequencyRspEvent.data[4 + 2 * i] * 256 + getTxFrequencyRspEvent.data[5 + 2 * i]);
					}
					catch
					{
					}
				}
			}
			else if (mode == OPERATION_CMD.FREQUENCY_SET_MODE.REDUCED_POWER_FREQUENCY_LIST)
			{
				int num3 = getTxFrequencyRspEvent.data[2] >> 8 + getTxFrequencyRspEvent.data[3];
				reducedPowerFrequency_list = new ushort[num3];
				for (int j = 0; j < num3 / 2; j++)
				{
					try
					{
						reducedPowerFrequency_list[j] = (ushort)(getTxFrequencyRspEvent.data[4 + 2 * j] * 256 + getTxFrequencyRspEvent.data[5 + 2 * j]);
					}
					catch
					{
					}
				}
			}
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN GetSupportedGen2Params(bool include_mode_id, bool include_inventory_search_mode, out OPERATION_CMD.SUPPORTED_GEN2_PARAMS_RSP rsp)
	{
		getSupportedGen2ParamsRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getSupportedGen2ParamsRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_GET_SUPPORTED_GEN2_PARAMS_CMD(include_timestamp, include_mode_id, include_inventory_search_mode);
		ns.Write(array, 0, array.Length);
		rsp = null;
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new OPERATION_CMD.SUPPORTED_GEN2_PARAMS_RSP(getSupportedGen2ParamsRspEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN CheckAntenna(out OPERATION_NTF.CHECK_ANTENNA_NTF ant_ntf)
	{
		ant_ntf = null;
		checkAntennaRspEvent = new SpeedwayManualResetEvent(status: false);
		checkAntennaNtfEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { checkAntennaRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_CHECK_ANTENNA_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!checkAntennaNtfEvent.evt.WaitOne(CMD_RESPONSE_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.COMMAND_FAILED;
		}
		try
		{
			ant_ntf = new OPERATION_NTF.CHECK_ANTENNA_NTF(checkAntennaNtfEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.COMMAND_FAILED;
		}
	}

	public CMD_RETURN SetInventoryReport(bool enable, OPERATION_CMD.OPTIONAL_INVENTORY_REPORT_PARAM param, out bool err_occur)
	{
		setInventoryReportRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setInventoryReportRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_SET_INVENTORY_REPORT(include_timestamp, !enable, param);
		err_occur = true;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		err_occur = setInventoryReportRspEvent.data[0] == 1;
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN ReportInventory(out OPERATION_CMD.REPORT_INVENTORY_RESULT rst)
	{
		reportInventoryRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { reportInventoryRspEvent.evt, invalidCommandNtfEvent.evt };
		rst = OPERATION_CMD.REPORT_INVENTORY_RESULT.ERROR;
		byte[] array = OPERATION_CMD.GENERATE_REPORT_INVENTORY_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rst = (OPERATION_CMD.REPORT_INVENTORY_RESULT)reportInventoryRspEvent.data[0];
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN SetLBTParams(bool disable_auto_select, out bool err_ocurr)
	{
		setLBTParamsRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setLBTParamsRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_SET_LBT_PARAMS_CMD(include_timestamp, disable_auto_select);
		err_ocurr = true;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		err_ocurr = setLBTParamsRspEvent.data[0] == 1;
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN GetLBTParams(out int lbt_time_mode, out bool err_occur)
	{
		getLBTParamRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { getLBTParamRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_GET_LBT_PARAMS_CMD(include_timestamp);
		err_occur = true;
		lbt_time_mode = 0;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		err_occur = getLBTParamRspEvent.data[0] == 1;
		if (getLBTParamRspEvent.data.Length > 2)
		{
			lbt_time_mode = getLBTParamRspEvent.data[1];
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN ModemStop()
	{
		modemStopRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { modemStopRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_MODEM_STOP_CMD(include_timestamp);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, MODEM_STOP_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN RFSurvey(ushort low_frequency_index, ushort high_frequency_index, OPERATION_CMD.MEASUREMENT_BANDWIDTH mb, byte antenna_byte, ushort sample_count, out bool err_ocurr)
	{
		rfSurveyRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { rfSurveyRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_RF_SURVEY_CMD(include_timestamp, low_frequency_index, high_frequency_index, mb, antenna_byte, sample_count);
		err_ocurr = true;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		err_ocurr = rfSurveyRspEvent.data[0] == 1;
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN TagRead(OPERATION_CMD.MEMORY_BANK mb, ushort addr, byte read_len, uint password, out OPERATION_NTF.TAG_READ_NTF rsp)
	{
		tagReadRspEvent = new SpeedwayManualResetEvent(status: false);
		tagReadNtfEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { tagReadRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_TAG_READ_CMD(include_timestamp, mb, addr, read_len, password);
		rsp = null;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!tagReadNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new OPERATION_NTF.TAG_READ_NTF(tagReadNtfEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN TagRead(OPERATION_CMD.MEMORY_BANK mb, ushort addr, byte read_len, out OPERATION_NTF.TAG_READ_NTF rsp)
	{
		tagReadRspEvent = new SpeedwayManualResetEvent(status: false);
		tagReadNtfEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { tagReadRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_TAG_READ_CMD(include_timestamp, mb, addr, read_len);
		rsp = null;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!tagReadNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new OPERATION_NTF.TAG_READ_NTF(tagReadNtfEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN TagWrite(OPERATION_CMD.MEMORY_BANK mb, ushort addr, ushort[] data, bool disable_block_write, uint password, out OPERATION_NTF.TAG_WRITE_NTF rsp)
	{
		tagReadNtfEvent = new SpeedwayManualResetEvent(status: false);
		tagReadRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { tagReadRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_TAG_WRITE_CMD(include_timestamp, mb, addr, data, disable_block_write, password);
		rsp = null;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!tagWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new OPERATION_NTF.TAG_WRITE_NTF(tagWriteNtfEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN TagWrite(OPERATION_CMD.MEMORY_BANK mb, ushort addr, ushort[] data, bool disable_block_write, out OPERATION_NTF.TAG_WRITE_NTF rsp)
	{
		tagWriteRspEvent = new SpeedwayManualResetEvent(status: false);
		tagWriteNtfEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { tagWriteRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_TAG_WRITE_CMD(include_timestamp, mb, addr, data, disable_block_write);
		rsp = null;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!tagWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		try
		{
			rsp = new OPERATION_NTF.TAG_WRITE_NTF(tagWriteNtfEvent.data);
			return CMD_RETURN.COMMAND_SUCESS;
		}
		catch
		{
			return CMD_RETURN.RESPONSE_CAN_NOT_RECOGNIZED;
		}
	}

	public CMD_RETURN TagLock(OPERATION_CMD.TAG_LOCK_OPERATION tlo, out OPERATION_NTF.TAG_ACCESS_RESULT_CODE rsp)
	{
		tagLockNtfEvent = new SpeedwayManualResetEvent(status: false);
		tagLockRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { tagLockRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_TAG_LOCK_CMD(include_timestamp, tlo);
		rsp = OPERATION_NTF.TAG_ACCESS_RESULT_CODE.FAIL_OTHER_TAG_ERROR;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!tagLockNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		rsp = (OPERATION_NTF.TAG_ACCESS_RESULT_CODE)tagLockNtfEvent.data[0];
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN TagLock(OPERATION_CMD.TAG_LOCK_OPERATION tlo, uint password, out OPERATION_NTF.TAG_ACCESS_RESULT_CODE rsp)
	{
		tagLockNtfEvent = new SpeedwayManualResetEvent(status: false);
		tagLockRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { tagLockRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_TAG_LOCK_CMD(include_timestamp, tlo, password);
		rsp = OPERATION_NTF.TAG_ACCESS_RESULT_CODE.FAIL_OTHER_TAG_ERROR;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!tagLockNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		rsp = (OPERATION_NTF.TAG_ACCESS_RESULT_CODE)tagLockNtfEvent.data[0];
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN TagKill(uint password, out OPERATION_NTF.TAG_ACCESS_RESULT_CODE rsp)
	{
		tagKillNtfEvent = new SpeedwayManualResetEvent(status: false);
		tagKillRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { tagKillRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_TAG_KILL_CMD(include_timestamp, password);
		rsp = OPERATION_NTF.TAG_ACCESS_RESULT_CODE.FAIL_OTHER_TAG_ERROR;
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!tagKillNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		rsp = (OPERATION_NTF.TAG_ACCESS_RESULT_CODE)tagKillNtfEvent.data[0];
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN SetProfileSequence(bool enabled, ArrayList sequences, ArrayList durations, out bool err_occur)
	{
		err_occur = true;
		setProfileSequenceRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[2] { setProfileSequenceRspEvent.evt, invalidCommandNtfEvent.evt };
		byte[] array = OPERATION_CMD.GENERATE_SET_PROFILE_SEQUENCE_CMD(include_timestamp: false, enabled, sequences, durations);
		ns.Write(array, 0, array.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		err_occur = setProfileSequenceRspEvent.data[0] == 1;
		return CMD_RETURN.COMMAND_SUCESS;
	}

	public CMD_RETURN SetFixedChannel(short channel)
	{
		short[] array = new short[1];
		short[] data = array;
		testWriteRspEvent = new SpeedwayManualResetEvent(status: false);
		invalidCommandNtfEvent = new SpeedwayManualResetEvent(status: false);
		testWriteNtfEvent = new SpeedwayManualResetEvent(status: false);
		WaitHandle[] waitHandles = new WaitHandle[3] { testWriteRspEvent.evt, invalidCommandNtfEvent.evt, testWriteNtfEvent.evt };
		byte[] array2 = TEST_CMD_SET.GENERATE_WRITE_CMD_DATA(2, 8193u, data, include_timestamp);
		ns.Write(array2, 0, array2.Length);
		int num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!testWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		testWriteRspEvent.evt.Reset();
		invalidCommandNtfEvent.evt.Reset();
		testWriteNtfEvent.evt.Reset();
		data = new short[1] { channel };
		array2 = TEST_CMD_SET.GENERATE_WRITE_CMD_DATA(2, 8195u, data, include_timestamp);
		ns.Write(array2, 0, array2.Length);
		num = WaitHandle.WaitAny(waitHandles, CMD_RESPONSE_TIME_OUT, exitContext: false);
		if (num == 1)
		{
			return CMD_RETURN.INVALID_COMMAND;
		}
		if (num > 1)
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		if (!testWriteNtfEvent.evt.WaitOne(NOTIFICTION_TIME_OUT, exitContext: false))
		{
			return CMD_RETURN.CMD_RESPONSE_TIME_OUT;
		}
		return CMD_RETURN.COMMAND_SUCESS;
	}
}
