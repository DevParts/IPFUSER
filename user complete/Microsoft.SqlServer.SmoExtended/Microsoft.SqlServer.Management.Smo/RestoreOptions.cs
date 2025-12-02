namespace Microsoft.SqlServer.Management.Smo;

public class RestoreOptions
{
	private bool keepReplication;

	private bool keepTemporalRetention;

	private bool setRestrictedUser;

	private int blocksize = -1;

	private int bufferCount = -1;

	private int maxTransferSize = -1;

	private int percentCompleteNotification = -1;

	public bool KeepReplication
	{
		get
		{
			return keepReplication;
		}
		set
		{
			if (RecoveryState == DatabaseRecoveryState.WithNoRecovery && value)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.ConflictWithNoRecovery);
			}
			keepReplication = value;
		}
	}

	public bool KeepTemporalRetention
	{
		get
		{
			return keepTemporalRetention;
		}
		set
		{
			if (RecoveryState == DatabaseRecoveryState.WithNoRecovery && value)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.ConflictWithNoRecovery);
			}
			keepTemporalRetention = value;
		}
	}

	public bool SetRestrictedUser
	{
		get
		{
			return setRestrictedUser;
		}
		set
		{
			setRestrictedUser = value;
		}
	}

	public bool ContinueAfterError { get; set; }

	public bool ClearSuspectPageTableAfterRestore { get; set; }

	public DatabaseRecoveryState RecoveryState { get; set; }

	public string StandByFile { get; set; }

	public int Blocksize
	{
		get
		{
			return blocksize;
		}
		set
		{
			blocksize = value;
		}
	}

	public int BufferCount
	{
		get
		{
			return bufferCount;
		}
		set
		{
			bufferCount = value;
		}
	}

	public int MaxTransferSize
	{
		get
		{
			return maxTransferSize;
		}
		set
		{
			maxTransferSize = value;
		}
	}

	public bool ReplaceDatabase { get; set; }

	public int PercentCompleteNotification
	{
		get
		{
			return percentCompleteNotification;
		}
		set
		{
			percentCompleteNotification = value;
		}
	}
}
