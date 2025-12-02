namespace Advantech.Adam;

public enum Adam4013_InputRange : byte
{
	Pt385_Neg100To100 = 32,
	Pt385_0To100 = 33,
	Pt385_0To200 = 34,
	Pt385_0To600 = 35,
	Pt392_Neg100To100 = 36,
	Pt392_0To100 = 37,
	Pt392_0To200 = 38,
	Pt392_0To600 = 39,
	Ni518_Neg80To100 = 40,
	Ni518_0To100 = 41,
	Unknown = byte.MaxValue
}
