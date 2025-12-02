using System.Data;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class SmartAdminState : ISmartAdminState, IDmfFacet, IDmfAdapter, IRefreshable
{
	private bool isInitialized;

	private SmartAdmin smartAdmin;

	private bool isMasterSwitchEnabled;

	private bool isBackupEnabled;

	private int numberOfStorageConnectivityErrors;

	private int numberOfSqlErrors;

	private int numberOfInvalidCredentialErrors;

	private int numberOfOtherErrors;

	private int numberOfCorruptedOrDeletedBackups;

	private int numberOfBackupLoops;

	private int numberOfRetentionLoops;

	public bool IsMasterSwitchEnabled
	{
		get
		{
			CheckInitialized();
			return isMasterSwitchEnabled;
		}
	}

	public bool IsBackupEnabled
	{
		get
		{
			CheckInitialized();
			return isBackupEnabled;
		}
	}

	public int NumberOfStorageConnectivityErrors
	{
		get
		{
			CheckInitialized();
			return numberOfStorageConnectivityErrors;
		}
	}

	public int NumberOfSqlErrors
	{
		get
		{
			CheckInitialized();
			return numberOfSqlErrors;
		}
	}

	public int NumberOfInvalidCredentialErrors
	{
		get
		{
			CheckInitialized();
			return numberOfInvalidCredentialErrors;
		}
	}

	public int NumberOfOtherErrors
	{
		get
		{
			CheckInitialized();
			return numberOfOtherErrors;
		}
	}

	public int NumberOfCorruptedOrDeletedBackups
	{
		get
		{
			CheckInitialized();
			return numberOfCorruptedOrDeletedBackups;
		}
	}

	public int NumberOfBackupLoops
	{
		get
		{
			CheckInitialized();
			return numberOfBackupLoops;
		}
	}

	public int NumberOfRetentionLoops
	{
		get
		{
			CheckInitialized();
			return numberOfRetentionLoops;
		}
	}

	public SmartAdminState(SmartAdmin smartadmin)
	{
		smartAdmin = smartadmin;
		isInitialized = false;
	}

	public void Refresh()
	{
		smartAdmin.Refresh();
		isInitialized = false;
	}

	private void Initialize()
	{
		isMasterSwitchEnabled = smartAdmin.MasterSwitch;
		isBackupEnabled = smartAdmin.BackupEnabled;
		DataTable dataTable = smartAdmin.EnumHealthStatus();
		if (dataTable.Rows.Count > 0)
		{
			DataRow dataRow = dataTable.Rows[0];
			numberOfStorageConnectivityErrors = (int)dataRow["number_of_storage_connectivity_errors"];
			numberOfSqlErrors = (int)dataRow["number_of_sql_errors"];
			numberOfInvalidCredentialErrors = (int)dataRow["number_of_invalid_credential_errors"];
			numberOfOtherErrors = (int)dataRow["number_of_other_errors"];
			numberOfCorruptedOrDeletedBackups = (int)dataRow["number_of_corrupted_or_deleted_backups"];
			numberOfBackupLoops = (int)dataRow["number_of_backup_loops"];
			numberOfRetentionLoops = (int)dataRow["number_of_retention_loops"];
		}
		isInitialized = true;
	}

	private void CheckInitialized()
	{
		if (!isInitialized)
		{
			Initialize();
		}
	}
}
