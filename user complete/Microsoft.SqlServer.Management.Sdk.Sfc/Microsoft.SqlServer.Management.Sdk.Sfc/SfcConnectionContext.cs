namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcConnectionContext
{
	private ISfcHasConnection domain;

	private SfcConnectionContextMode mode;

	public SfcConnectionContextMode Mode
	{
		get
		{
			if (domain.GetConnection() == null && mode != SfcConnectionContextMode.Offline)
			{
				mode = SfcConnectionContextMode.Offline;
			}
			return mode;
		}
		set
		{
			if (mode == value)
			{
				return;
			}
			if (value != SfcConnectionContextMode.TransactedBatch && value != SfcConnectionContextMode.NonTransactedBatch)
			{
				switch (mode)
				{
				case SfcConnectionContextMode.Online:
					if (value == SfcConnectionContextMode.Offline)
					{
						ForceDisconnected();
					}
					mode = value;
					return;
				case SfcConnectionContextMode.TransactedBatch:
				case SfcConnectionContextMode.NonTransactedBatch:
					switch (value)
					{
					case SfcConnectionContextMode.Offline:
						DiscardActionLog();
						mode = value;
						return;
					case SfcConnectionContextMode.Online:
						FlushActionLog();
						mode = value;
						return;
					}
					break;
				}
			}
			throw new SfcInvalidConnectionContextModeChangeException(mode.ToString(), value.ToString());
		}
	}

	public SfcConnectionContext(ISfcHasConnection domain)
	{
		this.domain = domain;
		if (domain.GetConnection() != null)
		{
			mode = SfcConnectionContextMode.Online;
		}
		else
		{
			mode = SfcConnectionContextMode.Offline;
		}
	}

	public void FlushActionLog()
	{
	}

	private void DiscardActionLog()
	{
	}

	private void ForceDisconnected()
	{
		domain.GetConnection()?.ForceDisconnected();
	}
}
