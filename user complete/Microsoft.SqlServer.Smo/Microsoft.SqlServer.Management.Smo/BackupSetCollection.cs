using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class BackupSetCollection : ICollection, IEnumerable
{
	internal List<BackupSet> backupsetList = new List<BackupSet>();

	private readonly ICollection backupsets;

	private readonly Database parent;

	private readonly string databaseName;

	private readonly Server server;

	public BackupSet this[int index] => backupsetList[index];

	public int Count => backupsets.Count;

	public bool IsSynchronized => backupsets.IsSynchronized;

	public object SyncRoot => backupsets.SyncRoot;

	internal BackupSetCollection(Database parent)
	{
		this.parent = parent;
		databaseName = parent.Name;
		server = parent.GetServerObject();
		backupsets = backupsetList;
		GetBackupSetsFromMsdb(includeSnapshotBackups: true);
	}

	internal BackupSetCollection(Server server, string DatabaseName, bool PopulateFromMsdb, bool includeSnapshotBackups)
	{
		this.server = server;
		databaseName = DatabaseName;
		backupsets = backupsetList;
		if (PopulateFromMsdb)
		{
			GetBackupSetsFromMsdb(includeSnapshotBackups);
		}
	}

	private void GetBackupSetsFromMsdb(bool includeSnapshotBackups)
	{
		DataSet dataSet = null;
		string text = null;
		string text2 = ((server.ServerVersion.Major < 9 || includeSnapshotBackups) ? string.Empty : "and bkps.is_snapshot = 0");
		text = ((server.ServerVersion.Major >= 9 && parent != null && parent.Status != DatabaseStatus.Offline) ? string.Format(SmoApplication.DefaultCulture, "SELECT bkps.backup_set_uuid FROM msdb.dbo.backupset bkps WHERE bkps.database_guid = {0} {1} ORDER BY bkps.backup_set_id ASC", new object[2]
		{
			SqlSmoObject.MakeSqlString(parent.DatabaseGuid.ToString()),
			text2
		}) : string.Format(SmoApplication.DefaultCulture, "SELECT bkps.backup_set_uuid FROM msdb.dbo.backupset bkps WHERE bkps.database_name = {0} {1} ORDER BY bkps.backup_set_id ASC", new object[2]
		{
			SqlSmoObject.MakeSqlString(databaseName),
			text2
		}));
		dataSet = server.ExecutionManager.ExecuteWithResults(text);
		if (dataSet.Tables.Count <= 0)
		{
			return;
		}
		foreach (DataRow row in dataSet.Tables[0].Rows)
		{
			Guid backupSetGuid = (Guid)row[0];
			BackupSet item = new BackupSet(server, backupSetGuid);
			backupsetList.Add(item);
		}
	}

	public void CopyTo(Array array, int index)
	{
		backupsets.CopyTo(array, index);
	}

	public IEnumerator GetEnumerator()
	{
		return backupsets.GetEnumerator();
	}
}
