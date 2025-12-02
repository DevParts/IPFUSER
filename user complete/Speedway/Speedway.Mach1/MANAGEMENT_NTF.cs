using System;

namespace Speedway.Mach1;

[Serializable]
public class MANAGEMENT_NTF
{
	[Serializable]
	public class SOCKET_CONNECTION_STATUS_NTF : MACH1_NTF
	{
		public SOCKET_STATUS socket_status = SOCKET_STATUS.CONNECTION_FAILED;

		public string ip;

		public SOCKET_CONNECTION_STATUS_NTF(byte[] data)
		{
			socket_status = (SOCKET_STATUS)data[0];
			if (socket_status == SOCKET_STATUS.CONNECTION_FAILED && data[1] == 1)
			{
				ip = $"{data[2]}.{data[3]}.{data[4]}.{data[5]}";
			}
		}
	}

	[Serializable]
	public class SYSTEM_ERROR_NTF : MACH1_NTF
	{
		public SYSTEM_ERROR_REASON err_reason;

		public MODEM_STATE current_state;

		public uint err_code;

		public SYSTEM_ERROR_NTF(byte[] data)
		{
			err_reason = (SYSTEM_ERROR_REASON)data[0];
			current_state = (MODEM_STATE)data[1];
			if (data.Length >= 7 && data[2] == 1)
			{
				uint num = data[3];
				uint num2 = data[4];
				uint num3 = data[5];
				uint num4 = data[6];
				err_code = (num << 24) + (num2 << 16) + (num3 << 8) + num4;
			}
		}
	}

	[Serializable]
	public class TEMPERETURE_ALARM_NTF : MACH1_NTF
	{
		public enum REASON
		{
			PERIODIC_REPORTING,
			ALARM_SHRESHOLD_REACHED,
			ONE_SHOT_REPORT
		}

		public REASON reason;

		public int temperature;

		public TEMPERETURE_ALARM_NTF(byte[] data)
		{
			try
			{
				reason = (REASON)data[0];
				temperature = data[1];
			}
			catch
			{
			}
		}
	}

	[Serializable]
	public class GPI_ALERT_NTF : MACH1_NTF
	{
		public enum GPI_TRIGGER
		{
			GPI_0_TRIGGERED,
			GPI_1_TRIGGERED,
			GPI_2_TRIGGERED,
			GPI_3_TRIGGERED
		}

		public enum GPI_STATUS
		{
			LOW,
			HIGH
		}

		public GPI_TRIGGER gpi;

		public GPI_STATUS status;

		public GPI_ALERT_NTF(byte[] data)
		{
			gpi = (GPI_TRIGGER)data[0];
			try
			{
				if (data[1] == 1)
				{
					status = (GPI_STATUS)data[2];
				}
			}
			catch
			{
			}
		}
	}

	[Serializable]
	public class BOOT_MODEM_NTF : MACH1_NTF
	{
		public enum BOOT_RESULT_CODE
		{
			BOOT_SUCESSFUL,
			BOOT_IN_PROGRESS,
			BOOT_FAIL_DUE_TO_INVALID_FIRMWARE,
			BOOT_FAIL_DUE_TO_UNKNOWN_HARDWARE
		}

		public BOOT_RESULT_CODE boot_result_code;

		public ushort percent_done;

		public BOOT_MODEM_NTF(byte[] data)
		{
			boot_result_code = (BOOT_RESULT_CODE)data[0];
			try
			{
				if (data.Length > 1 && data[1] == 1)
				{
					percent_done = data[2];
				}
			}
			catch
			{
			}
		}
	}

	[Serializable]
	public class STATUS_REPORT_NTF : MACH1_NTF
	{
		public uint modem_overflow_ntf_loss;

		public STATUS_REPORT_NTF(byte[] data)
		{
			if (data[0] == 1)
			{
				uint num = data[1];
				uint num2 = data[2];
				uint num3 = data[3];
				uint num4 = data[4];
				modem_overflow_ntf_loss = (num << 24) + (num2 << 16) + (num3 << 8) + num4;
			}
		}
	}

	public const byte SOCKET_CONNECTION_STATUS = 0;

	public const byte SYSTEM_ERROR = 1;

	public const byte BOOT_MODEM = 2;

	public const byte TEMPERATURE_ALART = 4;

	public const byte GPI_ALERT = 5;

	public const byte STATUS_REPORT = 6;
}
