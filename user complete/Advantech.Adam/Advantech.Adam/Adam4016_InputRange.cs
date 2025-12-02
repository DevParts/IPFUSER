namespace Advantech.Adam;

public enum Adam4016_InputRange : byte
{
	mV_Neg15To15 = 0,
	mV_Neg50To50 = 1,
	mV_Neg100To100 = 2,
	mV_Neg500To500 = 3,
	mA_Neg20To20 = 6,
	Unknown = byte.MaxValue
}
