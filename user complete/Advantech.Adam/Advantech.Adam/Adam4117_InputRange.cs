namespace Advantech.Adam;

public enum Adam4117_InputRange : byte
{
	mA_4To20 = 7,
	V_Neg10To10 = 8,
	V_Neg5To5 = 9,
	V_Neg1To1 = 10,
	mV_Neg500To500 = 11,
	mV_Neg150To150 = 12,
	mA_Neg20To20 = 13,
	V_Neg15To15 = 21,
	V_0To10 = 72,
	V_0To5 = 73,
	V_0To1 = 74,
	mV_0To500 = 75,
	mV_0To150 = 76,
	mA_0To20 = 77,
	V_0To15 = 85,
	Unknown = byte.MaxValue
}
