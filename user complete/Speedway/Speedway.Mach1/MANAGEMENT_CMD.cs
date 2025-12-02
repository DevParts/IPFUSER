using System;

namespace Speedway.Mach1;

[Serializable]
public class MANAGEMENT_CMD
{
	public enum TERMPERETURE_ALARM_MODE
	{
		OFF,
		PERIODIC,
		THRESHOLD,
		ONE_SHOT
	}

	public enum MESSAGE_FLUSH_BEHAVIOR
	{
		DEFAULT,
		FLUSH_ALL_MSG
	}

	[Serializable]
	public class GPI_STATUS
	{
		public enum STATUS
		{
			LOW,
			HIGH
		}

		public byte id;

		public STATUS status;

		public GPI_STATUS()
		{
			id = 0;
			status = STATUS.LOW;
		}
	}

	[Serializable]
	public class GPI_CONFIG
	{
		public enum CONFIG
		{
			NO_NOTFICATION,
			LO_TO_HIGH,
			HI_TO_LOW,
			BOTH
		}

		public enum GPI_ID
		{
			GPI0 = 1,
			GPI1,
			GPI2,
			GPI3
		}

		public GPI_ID id = GPI_ID.GPI0;

		public CONFIG config;

		public GPI_CONFIG()
		{
			id = GPI_ID.GPI0;
			config = CONFIG.NO_NOTFICATION;
		}
	}

	[Serializable]
	public class GPO_CONFIG
	{
		public enum CONFIG
		{
			LOW,
			HIGH
		}

		public enum GPO_ID
		{
			GPO0 = 1,
			GPO1,
			GPO2,
			GPO3,
			GPO4,
			GPO5,
			GPO6,
			GPO7
		}

		public GPO_ID id = GPO_ID.GPO0;

		public CONFIG configuration;

		public GPO_CONFIG()
		{
			id = GPO_ID.GPO0;
			configuration = CONFIG.LOW;
		}

		public GPO_CONFIG(GPO_ID id, CONFIG conf)
		{
			this.id = id;
			configuration = conf;
		}
	}

	public class MCS_VERSION_RSP
	{
		public string version = string.Empty;

		public MCS_VERSION_RSP(byte[] data)
		{
			try
			{
				version = $"v.{data[0]}.{data[1]}.{data[2]}";
			}
			catch
			{
			}
		}
	}

	public class READER_INFO_RSP
	{
		public string software_version;

		public string firmware_verison;

		public string FPGA_version;

		public uint uptime;

		public READER_INFO_RSP(byte[] data)
		{
			try
			{
				software_version = $"v.{data[0]}.{data[1]}.{data[2]}.{data[3]}";
				firmware_verison = $"v.{data[4]}.{data[5]}.{data[6]}.{data[7]}";
				FPGA_version = $"v.{data[8]}.{data[9]}.{data[10]}.{data[11]}";
				uint num = data[12];
				uint num2 = data[13];
				uint num3 = data[14];
				uint num4 = data[15];
				uptime = (num << 24) + (num2 << 16) + (num3 << 8) + num4;
			}
			catch
			{
			}
		}
	}

	public class STATE_RSP
	{
		public MODEM_STATE state;

		public STATE_RSP(byte[] data)
		{
			try
			{
				state = (MODEM_STATE)data[0];
			}
			catch
			{
			}
		}
	}

	public class TEMPERETURE_ALARM_RSP
	{
		public short temperature;

		public TERMPERETURE_ALARM_MODE mode;

		public short periodic_tempreture_rate;

		public short alarm_threshold;

		public TEMPERETURE_ALARM_RSP(byte[] data)
		{
			try
			{
				temperature = data[0];
				mode = (TERMPERETURE_ALARM_MODE)data[1];
				switch (data[2])
				{
				case 1:
					periodic_tempreture_rate = data[3];
					if (data.Length >= 6 && data[4] == 1)
					{
						alarm_threshold = data[5];
					}
					break;
				case 2:
					alarm_threshold = data[3];
					if (data.Length >= 6 && data[4] == 2)
					{
						periodic_tempreture_rate = data[5];
					}
					break;
				}
			}
			catch
			{
			}
		}
	}

	public class SET_GPO_RSP
	{
		public enum RESULT_CODE
		{
			GPO_SET_SUCCESSFUL,
			ONE_OR_MORE_GPOS_SPECIFIED_NOT_SUPPORTED
		}

		public RESULT_CODE result_code;

		public SET_GPO_RSP(byte[] data)
		{
			try
			{
				result_code = (RESULT_CODE)data[0];
			}
			catch
			{
			}
		}
	}

