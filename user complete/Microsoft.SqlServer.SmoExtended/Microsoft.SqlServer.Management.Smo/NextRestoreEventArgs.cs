using System;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class NextRestoreEventArgs : EventArgs
{
	public bool Continue { get; set; }

	public string BackupSetName { get; private set; }

	public string BackupSetDescription { get; private set; }

	public string DevicesName { get; private set; }

	public int Count { get; private set; }

	public NextRestoreEventArgs(string backupSetName, string backupSetDescription, string deviceName, int count)
	{
		Continue = true;
		BackupSetName = backupSetName;
		BackupSetDescription = backupSetDescription;
		DevicesName = deviceName;
		Count = count;
	}
}
