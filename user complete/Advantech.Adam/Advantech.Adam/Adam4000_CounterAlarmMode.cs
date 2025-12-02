namespace Advantech.Adam;

public enum Adam4000_CounterAlarmMode : byte
{
	Disable = 68,
	Latch = 76,
	Momentary = 77,
	Unknown = byte.MaxValue
}
