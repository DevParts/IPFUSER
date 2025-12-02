using System;

namespace Speedway.Mach1;

[Serializable]
public class MACH1_FRAME
{
	[Serializable]
	public abstract class HEADER
	{
		public const bool bit_15 = false;

		public bool timestamp_included;

		public bool is_ntf;

		public bool bit_12;

		public CATEGORY category;

		public byte message_id;

		public byte[] ToByteArray()
		{
			int num = 0;
			int num2 = (timestamp_included ? 1 : 0);
			int num3 = (is_ntf ? 1 : 0);
			num2 <<= 6;
			num3 <<= 5;
			int num4 = (int)category;
			num = num2 | num3 | num4;
			return new byte[2]
			{
				(byte)num,
				message_id
			};
		}
	}

	[Serializable]
	public class HEADER_READER_HOST : HEADER
	{
		public HEADER_READER_HOST(byte[] data)
		{
			if (data.Length != 2)
			{
				throw new Exception("The input is not a valid Header");
			}
			timestamp_included = (((data[0] & 0x40) != 0) ? true : false);
			is_ntf = (((data[0] & 0x20) != 0) ? true : false);
			category = (CATEGORY)(data[0] & 0xF);
			message_id = data[1];
		}
	}

	[Serializable]
	public class HEADER_HOST_READER : HEADER
	{
		public HEADER_HOST_READER(bool include_timestamp, CATEGORY category, byte message_id)
		{
			timestamp_included = include_timestamp;
			base.category = category;
			base.message_id = message_id;
			is_ntf = false;
		}
	}

	public HEADER header;

	public ushort packet_len = 6;

	public ushort payload_len;

	public uint timestamp_second;

	public uint timestamp_us;

	private byte[] packet;

	private byte[] payload;

	public byte[] PACKET
	{
		get
		{
			if (packet == null)
			{
				return null;
			}
			return packet;
		}
	}

	public byte[] PAYLOAD
	{
		get
		{
			if (payload != null)
			{
				return payload;
			}
			return null;
		}
	}

	private void CreateFrameWithoutPayload(CATEGORY category, byte cmd, bool include_timestamp)
	{
		try
		{
			header = new HEADER_HOST_READER(include_timestamp, category, cmd);
			long num = DateTime.Now.ToUniversalTime().ToFileTimeUtc();
			timestamp_second = (uint)(num >> 32);
			timestamp_us = (uint)(num & 0xFFFFFFFFu);
			if (include_timestamp)
			{
				packet_len += 8;
			}
			packet = new byte[packet_len];
			packet[0] = 238;
			Array.Copy(header.ToByteArray(), 0, packet, 1, 2);
			packet[4] = 0;
			packet[3] = 0;
			if (include_timestamp)
			{
				packet[5] = (byte)(timestamp_second >> 24);
				packet[6] = (byte)((timestamp_second & 0xFF0000) >> 16);
				packet[7] = (byte)((timestamp_second & 0xFF00) >> 8);
				packet[8] = (byte)(timestamp_second & 0xFF);
				packet[9] = (byte)(timestamp_us >> 24);
				packet[10] = (byte)((timestamp_us & 0xFF0000) >> 16);
				packet[11] = (byte)((timestamp_us & 0xFF00) >> 8);
				packet[12] = (byte)(timestamp_us & 0xFF);
			}
			packet[packet_len - 1] = CRC.CalculateCRC(packet, packet_len - 1);
		}
		catch
		{
		}
	}

	private void CreateFrameWithPayload(CATEGORY category, byte cmd, bool include_timestamp, byte[] data)
	{
		try
		{
			header = new HEADER_HOST_READER(include_timestamp, category, cmd);
			payload = new byte[data.Length];
			Array.Copy(data, payload, data.Length);
			long num = DateTime.Now.ToUniversalTime().ToFileTimeUtc();
			timestamp_second = (uint)(num >> 32);
			timestamp_us = (uint)(num & 0xFFFFFFFFu);
			if (include_timestamp)
			{
				packet_len += 8;
			}
			payload_len = (ushort)data.Length;
			packet_len += payload_len;
			packet = new byte[packet_len];
			packet[0] = 238;
			Array.Copy(header.ToByteArray(), 0, packet, 1, 2);
			packet[4] = (byte)(payload_len & 0xFF);
			packet[3] = (byte)(payload_len >> 8);
			if (include_timestamp)
			{
				packet[5] = (byte)(timestamp_second >> 24);
				packet[6] = (byte)((long)((ulong)timestamp_second & 0xFFFF00000000uL) >> 16);
				packet[7] = (byte)((timestamp_second & 0xFFFF0000u) >> 8);
				packet[8] = (byte)(timestamp_second & 0xFFFF);
				packet[9] = (byte)(timestamp_us >> 24);
				packet[10] = (byte)((long)((ulong)timestamp_us & 0xFFFF00000000uL) >> 16);
				packet[11] = (byte)((timestamp_us & 0xFFFF0000u) >> 8);
				packet[12] = (byte)(timestamp_us & 0xFFFF);
				Array.Copy(data, 0, packet, 13, data.Length);
			}
			else
			{
				Array.Copy(data, 0, packet, 5, data.Length);
			}
			packet[packet_len - 1] = CRC.CalculateCRC(packet, packet_len - 1);
		}
		catch
		{
		}
	}

