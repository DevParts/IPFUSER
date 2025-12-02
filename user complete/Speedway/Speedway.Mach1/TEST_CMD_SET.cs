namespace Speedway.Mach1;

public class TEST_CMD_SET
{
	public const byte GET_TCS_VERSION = 0;

	public const byte GET_VIRTUAL_PAGE_VERSION = 1;

	public const byte TEST_READ = 2;

	public const byte TEST_WRITE = 3;

	public static byte[] GENERATE_WRITE_CMD_DATA(byte memory_space, uint addr, short[] data, bool include_timestamp)
	{
		int num = 8 + data.Length;
		byte[] array = new byte[num];
		array[0] = memory_space;
		array[1] = (byte)(addr >> 24);
		array[2] = (byte)(addr >> 16);
		array[3] = (byte)(addr >> 8);
		array[4] = (byte)(addr & 0xFF);
		array[5] = (byte)(((data.Length * 2) & 0xFF00) >> 8);
		array[6] = (byte)((data.Length * 2) & 0xFF);
		for (int i = 0; i < data.Length; i++)
		{
			array[7 + 2 * i] = (byte)(data[i] >> 8);
			array[8 + 2 * i] = (byte)(data[i] & 0xFF);
		}
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.TEST, 3, include_timestamp, array);
		return mACH1_FRAME.PACKET;
	}

	public static byte[] GENERATE_READ_CMD_DATA(byte memory_space, uint addr, ushort length, bool include_timestamp)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME(CATEGORY.TEST, 3, include_timestamp, new byte[6]
		{
			memory_space,
			(byte)(addr >> 24),
			(byte)(addr >> 16),
			(byte)(addr >> 8),
			(byte)(addr & 0xFF),
			(byte)length
		});
		return mACH1_FRAME.PACKET;
	}
}
