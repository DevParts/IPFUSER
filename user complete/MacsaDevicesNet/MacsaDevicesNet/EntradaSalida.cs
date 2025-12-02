using System.Diagnostics;

namespace MacsaDevicesNet;

public abstract class EntradaSalida
{
	[DebuggerNonUserCode]
	protected EntradaSalida()
	{
	}

	public abstract bool WriteDO(int iDO, bool bState);

	public abstract void DoPulseOutput(int Output, long Time);
}
