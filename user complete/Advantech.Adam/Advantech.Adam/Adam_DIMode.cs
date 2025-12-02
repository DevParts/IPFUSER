namespace Advantech.Adam;

public enum Adam_DIMode : byte
{
	Di = 0,
	Counter = 1,
	LowToHighLatch = 2,
	HighToLowLatch = 3,
	Frequency = 4,
	Unknown = 16
}
