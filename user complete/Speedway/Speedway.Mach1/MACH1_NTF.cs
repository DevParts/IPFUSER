using System;

namespace Speedway.Mach1;

[Serializable]
public abstract class MACH1_NTF
{
	public uint timestamp_second;

	public uint timestamp_us;

	public string reader_name;
}
