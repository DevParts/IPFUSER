namespace Advantech.Adam;

public enum Adam4015_InputRange : byte
{
	Pt385_Neg50To150 = 32,
	Pt385_0To100 = 33,
	Pt385_0To200 = 34,
	Pt385_0To400 = 35,
	Pt385_Neg200To200 = 36,
	Pt392_Neg50To150 = 37,
	Pt392_0To100 = 38,
	Pt392_0To200 = 39,
	Pt392_0To400 = 40,
	Pt392_Neg200To200 = 41,
	Pt1000_Neg40To160 = 42,
	Balcon500_Neg30To120 = 43,
	Ni518_Neg80To100 = 44,
	Ni518_0To100 = 45,
	Ni508_0To100 = 50,
	Ni508_Neg50To200 = 51,
	BA1_Neg200To600 = 52,
	Unknown = byte.MaxValue
}
