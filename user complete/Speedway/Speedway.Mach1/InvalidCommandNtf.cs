using System;

namespace Speedway.Mach1;

[Serializable]
public class InvalidCommandNtf
{
	public enum REASON_CODE
	{
		INVALID_CATEGORY,
		INVALID_CRC,
		INVALID_MID,
		OTHER_INVALID_HEADER,
		INVALID_MODEM_STATE,
		PARAM_OUT_OF_RANGE,
		MISSING_COMMAND_PARAM,
		INVALID_OPTIONAL_PARAM,
		OTHER_PARAM_ERR,
		OUT_OF_SYNC,
		INVALID_RFU_BITS_IN_LENGTH,
		COMMAND_IN_PROGRESS
	}

	public REASON_CODE reason_code;

	public MODEM_STATE state;

	private ushort received_header;

	private ushort received_length;

	public InvalidCommandNtf(byte[] data)
	{
		if (data.Length != 6)
		{
			throw new Exception("Not a valid packet!");
		}
		reason_code = (REASON_CODE)data[0];
		state = (MODEM_STATE)data[1];
		received_header = (ushort)((data[2] << 8) + data[3]);
		received_length = (ushort)((data[4] << 8) + data[5]);
	}
}
