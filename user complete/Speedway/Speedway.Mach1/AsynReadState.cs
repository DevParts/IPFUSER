using System;

namespace Speedway.Mach1;

[Serializable]
public class AsynReadState
{
	public byte[] data;

	public AsynReadState(int buffer_size)
	{
		data = new byte[buffer_size];
	}
}
