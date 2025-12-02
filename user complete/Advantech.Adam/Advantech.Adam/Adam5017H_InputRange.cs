namespace Advantech.Adam;

public enum Adam5017H_InputRange : byte
{
	V_Neg10To10 = 0,
	V_0To10 = 1,
	V_Neg5To5 = 2,
	V_0To5 = 3,
	V_Neg2AndHalfTo2AndHalf = 4,
	V_0To2AndHalf = 5,
	V_Neg1To1 = 6,
	V_0To1 = 7,
	mV_Neg500To500 = 8,
	mV_0To500 = 9,
	mA_4To20 = 10,
	mA_0To20 = 11,
	Unknown = byte.MaxValue
}
