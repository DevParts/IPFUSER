using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class PageRestorePlanner
{
	private Database database;

	private List<SuspectPage> suspectPages = new List<SuspectPage>();

	private Server Server
	{
		get
		{
			if (Database != null)
			{
				return Database.GetServerObject();
			}
			return null;
		}
	}

	public Database Database
	{
		get
		{
			return database;
		}
		set
		{
			database = value;
			if (database == null)
			{
				return;
			}
			SuspectPages.Clear();
			List<SuspectPage> list = GetSuspectPages(database);
			foreach (SuspectPage item in list)
			{
				try
				{
					item.Validate();
					SuspectPages.Add(item);
				}
				catch (Exception)
				{
				}
			}
		}
	}

	public string TailLogBackupFile { get; set; }

	public ICollection<SuspectPage> SuspectPages => suspectPages;

	public PageRestorePlanner(Database database)
	{
		Database = database;
	}

	public PageRestorePlanner(Database database, string tailLogBackupFileName)
	{
		Database = database;
		TailLogBackupFile = tailLogBackupFileName;
	}

	public RestorePlan CreateRestorePlan()
	{
		CheckPageRestorePossible();
		RestorePlan restorePlan = new RestorePlan(Database);
		restorePlan.RestoreAction = RestoreActionType.OnlinePage;
		if (SuspectPages == null || SuspectPages.Count < 1)
		{
			return restorePlan;
		}
		BackupSetCollection backupSets = Database.BackupSets;
		List<BackupSet> list = CreatePageRestorePlan(backupSets);
		if (list.Count > 0)
		{
			restorePlan.AddRestoreOperation(list);
			AddTailLogBackupRestore(restorePlan, backupSets);
			restorePlan.RestoreOperations[0].Action = RestoreActionType.OnlinePage;
			foreach (SuspectPage suspectPage in SuspectPages)
			{
				suspectPage.Validate();
				restorePlan.RestoreOperations[0].DatabasePages.Add(suspectPage);
			}
			CheckDuplicateSuspectPages();
		}
		restorePlan.SetRestoreOptions(new RestoreOptions());
		return restorePlan;
	}

	private void AddTailLogBackupRestore(RestorePlan plan, BackupSetCollection backupSets)
	{
		if (string.IsNullOrEmpty(TailLogBackupFile))
		{
			throw new PropertyNotSetException("TailLogBackupFile");
		}
		Backup backup = new Backup();
		backup.Database = Database.Name;
		backup.Action = BackupActionType.Log;
		backup.BackupSetName = Path.GetFileNameWithoutExtension(TailLogBackupFile);
		if (BackupRestoreBase.IsBackupDeviceUrl(TailLogBackupFile))
		{
			backup.Devices.Add(new BackupDeviceItem(TailLogBackupFile, DeviceType.Url));
		}
		else
		{
			TailLogBackupFile = Path.GetFullPath(TailLogBackupFile);
			if (!BackupRestoreBase.CheckNewBackupFile(Server, TailLogBackupFile))
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.BackupFileAlreadyExists(TailLogBackupFile));
			}
			backup.Devices.Add(new BackupDeviceItem(TailLogBackupFile, DeviceType.File));
		}
		backup.NoRewind = true;
		plan.TailLogBackupOperation = backup;
		Restore restore = new Restore();
		restore.Database = Database.Name;
		restore.Action = RestoreActionType.Log;
		if (BackupRestoreBase.IsBackupDeviceUrl(TailLogBackupFile))
		{
			restore.Devices.Add(new BackupDeviceItem(TailLogBackupFile, DeviceType.Url));
		}
		else
		{
			restore.Devices.Add(new BackupDeviceItem(TailLogBackupFile, DeviceType.File));
		}
		plan.RestoreOperations.Add(restore);
	}

	internal void CheckPageRestorePossible()
	{
		if (Database == null)
		{
			throw new PropertyNotSetException("Database");
		}
		if (Server.Version.Major < 9)
		{
			throw new InvalidVersionSmoOperationException(Server.ExecutionManager.GetServerVersion());
		}
		if (Database.RecoveryModel != RecoveryModel.Full)
		{
			throw new InvalidOperationException(ExceptionTemplatesImpl.PageRestoreOnlyForFullRecovery);
		}
		if (Database.Status != DatabaseStatus.Normal && Database.Status != DatabaseStatus.Suspect && Database.Status != DatabaseStatus.EmergencyMode)
		{
			throw new InvalidOperationException(ExceptionTemplatesImpl.InvalidDatabaseState);
		}
	}

	private List<BackupSet> CreatePageRestorePlan(BackupSetCollection backupsets)
	{
		List<BackupSet> list = new List<BackupSet>();
		int num = 0;
		int num2 = -1;
		IEnumerable<BackupSet> source = from BackupSet bkset in backupsets
			where bkset.BackupSetType == BackupSetType.Database || bkset.BackupSetType == BackupSetType.Differential
			orderby bkset.BackupStartDate descending, bkset.ID descending
			select bkset;
		for (num = 0; num < source.Count(); num++)
		{
			if (source.ElementAt(num).BackupSetType == BackupSetType.Database)
			{
				list.Add(source.ElementAt(num));
				num2++;
				break;
			}
		}
		if (num == source.Count())
		{
			list.Clear();
			return list;
		}
		if (num > 0)
		{
			list.Add(source.ElementAt(0));
			num2++;
		}
		DateTime lastBkStartDate = list[num2].BackupStartDate;
		source = from BackupSet bkset in backupsets
			where bkset.BackupSetType == BackupSetType.Log && bkset.BackupStartDate >= lastBkStartDate
			orderby bkset.BackupStartDate, bkset.ID
			select bkset;
		foreach (BackupSet item in source)
		{
			list.Add(item);
		}
		for (int num3 = 0; num3 < list.Count - 1; num3++)
		{
			if (!BackupSet.IsBackupSetsInSequence(list[num3], list[num3 + 1]))
			{
				throw new SmoException(ExceptionTemplatesImpl.UnableToCreatePageRestoreSequence);
			}
		}
		string query = string.Format(SmoApplication.DefaultCulture, "SELECT TOP(1) restore_date FROM msdb.dbo.restorehistory WHERE destination_database_name = N'{0}' ORDER BY restore_date DESC", new object[1] { SqlSmoObject.SqlString(Database.Name) });
		DateTime? dateTime = null;
		DataSet dataSet = Server.ExecutionManager.ExecuteWithResults(query);
		if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0 && !(dataSet.Tables[0].Rows[0]["restore_date"] is DBNull))
		{
			dateTime = (DateTime)dataSet.Tables[0].Rows[0]["restore_date"];
		}
		if (dateTime.HasValue && list.Count > 0 && dateTime > list[list.Count - 1].BackupStartDate)
		{
			throw new SmoException(ExceptionTemplatesImpl.UnableToCreatePlanTakeTLogBackup);
		}
		return list;
	}

	internal void CheckDuplicateSuspectPages()
	{
		suspectPages.Sort();
		for (int i = 1; i < suspectPages.Count; i++)
		{
			if (suspectPages[i - 1].Equals(suspectPages[i]))
			{
				throw new SmoException(ExceptionTemplatesImpl.DuplicateSuspectPage(suspectPages[i].FileID, suspectPages[i].PageID));
			}
		}
	}

	public static List<SuspectPage> GetSuspectPages(Database database)
	{
		List<SuspectPage> list = new List<SuspectPage>();
		Restore restore = new Restore();
		restore.Database = database.Name;
		DataTable dataTable = restore.ReadSuspectPageTable(database.GetServerObject());
		if (dataTable != null)
		{
			foreach (DataRow row in dataTable.Rows)
			{
				int fileID = (int)row["file_id"];
				long pageID = (long)row["page_id"];
				int num = (int)row["event_type"];
				if (num == 1 || num == 2 || num == 3)
				{
					list.Add(new SuspectPage(fileID, pageID));
				}
			}
		}
		return list;
	}
}
