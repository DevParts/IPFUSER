namespace Advantech.Adam;

public enum Adam4022T_InputRange : byte
{
	mA_4To20 = 7,
	V_0To10 = 8,
	mA_0To20 = 13,
	Pt385_Neg100To100 = 32,
	Pt385_0To100 = 33,
	Pt385_0To200 = 34,
	Pt385_0To600 = 35,
	Pt392_Neg100To100 = 36,
	Pt392_0To100 = 37,
	Pt392_0To200 = 38,
	Pt392_0To600 = 39,
	Pt1000_Neg40To160 = 42,
	Thermistor_3K_0To100 = 48,
	Thermistor_10K_0To100 = 49,
	Unknown = byte.MaxValue
}