	public MACH1_FRAME()
	{
	}

	public MACH1_FRAME(CATEGORY category, byte cmd, bool include_timestamp)
	{
		CreateFrameWithoutPayload(category, cmd, include_timestamp);
	}

	public MACH1_FRAME(CATEGORY category, byte cmd, bool include_timestamp, byte[] data)
	{
		if (data != null)
		{
			CreateFrameWithPayload(category, cmd, include_timestamp, data);
		}
		else
		{
			CreateFrameWithoutPayload(category, cmd, include_timestamp);
		}
	}

	public MACH1_FRAME(byte[] data)
	{
		if (data[0] != 238 && data[0] != 239)
		{
			throw new Exception("Input is not a valid Mach1 packet!");
		}
		try
		{
			byte[] array = new byte[2];
			Array.Copy(data, 1, array, 0, 2);
			header = new HEADER_READER_HOST(array);
			payload_len = (ushort)((data[3] << 8) + data[4]);
			if (header.timestamp_included)
			{
				packet_len += 8;
			}
			packet_len += payload_len;
			packet = new byte[packet_len];
			Array.Copy(data, packet, packet_len);
			byte b = CRC.CalculateCRC(packet, packet.Length - 1);
			if (b != packet[packet.Length - 1])
			{
				throw new Exception("Validate CRC failed!");
			}
			if (header.timestamp_included)
			{
				uint num = packet[5];
				uint num2 = packet[6];
				uint num3 = packet[7];
				uint num4 = packet[8];
				timestamp_second = (num << 24) + (num2 << 16) + (num3 << 8) + num4;
				num = packet[9];
				num2 = packet[10];
				num3 = packet[11];
				num4 = packet[12];
				timestamp_us = (num << 24) + (num2 << 16) + (num3 << 8) + num4;
				if (payload_len > 0)
				{
					payload = new byte[payload_len];
					Array.Copy(packet, 13, payload, 0, payload_len);
				}
			}
			else if (payload_len > 0)
			{
				payload = new byte[payload_len];
				Array.Copy(packet, 5, payload, 0, payload_len);
			}
		}
		catch
		{
			throw new Exception("Input is not a valid Mach1 packet!");
		}
	}

	public static MACH1_FRAME ParseMachData(byte[] data, out byte[] reserved_data)
	{
		MACH1_FRAME mACH1_FRAME = new MACH1_FRAME();
		if (data[0] != 238 && data[0] != 239)
		{
			throw new Exception("Input is not a valid Mach1 packet!");
		}
		reserved_data = null;
		try
		{
			if (data.Length < 5)
			{
				reserved_data = new byte[data.Length];
				Array.Copy(data, reserved_data, data.Length);
				return null;
			}
			byte[] array = new byte[2];
			Array.Copy(data, 1, array, 0, 2);
			mACH1_FRAME.header = new HEADER_READER_HOST(array);
			mACH1_FRAME.payload_len = (ushort)(((data[3] & 3) << 8) + data[4]);
			if (mACH1_FRAME.header.timestamp_included)
			{
				mACH1_FRAME.packet_len += 8;
			}
			mACH1_FRAME.packet_len += mACH1_FRAME.payload_len;
			if (data.Length < mACH1_FRAME.packet_len)
			{
				reserved_data = new byte[data.Length];
				Array.Copy(data, reserved_data, data.Length);
				return null;
			}
			mACH1_FRAME.packet = new byte[mACH1_FRAME.packet_len];
			Array.Copy(data, mACH1_FRAME.packet, mACH1_FRAME.packet_len);
			byte b = CRC.CalculateCRC(mACH1_FRAME.packet, mACH1_FRAME.packet.Length - 1);
			if (b != mACH1_FRAME.packet[mACH1_FRAME.packet.Length - 1])
			{
				return null;
			}
			if (mACH1_FRAME.header.timestamp_included)
			{
				uint num = data[5];
				uint num2 = data[6];
				uint num3 = data[7];
				uint num4 = data[8];
				mACH1_FRAME.timestamp_second = (num << 24) + (num2 << 16) + (num3 << 8) + num4;
				num = data[9];
				num2 = data[10];
				num3 = data[11];
				num4 = data[12];
				mACH1_FRAME.timestamp_us = (num << 24) + (num2 << 16) + (num3 << 8) + num4;
				if (mACH1_FRAME.payload_len > 0)
				{
					mACH1_FRAME.payload = new byte[mACH1_FRAME.payload_len];
					Array.Copy(data, 13, mACH1_FRAME.payload, 0, mACH1_FRAME.payload_len);
				}
			}
			else if (mACH1_FRAME.payload_len > 0)
			{
				mACH1_FRAME.payload = new byte[mACH1_FRAME.payload_len];
				Array.Copy(data, 5, mACH1_FRAME.payload, 0, mACH1_FRAME.payload_len);
			}
			return mACH1_FRAME;
		}
		catch
		{
			throw new Exception("Input is not a valid Mach1 packet!");
		}
	}
}