	public class SET_GPI_RSP
	{
		public enum RESULT_CODE
		{
			GPI_SET_SUCCESSFUL,
			ONE_OR_MORE_GPIS_SPECIFIED_NOT_SUPPORTED,
			ATTEMPT_TO_CONFIGURE_ACTIVE_TRIGGER
		}

		public RESULT_CODE result_code;

		public SET_GPI_RSP(byte[] data)
		{
			try
			{
				result_code = (RESULT_CODE)data[0];
			}
			catch
			{
			}
		}
	}

	public class GET_GPI_RSP
	{
		public GPI_STATUS[] gpi_status;

		public GPI_CONFIG[] gpi_config;

		public GET_GPI_RSP(byte[] data)
		{
			gpi_status = new GPI_STATUS[4];
			gpi_config = new GPI_CONFIG[4];
			try
			{
				for (int i = 0; i < 4; i++)
				{
					gpi_config[i] = new GPI_CONFIG();
					gpi_config[i].id = (GPI_CONFIG.GPI_ID)(i + 1);
					gpi_config[i].config = (GPI_CONFIG.CONFIG)data[i];
					gpi_status[i] = new GPI_STATUS();
					gpi_status[i].id = (byte)(i + 1);
					gpi_status[i].status = (GPI_STATUS.STATUS)data[i + 4];
				}
			}
			catch
			{
			}
		}
	}

	public const byte GET_MCS_VERSION = 0;

	public const byte GET_READER_INFO = 1;

	public const byte GET_STATE = 2;

	public const byte BOOT_MODEM = 4;

	public const byte SHUTDOWN_MODEM = 5;

	public const byte SET_TEMPERATURE_ALART = 7;

	public const byte GET_TEMPERATURE_ALART = 8;

	public const byte SET_GPO = 9;

	public const byte SET_GPI = 10;

	public const byte GET_GPI = 11;

	public const byte SET_STATUS_REPORT = 12;

	public const byte SET_TCP_CONNECTION_OPTIONS = 13;

	public const byte GET_TCP_CONNECTION_OPTIONS = 14;

	public static byte[] GENERATE_GET_MCS_VERSION_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 0, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_READER_INFO_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 1, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_STATE_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 2, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_BOOT_MODEM_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 4, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SHUTDOWN_MODEM_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 5, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_TEMPERETURE_ALARM_CMD(bool include_timestamp, TERMPERETURE_ALARM_MODE mode, ushort periodicTemperetureRate, short alertThreshold)
	{
		byte[] data = new byte[1];
		switch (mode)
		{
		case TERMPERETURE_ALARM_MODE.OFF:
		case TERMPERETURE_ALARM_MODE.ONE_SHOT:
			data = new byte[1] { (byte)mode };
			break;
		case TERMPERETURE_ALARM_MODE.PERIODIC:
			data = new byte[3]
			{
				(byte)mode,
				1,
				(byte)periodicTemperetureRate
			};
			break;
		case TERMPERETURE_ALARM_MODE.THRESHOLD:
			data = new byte[3]
			{
				(byte)mode,
				2,
				(byte)alertThreshold
			};
			break;
		}
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 7, include_timestamp, data);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_TEMPERETURE_ALARM_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 8, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_GPO_CMD(bool include_timestamp, GPO_CONFIG[] gpo)
	{
		int num = gpo.Length;
		byte[] array = new byte[num * 2];
		for (int i = 0; i < num; i++)
		{
			array[2 * i] = (byte)gpo[i].id;
			array[2 * i + 1] = (byte)gpo[i].configuration;
		}
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 9, include_timestamp, array);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_GPI_CMD(bool include_timestamp, GPI_CONFIG[] gpi)
	{
		int num = gpi.Length;
		byte[] array = new byte[num * 2];
		for (int i = 0; i < num; i++)
		{
			array[2 * i] = (byte)gpi[i].id;
			array[2 * i + 1] = (byte)gpi[i].config;
		}
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 10, include_timestamp, array);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_GPI_CMD(bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 11, include_timestamp);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_STATUS_REPORT_CMD(bool include_timestamp, bool enable_reporting)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 12, include_timestamp, new byte[2]
		{
			1,
			(byte)(enable_reporting ? 1u : 0u)
		});
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_SET_TCP_CONNECTION_OPTIONS_CMD(bool include_timestamp, MESSAGE_FLUSH_BEHAVIOR behavior)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 13, include_timestamp, new byte[2]
		{
			1,
			(byte)behavior
		});
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_GET_TCP_CONNECTION_OPTIONS_CMD(bool include_timestamp, bool report_behavior)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.MODEM_MANAGEMENT, 14, include_timestamp, new byte[2]
		{
			1,
			(byte)(report_behavior ? 1u : 0u)
		});
		return mACH1_FRAME.PACKET;
	}
}
