using System;

namespace Speedway.Mach1;

[Serializable]
public class OPERATION_NTF
{
	[Serializable]
	public class SET_REGULATORY_REGION_NTF : MACH1_NTF
	{
		public enum NTF_CODE
		{
			SUCCESS,
			NOT_VALID_REGULATORY_CALIBRATION,
			ERROR_SETTING
		}

		public NTF_CODE code;
	}

	[Serializable]
	public class MODEM_STOPPED_NTF : MACH1_NTF
	{
		public enum NTF_CODE
		{
			RESULT_OF_COMMAND,
			TO_MEET_REGULATORY,
			HARDWARE_ERROR
		}

		public NTF_CODE code;
	}

	public enum TAG_ACCESS_RESULT_CODE
	{
		SUCCEEDED,
		FAIL_NO_RESPONSE,
		FAIL_MEMORY_LOCKED,
		FAIL_MEMORY_OVERRUN,
		FAIL_INSUFFICIENT_POWER,
		FAIL_INVALID_PASSWORD,
		FAIL_OTHER_TAG_ERROR,
		FAIL_TAG_LOST,
		FAIL_READER_ERROR
	}

	public enum ANTENNA_STATUS
	{
		READY = 1,
		DISCONNECTED = 3
	}

	[Serializable]
	public class ACCUMULATION_STATUS_NTF : MACH1_NTF
	{
		public enum NTF_CODE
		{
			BUFFER_EMPTIED,
			BUFFER_FILLED
		}

		public NTF_CODE code;
	}

	[Serializable]
	public class ANTENNA_ALERT_NTF : MACH1_NTF
	{
		public OPERATION_CMD.ANTENNA antenna;

		public ANTENNA_STATUS antenna_status;

		public ANTENNA_ALERT_NTF(byte[] data)
		{
			try
			{
				antenna = (OPERATION_CMD.ANTENNA)data[0];
				antenna_status = (ANTENNA_STATUS)data[1];
			}
			catch
			{
			}
		}
	}

	[Serializable]
	public class CHECK_ANTENNA_NTF : MACH1_NTF
	{
		public ANTENNA_STATUS[] antenna_status;

		public CHECK_ANTENNA_NTF(byte[] data)
		{
			try
			{
				int num = data.Length;
				antenna_status = new ANTENNA_STATUS[num];
				for (int i = 0; i < num; i++)
				{
					antenna_status[i] = (ANTENNA_STATUS)data[i];
				}
			}
			catch
			{
			}
		}
	}

	[Serializable]
	public class INVENTORY_NTF : MACH1_NTF
	{
		public enum GEN2_READ_RESULT_CODE
		{
			READ_SUCESS,
			NO_RESPONSE,
			CRC_ERROR,
			MEMORY_LOCKED,
			MEMORY_OVERRUN,
			OTHER_TAG_ERROR,
			OTHER_READER_ERROR
		}

		public enum E_ANTENNA
		{
			ANTENNA1 = 1,
			ANTENNA2 = 2,
			ANTENNA3 = 4,
			ANTENNA4 = 8
		}

		public enum ACCUMULATION_STATUS
		{
			INVENOTRY_NTF_BYPASSED,
			INVENTORY_NTF_ADDED,
			INVENTORY_NTF_REMOVED,
			INVENTORY_NTF_OVERFLOWED
		}

		public byte[] epcb;

		public bool halted;

		public short rssi;

		public byte[] gen2PC = new byte[2];

		public byte[] epcCrc = new byte[2];

		public GEN2_READ_RESULT_CODE gen2_read_result_code;

		public byte[] gen2ReadData;

		public ushort gen2Handle;

		public E_ANTENNA antenna;

		public ushort read_count;

		public uint first_seen_ago;

		public uint last_seen_ago;

		public ACCUMULATION_STATUS accumulation_status;

		public short phaseI;

		public short phaseQ;

		public string EPC
		{
			get
			{
				string text = string.Empty;
				for (int i = 0; i < epcb.Length; i++)
				{
					text += epcb[i].ToString("X2");
				}
				return text;
			}
		}

		public INVENTORY_NTF()
		{
		}

		public INVENTORY_NTF(byte[] data)
		{
			try
			{
				int num = 1;
				epcb = new byte[data[num]];
				num++;
				Array.Copy(data, num, epcb, 0, data[1]);
				num += data[1];
				halted = data[num] == 1;
				num++;
				rssi = (short)(-((data[num] ^ 0xFF) + 1));
				num++;
				gen2PC[0] = data[num++];
				gen2PC[1] = data[num++];
				epcCrc[0] = data[num++];
				epcCrc[1] = data[num++];
				if ((epcCrc[0] & 0x80) != 0)
				{
					phaseI = (short)(-((epcCrc[0] ^ 0xFF) + 1));
				}
				else
				{
					phaseI = epcCrc[0];
				}
				if ((epcCrc[1] & 0x80) != 0)
				{
					phaseQ = (short)(-((epcCrc[1] ^ 0xFF) + 1));
				}
				else
				{
					phaseQ = epcCrc[1];
				}
				while (num < data.Length)
				{
					switch (data[num++])
					{
					case 1:
						gen2_read_result_code = (GEN2_READ_RESULT_CODE)data[num++];
						break;
					case 2:
					{
						int num2 = data[num++];
						gen2ReadData = new byte[num2];
						Array.Copy(data, num, gen2ReadData, 0, num2);
						num += num2;
						break;
					}
					case 3:
						gen2Handle = (ushort)(data[num++] * 256 + data[num++]);
						break;
					case 4:
						antenna = (E_ANTENNA)data[num++];
						break;
					case 5:
						read_count = (ushort)(data[num++] * 256 + data[num++]);
						break;
					case 6:
						first_seen_ago = (uint)((data[num++] << 24) + (data[num++] << 16) + (data[num++] << 8) + data[num++]);
						break;
					case 7:
						last_seen_ago = (uint)((data[num++] << 24) + (data[num++] << 16) + (data[num++] << 8) + data[num++]);
						break;
					case 8:
						accumulation_status = (ACCUMULATION_STATUS)data[num++];
						break;
					default:
						num++;
						break;
					}
				}
			}
			catch
			{
			}
		}
	}

