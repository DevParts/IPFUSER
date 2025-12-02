using System.Diagnostics;

namespace MacsaDevicesNet;

public abstract class Inyector : MacsaDevice
{
	protected bool _IsOnLine;

	public bool IsOnLine => _IsOnLine;

	public bool IsDataSendRequested
	{
		[DebuggerNonUserCode]
		get;
		[DebuggerNonUserCode]
		set;
	}

	protected Inyector()
	{
		_IsOnLine = false;
	}

	public abstract bool GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L);

	protected virtual Common.DATA_ERROR GetDataError(int sErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		return result;
	}
}
