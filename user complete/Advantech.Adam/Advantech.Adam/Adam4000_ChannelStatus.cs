namespace Advantech.Adam;

public enum Adam4000_ChannelStatus : byte
{
	Normal = 0,
	Over = 1,
	Under = 2,
	Burn = 3,
	Disable = byte.MaxValue
}
