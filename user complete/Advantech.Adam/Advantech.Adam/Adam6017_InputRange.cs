namespace Advantech.Adam;

public enum Adam6017_InputRange : byte
{
	mA_4To20 = 7,
	V_Neg10To10 = 8,
	V_Neg5To5 = 9,
	V_Neg1To1 = 10,
	mV_Neg500To500 = 11,
	mV_Neg150To150 = 12,
	mA_0To20 = 13,
	Unknown = byte.MaxValue
}
