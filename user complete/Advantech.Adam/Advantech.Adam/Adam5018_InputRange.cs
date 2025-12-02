namespace Advantech.Adam;

public enum Adam5018_InputRange : byte
{
	mV_Neg15To15 = 0,
	mV_Neg50To50 = 1,
	mV_Neg100To100 = 2,
	mV_Neg500To500 = 3,
	V_Neg1To1 = 4,
	V_Neg2AndHalfTo2AndHalf = 5,
	mA_Neg20To20 = 6,
	Jtype_0To760C = 14,
	Ktype_0To1370C = 15,
	Ttype_Neg100To400C = 16,
	Etype_0To1000C = 17,
	Rtype_500To1750C = 18,
	Stype_500To1750C = 19,
	Btype_500To1800C = 20,
	Unknown = byte.MaxValue
}
