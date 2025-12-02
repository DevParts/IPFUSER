namespace Advantech.Adam;

public enum Adam_EventType : byte
{
	Dio_OffToOn = 0,
	Dio_OnToOff = 1,
	Alarm_OffToOn = 2,
	Alarm_OnToOff = 3,
	Unknown = byte.MaxValue
}
