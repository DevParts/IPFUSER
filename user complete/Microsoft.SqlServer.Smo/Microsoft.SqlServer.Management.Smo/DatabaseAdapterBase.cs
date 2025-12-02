using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

public class DatabaseAdapterBase : IRefreshable, IAlterable
{
	private Database wrappedObject;

	protected Database Database => wrappedObject;

	public double Size => Database.Size;

	public DatabaseStatus Status => Database.Status;

	public DateTime LastBackupDate => Database.LastBackupDate;

	public DateTime LastLogBackupDate => Database.LastLogBackupDate;

	public bool IsSystemObject => Database.IsSystemObject;

	public bool Trustworthy
	{
		get
		{
			return Database.DatabaseOptions.Trustworthy;
		}
		set
		{
			Database.DatabaseOptions.Trustworthy = value;
		}
	}

	public bool AutoClose
	{
		get
		{
			return Database.DatabaseOptions.AutoClose;
		}
		set
		{
			Database.DatabaseOptions.AutoClose = value;
		}
	}

	public bool AutoShrink
	{
		get
		{
			return Database.DatabaseOptions.AutoShrink;
		}
		set
		{
			Database.DatabaseOptions.AutoShrink = value;
		}
	}

	public RecoveryModel RecoveryModel
	{
		get
		{
			return Database.DatabaseOptions.RecoveryModel;
		}
		set
		{
			Database.DatabaseOptions.RecoveryModel = value;
		}
	}

	public bool ReadOnly
	{
		get
		{
			return Database.DatabaseOptions.ReadOnly;
		}
		set
		{
			Database.DatabaseOptions.ReadOnly = value;
		}
	}

	public PageVerify PageVerify
	{
		get
		{
			return Database.DatabaseOptions.PageVerify;
		}
		set
		{
			Database.DatabaseOptions.PageVerify = value;
		}
	}

	public int TargetRecoveryTime
	{
		get
		{
			return Database.TargetRecoveryTime;
		}
		set
		{
			Database.TargetRecoveryTime = value;
		}
	}

	public DelayedDurability DelayedDurability
	{
		get
		{
			return Database.DelayedDurability;
		}
		set
		{
			Database.DelayedDurability = value;
		}
	}

	public DatabaseAdapterBase(Database obj)
	{
		wrappedObject = obj;
	}

	public virtual void Refresh()
	{
		Database.Refresh();
	}

	public virtual void Alter()
	{
		Database.Alter();
	}

	public string GetVolume(string file)
	{
		string pathRoot = Path.GetPathRoot(file);
		return pathRoot.ToUpperInvariant();
	}

	protected bool DataFileVolumeNotIn(List<string> checkVolumes)
	{
		TraceHelper.Assert(checkVolumes != null, "DataFileVolumeNotIn parameter checkVolumes should not be null");
		bool result = true;
		if (checkVolumes.Count > 0)
		{
			foreach (FileGroup fileGroup in Database.FileGroups)
			{
				foreach (DataFile file in fileGroup.Files)
				{
					string volume = GetVolume(file.FileName);
					if (checkVolumes.Contains(volume))
					{
						result = false;
						return result;
					}
				}
			}
		}
		return result;
	}
}