	[Serializable]
	public class INVENTORY_STATUS_NTF : MACH1_NTF
	{
		public bool transmitter_status_enabled;

		public byte enabled_antenna_byte;

		public ushort center_frequncy_index;

		public ushort inventory_attempt_count;

		public INVENTORY_STATUS_NTF(byte[] data)
		{
			transmitter_status_enabled = data[0] == 1;
			try
			{
				int num = 1;
				while (num < data.Length - 1)
				{
					switch (data[num])
					{
					case 1:
						num++;
						enabled_antenna_byte = data[num];
						num++;
						break;
					case 2:
						num++;
						center_frequncy_index = (ushort)(data[num] * 256 + data[num + 1]);
						num += 2;
						break;
					case 3:
						num++;
						inventory_attempt_count = (ushort)(data[num] * 256 + data[num + 1]);
						num += 2;
						break;
					default:
						num++;
						break;
					}
				}
			}
			catch
			{
			}
		}
	}

	[Serializable]
	public class RF_SURVEY_NTF : MACH1_NTF
	{
		public enum RESULT_CODE
		{
			SURVEY_IN_PROGRESS,
			SUCCESS,
			ABORT
		}

		public RESULT_CODE result_code;

		public ushort low_frequency_index;

		public ushort high_frequency_index;

		public byte bandwidth_index;

		public byte antenna_byte;

		public uint low_time;

		public uint high_time;

		public short[] rssi_data;

		public RF_SURVEY_NTF(byte[] data)
		{
			try
			{
				result_code = (RESULT_CODE)data[0];
				low_frequency_index = (ushort)(data[1] * 256 + data[2]);
				high_frequency_index = (ushort)(data[3] * 256 + data[4]);
				bandwidth_index = data[5];
				antenna_byte = data[6];
				low_time = (uint)((data[7] << 24) & (data[8] << 16) & (data[9] << 8) & data[10]);
				high_time = (uint)((data[11] << 24) & (data[12] << 16) & (data[13] << 8) & data[14]);
				if (data.Length > 17 && data[15] == 1)
				{
					int num = (data[16] << 8) + data[17];
					rssi_data = new short[num];
					for (int i = 0; i < num; i++)
					{
						rssi_data[i] = (short)(-((data[18 + i] ^ 0xFF) + 1));
					}
				}
			}
			catch
			{
			}
		}
	}

	[Serializable]
	public class TAG_READ_NTF : MACH1_NTF
	{
		public enum RESULT_CODE
		{
			READ_SUCCEEDED,
			NO_RESPONSE,
			CRC_ERROR,
			MEMORY_LOCKED,
			MEMORY_OVERRUN,
			INVALID_PASSWORD,
			OTHER_TAG_ERROR,
			TAG_LOST,
			READER_ERROR
		}

		public RESULT_CODE result_code;

		public byte[] data;

		public TAG_READ_NTF(byte[] data)
		{
			try
			{
				result_code = (RESULT_CODE)data[0];
				if (data.Length > 4)
				{
					int num = data[2] * 256 + data[3];
					this.data = new byte[num];
					Array.Copy(data, 4, this.data, 0, num);
				}
			}
			catch
			{
			}
		}
	}

	[Serializable]
	public class TAG_WRITE_NTF : MACH1_NTF
	{
		public TAG_ACCESS_RESULT_CODE result_code;

		public ushort err_addr;

		public TAG_WRITE_NTF(byte[] data)
		{
			try
			{
				result_code = (TAG_ACCESS_RESULT_CODE)data[0];
				if (data.Length > 4)
				{
					err_addr = (ushort)(data[2] * 256 + data[3]);
				}
			}
			catch
			{
			}
		}
	}

	public const byte ANTENNA_ALERT = 0;

	public const byte SET_REGULATORY_REGION = 10;

	public const byte INVENTORY = 1;

	public const byte TAG_CUSTOM = 9;

	public const byte TAG_KILL = 8;

	public const byte TAG_LOCK = 7;

	public const byte TAG_WRITE = 6;

	public const byte TAG_READ = 5;

	public const byte RF_SURVEY = 3;

	public const byte MODEM_STOP = 4;

	public const byte INVENTORY_STATUS = 2;

	public const byte CHECK_ANTENNA = 11;

	public const byte ACCUMULATION_STATUS = 12;
}
