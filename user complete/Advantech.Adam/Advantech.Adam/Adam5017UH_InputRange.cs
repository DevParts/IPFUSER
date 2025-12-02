namespace Advantech.Adam;

public enum Adam5017UH_InputRange : byte
{
	mA_4To20 = 7,
	V_Neg10To10 = 8,
	mV_0To500 = 67,
	mA_0To20 = 70,
	V_0To10 = 72,
	Unknown = byte.MaxValue
}
