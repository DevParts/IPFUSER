namespace Advantech.Adam;

public enum Adam_SlewRate : byte
{
	Immediate = 0,
	V_p0625_mA_p125 = 1,
	V_p125_mA_p250 = 2,
	V_p250_mA_p5 = 3,
	V_p5_mA_1 = 4,
	V_1_mA_2 = 5,
	V_2_mA_4 = 6,
	V_4_mA_8 = 7,
	V_8_mA_16 = 8,
	V_16_mA_32 = 9,
	V_32_mA_64 = 10,
	V_64_mA_128 = 11,
	Unknown = byte.MaxValue
}
