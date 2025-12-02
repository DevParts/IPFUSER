namespace Advantech.Adam;

public enum Adam6022_OutputRange : byte
{
	mA_0To20 = 0,
	mA_4To20 = 1,
	V_0To10 = 2,
	Unknown = byte.MaxValue
}
