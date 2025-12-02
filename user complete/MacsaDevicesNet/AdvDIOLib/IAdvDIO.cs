using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AdvDIOLib;

[ComImport]
[CompilerGenerated]
[Guid("EE761BD2-60A1-4CB9-B6BE-228F8B65CF6F")]
[TypeIdentifier]
public interface IAdvDIO
{
	[DispId(1)]
	int deviceNumber
	{
		[DispId(1)]
		get;
		[DispId(1)]
		[param: In]
		set;
	}

	void _VtblGap1_15();

	[DispId(14)]
	bool ReadDiPorts([MarshalAs(UnmanagedType.Struct)] out object pBuffer, int portStart, int portCount = 1);

	[DispId(15)]
	bool ReadDoPorts([MarshalAs(UnmanagedType.Struct)] out object pBuffer, int portStart, int portCount = 1);

	[DispId(16)]
	bool WriteDoChannel(int status, int channel);
}
