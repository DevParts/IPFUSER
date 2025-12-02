namespace Advantech.Adam;

public enum Adam4018P_InputRange : byte
{
	mA_0To20 = 6,
	mA_4To20 = 7,
	Jtype_0To760C = 14,
	Ktype_0To1370C = 15,
	Ttype_Neg100To400C = 16,
	Etype_0To1000C = 17,
	Rtype_500To1750C = 18,
	Stype_500To1750C = 19,
	Btype_500To1800C = 20,
	Unknown = byte.MaxValue
}
