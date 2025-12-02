namespace MacsaDevicesNet;

public interface IDeviceIntegra
{
	public delegate void OnLineEventHandler(object Sender);

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	event OnLineEventHandler OnLine;

	event OnErrorEventHandler OnError;

	bool GetStatus(Common.WAIT_TYPE tWaitType = Common.WAIT_TYPE.Thread, long lTimeToWait = 1000L);
}
