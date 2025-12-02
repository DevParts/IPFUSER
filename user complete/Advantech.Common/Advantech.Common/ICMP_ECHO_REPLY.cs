using System;

namespace Advantech.Common;

public class ICMP_ECHO_REPLY
{
	private byte[] data;

	public byte[] _Data => data;

	public int Address => BitConverter.ToInt32(data, 0);

	public int Status => BitConverter.ToInt32(data, 4);

	public int RoundTripTime => BitConverter.ToInt32(data, 8);

	public short DataSize
	{
		get
		{
			return BitConverter.ToInt16(data, 12);
		}
		set
		{
			BitConverter.GetBytes(value).CopyTo(data, 12);
		}
	}

	public IntPtr Data
	{
		get
		{
			return new IntPtr(BitConverter.ToInt32(data, 16));
		}
		set
		{
			BitConverter.GetBytes(value.ToInt32()).CopyTo(data, 16);
		}
	}

	public byte Ttl => data[20];

	public byte Tos => data[21];

	public byte Flags => data[22];

	public byte OptionsSize => data[23];

	public IntPtr OptionsData
	{
		get
		{
			return new IntPtr(BitConverter.ToInt32(data, 24));
		}
		set
		{
			BitConverter.GetBytes(value.ToInt32()).CopyTo(data, 24);
		}
	}

	public ICMP_ECHO_REPLY(int size)
	{
		data = new byte[size];
	}
}
