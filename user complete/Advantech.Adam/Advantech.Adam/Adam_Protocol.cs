namespace Advantech.Adam;

public enum Adam_Protocol : byte
{
	Advantech = 0,
	Modbus = 1,
	BACnet = 2,
	Unknown = byte.MaxValue
}
