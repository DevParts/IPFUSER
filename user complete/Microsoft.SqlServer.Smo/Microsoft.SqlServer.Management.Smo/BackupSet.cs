using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class BackupSet
{
	internal decimal StopAtLsn;

	internal int ID;

	private bool isPopulated;

	internal Server server;

	internal string name;

	internal string description;

	internal BackupSetType backupSetType;

	internal DateTime backupStartDate;

	internal DateTime backupFinishDate;

	internal DateTime expirationDate;

	internal int position;

	internal string databaseName;

	internal string serverName;

	internal string machineName;

	internal string userName;

	internal ServerVersion serverVersion;

	internal bool isSnapshot;

	internal bool isReadOnly;

	internal bool isDamaged;

	internal bool isCopyOnly;

	internal bool isForceOffline;

	internal bool hasIncompleteMetaData;

	internal bool hasBulkLoggedData;

	internal bool beginsLogChain;

	internal int softwareVendorId;

	internal decimal firstLsn;

	internal decimal lastLsn;

	internal decimal checkpointLsn;

	internal decimal databaseBackupLsn;

	internal decimal forkPointLsn;

	internal decimal differentialBaseLsn;

	internal decimal backupSize;

	internal decimal compressedBackupSize;

	internal Guid backupSetGuid = default(Guid);

	internal Guid databaseGuid = default(Guid);

	internal Guid familyGuid = default(Guid);

	internal Guid differentialBaseGuid = default(Guid);

	internal Guid recoveryForkID = default(Guid);

	internal Guid firstRecoveryForkID = default(Guid);

	internal BackupMediaSet backupMediaSet;

	private DataSet fileList;

	private int targetServerVersion => server.Version.Major;

	public Server Parent => server;

	public string Name
	{
		get
		{
			Populate();
			return name;
		}
	}

	public string Description
	{
		get
		{
			Populate();
			return description;
		}
	}

	public BackupSetType BackupSetType
	{
		get
		{
			Populate();
			return backupSetType;
		}
	}

	public DateTime BackupStartDate
	{
		get
		{
			Populate();
			return backupStartDate;
		}
	}

	public DateTime BackupFinishDate
	{
		get
		{
			Populate();
			return backupFinishDate;
		}
	}

	public DateTime ExpirationDate
	{
		get
		{
			Populate();
			return expirationDate;
		}
	}

	public int Position
	{
		get
		{
			Populate();
			return position;
		}
	}

	public string DatabaseName
	{
		get
		{
			Populate();
			return databaseName;
		}
	}

	public string ServerName
	{
		get
		{
			Populate();
			return serverName;
		}
	}

	public string MachineName
	{
		get
		{
			Populate();
			return machineName;
		}
	}

	public string UserName
	{
		get
		{
			Populate();
			return userName;
		}
	}

	public ServerVersion ServerVersion
	{
		get
		{
			Populate();
			return serverVersion;
		}
	}

	public bool IsSnapshot
	{
		get
		{
			VersionCheck(9, "IsSnapshot");
			Populate();
			return isSnapshot;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			VersionCheck(9, "IsReadOnly");
			Populate();
			return isReadOnly;
		}
	}

	public bool IsDamaged
	{
		get
		{
			VersionCheck(9, "IsDamaged");
			Populate();
			return isDamaged;
		}
	}

	public bool IsCopyOnly
	{
		get
		{
			VersionCheck(9, "IsCopyOnly");
			Populate();
			return isCopyOnly;
		}
	}

	public bool IsForceOffline
	{
		get
		{
			VersionCheck(9, "IsForceOffline");
			Populate();
			return isForceOffline;
		}
	}

	public bool HasIncompleteMetaData
	{
		get
		{
			VersionCheck(9, "HasIncompleteMetaData");
			Populate();
			return hasIncompleteMetaData;
		}
	}

	public bool HasBulkLoggedData
	{
		get
		{
			VersionCheck(9, "HasBulkLoggedData");
			Populate();
			return hasBulkLoggedData;
		}
	}

	public bool BeginsLogChain
	{
		get
		{
			VersionCheck(9, "BeginsLogChain");
			Populate();
			return beginsLogChain;
		}
	}

	public int SoftwareVendorId
	{
		get
		{
			Populate();
			return softwareVendorId;
		}
	}

	public decimal FirstLsn
	{
		get
		{
			Populate();
			return firstLsn;
		}
	}

	public decimal LastLsn
	{
		get
		{
			Populate();
			return lastLsn;
		}
	}

	public decimal CheckpointLsn
	{
		get
		{
			Populate();
			return checkpointLsn;
		}
	}

	public decimal DatabaseBackupLsn
	{
		get
		{
			Populate();
			return databaseBackupLsn;
		}
	}

	public decimal ForkPointLsn
	{
		get
		{
			VersionCheck(9, "ForkPointLsn");
			Populate();
			return forkPointLsn;
		}
	}

	public decimal DifferentialBaseLsn
	{
		get
		{
			VersionCheck(9, "DifferentialBaseLsn");
			Populate();
			return differentialBaseLsn;
		}
	}

	public decimal BackupSize
	{
		get
		{
			Populate();
			return backupSize;
		}
	}

	public decimal CompressedBackupSize
	{
		get
		{
			VersionCheck(10, "CompressedBackupSize");
			Populate();
			return compressedBackupSize;
		}
	}

	public Guid BackupSetGuid => backupSetGuid;

	public Guid DatabaseGuid
	{
		get
		{
			VersionCheck(9, "DatabaseGuid");
			Populate();
			return databaseGuid;
		}
	}

	public Guid FamilyGuid
	{
		get
		{
			VersionCheck(9, "FamilyGuid");
			Populate();
			return familyGuid;
		}
	}

	public Guid DifferentialBaseGuid
	{
		get
		{
			VersionCheck(9, "DifferentialBaseGuid");
			Populate();
			return differentialBaseGuid;
		}
	}

	public Guid RecoveryForkID
	{
		get
		{
			VersionCheck(9, "RecoveryForkID");
			Populate();
			return recoveryForkID;
		}
	}

	public Guid FirstRecoveryForkID
	{
		get
		{
			VersionCheck(9, "FirstRecoveryForkID");
			Populate();
			return firstRecoveryForkID;
		}
	}

	public BackupMediaSet BackupMediaSet => backupMediaSet;

	internal DataSet FileList
	{
		get
		{
			if (fileList == null)
			{
				BackupMedia backupMedia = BackupMediaSet.BackupMediaList.ElementAt(0);
				string query = string.Format(SmoApplication.DefaultCulture, "RESTORE FILELISTONLY FROM {0} WITH FILE = {1}", new object[2]
				{
					BackupMedia.GetBackupMediaNameForScript(backupMedia.MediaName, backupMedia.MediaType),
					Position
				});
				fileList = server.ExecutionManager.ExecuteWithResults(query);
			}
			return fileList;
		}
	}

	internal BackupSet(Server parentServer, Guid BackupSetGuid)
	{
		server = parentServer;
		backupSetGuid = BackupSetGuid;
	}

	internal BackupSet(Server parentServer, BackupMediaSet mediaSet, DataRow dr)
	{
		server = parentServer;
		backupMediaSet = mediaSet;
		PopulateFromDevice(dr);
	}

	public void CheckBackupFilesExistence()
	{
		IEnumerable<BackupMedia> enumerable = from BackupMedia x in BackupMediaSet.BackupMediaList
			orderby x.MirrorSequenceNumber
			group x by x.FamilySequenceNumber into x
			select x.First();
		foreach (BackupMedia item in enumerable)
		{
			if (item.MediaType == DeviceType.File)
			{
				Request request = new Request();
				request.Urn = "Server/File[@FullName='" + Urn.EscapeString(item.MediaName) + "']";
				DataTable enumeratorData = server.ExecutionManager.GetEnumeratorData(request);
				bool flag = false;
				if (enumeratorData != null && enumeratorData.Rows != null && enumeratorData.Rows.Count > 0)
				{
					flag = Convert.ToBoolean(enumeratorData.Rows[0]["IsFile"], CultureInfo.InvariantCulture);
				}
				if (!flag)
				{
					throw new SmoException(ExceptionTemplatesImpl.BackupFileNotFound(item.MediaName));
				}
			}
		}
	}

	public void Verify()
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("RESTORE VERIFYONLY FROM");
		BackupMedia backupMedia = null;
		int i;
		for (i = 1; i <= BackupMediaSet.FamilyCount; i++)
		{
			IOrderedEnumerable<BackupMedia> source = from BackupMedia bkMedia in BackupMediaSet.BackupMediaList
				where bkMedia.FamilySequenceNumber == i
				orderby bkMedia.MirrorSequenceNumber
				select bkMedia;
			if (source.Count() > 0)
			{
				BackupMedia backupMedia2 = source.ElementAt(0);
				if (backupMedia == null && !string.IsNullOrEmpty(backupMedia2.CredentialName))
				{
					backupMedia = backupMedia2;
				}
				if (i != 1)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(BackupMedia.GetBackupMediaNameForScript(backupMedia2.MediaName, backupMedia2.MediaType));
			}
		}
		stringBuilder.Append(" WITH ");
		if (backupMedia != null && backupMedia.AddCredential(server.ServerVersion, stringBuilder))
		{
			stringBuilder.Append(Globals.commaspace);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FILE = {0}, NOUNLOAD, NOREWIND", new object[1] { Position });
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(stringBuilder.ToString());
		try
		{
			server.ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			if (ex.InnerException == null || !(ex.InnerException is SqlException ex2))
			{
				return;
			}
			bool flag = false;
			foreach (SqlError error in ex2.Errors)
			{
				if (error.Number == 3014)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				throw ex;
			}
		}
	}

	private void Populate()
	{
		if (!isPopulated)
		{
			DataTable dataTable = null;
			Request req = new Request(string.Format(SmoApplication.DefaultCulture, "Server/BackupSet[@BackupSetUuid='{0}']", new object[1] { backupSetGuid.ToString() }));
			dataTable = server.ExecutionManager.GetEnumeratorData(req);
			if (dataTable.Rows.Count >= 1)
			{
				DataRow dr = dataTable.Rows[0];
				Populate(dr);
			}
		}
	}

	private void Populate(DataRow dr)
	{
		if (targetServerVersion >= 7)
		{
			PopulateV7Properties(dr);
		}
		if (targetServerVersion >= 9)
		{
			PopulateV9Properties(dr);
		}
		if (targetServerVersion >= 10)
		{
			PopulateV10Properties(dr);
		}
		isPopulated = true;
	}

	private void PopulateFromDevice(DataRow dr)
	{
		if (targetServerVersion >= 7)
		{
			PopulateV7PropertiesFromDevices(dr);
		}
		if (targetServerVersion >= 9)
		{
			PopulateV9PropertiesFromDevices(dr);
		}
		if (targetServerVersion >= 10)
		{
			PopulateV10PropertiesFromDevices(dr);
		}
		isPopulated = true;
	}

	private void PopulateV7Properties(DataRow dr)
	{
		if (!(dr["Name"] is DBNull))
		{
			name = (string)dr["Name"];
		}
		if (!(dr["Description"] is DBNull))
		{
			description = (string)dr["Description"];
		}
		if (!(dr["BackupSetType"] is DBNull))
		{
			switch ((int)dr["BackupSetType"])
			{
			case 1:
				backupSetType = BackupSetType.Database;
				break;
			case 2:
				backupSetType = BackupSetType.Differential;
				break;
			case 3:
				backupSetType = BackupSetType.Log;
				break;
			case 4:
				backupSetType = BackupSetType.FileOrFileGroup;
				break;
			case 5:
				backupSetType = BackupSetType.FileOrFileGroupDifferential;
				break;
			}
		}
		if (!(dr["ID"] is DBNull))
		{
			ID = (int)dr["ID"];
		}
		if (!(dr["SoftwareVendorId"] is DBNull))
		{
			softwareVendorId = (int)dr["SoftwareVendorId"];
		}
		if (!(dr["BackupStartDate"] is DBNull))
		{
			backupStartDate = (DateTime)dr["BackupStartDate"];
		}
		if (!(dr["BackupFinishDate"] is DBNull))
		{
			backupFinishDate = (DateTime)dr["BackupFinishDate"];
		}
		if (!(dr["ExpirationDate"] is DBNull))
		{
			expirationDate = (DateTime)dr["ExpirationDate"];
		}
		if (!(dr["Position"] is DBNull))
		{
			position = (int)dr["Position"];
		}
		if (!(dr["DatabaseName"] is DBNull))
		{
			databaseName = (string)dr["DatabaseName"];
		}
		if (!(dr["ServerName"] is DBNull))
		{
			serverName = (string)dr["ServerName"];
		}
		if (!(dr["MachineName"] is DBNull))
		{
			machineName = (string)dr["MachineName"];
		}
		if (!(dr["UserName"] is DBNull))
		{
			userName = (string)dr["UserName"];
		}
		if (!(dr["SoftwareMajorVersion"] is DBNull))
		{
			int major = (byte)dr["SoftwareMajorVersion"];
			int minor = (byte)dr["SoftwareMinorVersion"];
			int buildNumber = (short)dr["SoftwareBuildVersion"];
			serverVersion = new ServerVersion(major, minor, buildNumber);
		}
		if (!(dr["FirstLsn"] is DBNull))
		{
			firstLsn = (decimal)dr["FirstLsn"];
		}
		if (!(dr["LastLsn"] is DBNull))
		{
			lastLsn = (decimal)dr["LastLsn"];
		}
		if (!(dr["CheckpointLsn"] is DBNull))
		{
			checkpointLsn = (decimal)dr["CheckpointLsn"];
		}
		if (!(dr["DatabaseBackupLsn"] is DBNull))
		{
			databaseBackupLsn = (decimal)dr["DatabaseBackupLsn"];
		}
		if (!(dr["BackupSize"] is DBNull))
		{
			backupSize = (decimal)dr["BackupSize"];
		}
		backupMediaSet = new BackupMediaSet(server, (int)dr["MediaSetId"]);
	}

	private void PopulateV7PropertiesFromDevices(DataRow dr)
	{
		if (!(dr["BackupName"] is DBNull))
		{
			name = (string)dr["BackupName"];
		}
		if (!(dr["BackupDescription"] is DBNull))
		{
			description = (string)dr["BackupDescription"];
		}
		switch ((byte)dr["BackupType"])
		{
		case 1:
			backupSetType = BackupSetType.Database;
			break;
		case 2:
			backupSetType = BackupSetType.Log;
			break;
		case 4:
			backupSetType = BackupSetType.FileOrFileGroup;
			break;
		case 5:
			backupSetType = BackupSetType.Differential;
			break;
		case 6:
			backupSetType = BackupSetType.FileOrFileGroupDifferential;
			break;
		}
		if (!(dr["SoftwareVendorId"] is DBNull))
		{
			softwareVendorId = (int)dr["SoftwareVendorId"];
		}
		if (!(dr["BackupStartDate"] is DBNull))
		{
			backupStartDate = (DateTime)dr["BackupStartDate"];
		}
		if (!(dr["BackupFinishDate"] is DBNull))
		{
			backupFinishDate = (DateTime)dr["BackupFinishDate"];
		}
		if (!(dr["ExpirationDate"] is DBNull))
		{
			expirationDate = (DateTime)dr["ExpirationDate"];
		}
		if (!(dr["Position"] is DBNull))
		{
			position = (short)dr["Position"];
		}
		if (!(dr["DatabaseName"] is DBNull))
		{
			databaseName = (string)dr["DatabaseName"];
		}
		if (!(dr["ServerName"] is DBNull))
		{
			serverName = (string)dr["ServerName"];
		}
		if (!(dr["MachineName"] is DBNull))
		{
			machineName = (string)dr["MachineName"];
		}
		if (!(dr["UserName"] is DBNull))
		{
			userName = (string)dr["UserName"];
		}
		if (!(dr["SoftwareVersionMajor"] is DBNull))
		{
			int major = (int)dr["SoftwareVersionMajor"];
			int minor = (int)dr["SoftwareVersionMinor"];
			int buildNumber = (int)dr["SoftwareVersionBuild"];
			serverVersion = new ServerVersion(major, minor, buildNumber);
		}
		if (!(dr["FirstLSN"] is DBNull))
		{
			firstLsn = (decimal)dr["FirstLSN"];
		}
		if (!(dr["LastLSN"] is DBNull))
		{
			lastLsn = (decimal)dr["LastLSN"];
		}
		if (!(dr["CheckpointLSN"] is DBNull))
		{
			checkpointLsn = (decimal)dr["CheckpointLSN"];
		}
		if (!(dr["BackupSize"] is DBNull))
		{
			if (dr["BackupSize"] is long)
			{
				backupSize = (long)dr["BackupSize"];
			}
			else
			{
				backupSize = (decimal)dr["BackupSize"];
			}
		}
	}

	private void PopulateV9Properties(DataRow dr)
	{
		if (!(dr["DifferentialBaseGuid"] is DBNull))
		{
			differentialBaseGuid = (Guid)dr["DifferentialBaseGuid"];
		}
		if (!(dr["FirstRecoveryForkID"] is DBNull))
		{
			firstRecoveryForkID = (Guid)dr["FirstRecoveryForkID"];
		}
		if (!(dr["RecoveryForkID"] is DBNull))
		{
			recoveryForkID = (Guid)dr["RecoveryForkID"];
		}
		if (!(dr["FamilyGuid"] is DBNull))
		{
			familyGuid = (Guid)dr["FamilyGuid"];
		}
		if (!(dr["DatabaseGuid"] is DBNull))
		{
			databaseGuid = (Guid)dr["DatabaseGuid"];
		}
		if (!(dr["ForkPointLsn"] is DBNull))
		{
			forkPointLsn = (decimal)dr["ForkPointLsn"];
		}
		if (!(dr["DifferentialBaseLsn"] is DBNull))
		{
			differentialBaseLsn = (decimal)dr["DifferentialBaseLsn"];
		}
		hasIncompleteMetaData = (bool)dr["HasIncompleteMetaData"];
		hasBulkLoggedData = (bool)dr["HasBulkLoggedData"];
		isCopyOnly = (bool)dr["IsCopyOnly"];
		isForceOffline = (bool)dr["IsForceOffline"];
		isDamaged = (bool)dr["IsDamaged"];
		isReadOnly = (bool)dr["IsReadOnly"];
		isSnapshot = (bool)dr["IsSnapshot"];
		beginsLogChain = (bool)dr["BeginsLogChain"];
	}

	private void PopulateV9PropertiesFromDevices(DataRow dr)
	{
		if (!(dr["BackupSetGUID"] is DBNull))
		{
			backupSetGuid = (Guid)dr["BackupSetGUID"];
		}
		if (!(dr["DifferentialBaseGUID"] is DBNull))
		{
			differentialBaseGuid = (Guid)dr["DifferentialBaseGUID"];
		}
		if (!(dr["FirstRecoveryForkID"] is DBNull))
		{
			firstRecoveryForkID = (Guid)dr["FirstRecoveryForkID"];
		}
		if (!(dr["RecoveryForkID"] is DBNull))
		{
			recoveryForkID = (Guid)dr["RecoveryForkID"];
		}
		if (!(dr["FamilyGUID"] is DBNull))
		{
			familyGuid = (Guid)dr["FamilyGUID"];
		}
		if (!(dr["BindingID"] is DBNull))
		{
			databaseGuid = (Guid)dr["BindingID"];
		}
		if (!(dr["ForkPointLSN"] is DBNull))
		{
			forkPointLsn = (decimal)dr["ForkPointLSN"];
		}
		if (!(dr["DifferentialBaseLSN"] is DBNull))
		{
			differentialBaseLsn = (decimal)dr["DifferentialBaseLSN"];
		}
		if (!(dr["DatabaseBackupLSN"] is DBNull))
		{
			databaseBackupLsn = (decimal)dr["DatabaseBackupLSN"];
		}
		hasIncompleteMetaData = (bool)dr["HasIncompleteMetaData"];
		hasBulkLoggedData = (bool)dr["HasBulkLoggedData"];
		isCopyOnly = (bool)dr["IsCopyOnly"];
		isForceOffline = (bool)dr["IsForceOffline"];
		isDamaged = (bool)dr["IsDamaged"];
		isReadOnly = (bool)dr["IsReadOnly"];
		isSnapshot = (bool)dr["IsSnapshot"];
		beginsLogChain = (bool)dr["BeginsLogChain"];
	}

	private void PopulateV10Properties(DataRow dr)
	{
		if (!(dr["CompressedBackupSize"] is DBNull))
		{
			compressedBackupSize = (decimal)dr["CompressedBackupSize"];
		}
	}

	private void PopulateV10PropertiesFromDevices(DataRow dr)
	{
		if (!(dr["CompressedBackupSize"] is DBNull))
		{
			compressedBackupSize = (long)dr["CompressedBackupSize"];
		}
	}

	private void VersionCheck(int minSupportedVersion, string propertyName)
	{
		if (Parent.ServerVersion.Major < minSupportedVersion)
		{
			string message = ExceptionTemplatesImpl.CannotReadProp + " " + propertyName + ". " + ExceptionTemplatesImpl.PropertyAvailable + SqlSmoObject.GetSqlServerName(Parent);
			throw new UnsupportedVersionException(message);
		}
	}

	public DataSet GetFileList()
	{
		return FileList;
	}

	internal static bool IsBackupsForked(List<BackupSet> backupSetList)
	{
		if (backupSetList == null || backupSetList.Count == 0)
		{
			return false;
		}
		int num = backupSetList.Select((BackupSet bkSet) => bkSet.recoveryForkID).Distinct().Count();
		if (num == 1)
		{
			return false;
		}
		return true;
	}

	public static bool IsBackupSetsInSequence(BackupSet first, BackupSet second, ref decimal stopAtLsn)
	{
		string errMsg;
		object errSource;
		return IsBackupSetsInSequence(first, second, out errMsg, out errSource, ref stopAtLsn);
	}

	public static bool IsBackupSetsInSequence(BackupSet first, BackupSet second)
	{
		decimal stopAtLsn = 0m;
		string errMsg;
		object errSource;
		return IsBackupSetsInSequence(first, second, out errMsg, out errSource, ref stopAtLsn);
	}

	internal static bool IsBackupSetsInSequence(BackupSet first, BackupSet second, out string errMsg, out object errSource, ref decimal stopAtLsn)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(first != null && second != null);
		stopAtLsn = 0m;
		errMsg = null;
		errSource = null;
		if (first.BackupSetType == BackupSetType.FileOrFileGroup || first.BackupSetType == BackupSetType.FileOrFileGroupDifferential)
		{
			errMsg = ExceptionTemplatesImpl.FileGroupNotSupported;
			errSource = first;
			return false;
		}
		if (second.BackupSetType == BackupSetType.FileOrFileGroup || second.BackupSetType == BackupSetType.FileOrFileGroupDifferential)
		{
			errMsg = ExceptionTemplatesImpl.FileGroupNotSupported;
			errSource = second;
			return false;
		}
		if (first.FamilyGuid != second.FamilyGuid)
		{
			errMsg = ExceptionTemplatesImpl.BackupsOfDifferentDb;
			errSource = second;
			return false;
		}
		if (second.BackupSetType == BackupSetType.Database)
		{
			errMsg = ExceptionTemplatesImpl.FullBackupShouldBeFirst;
			errSource = second;
			return false;
		}
		if (first.CheckpointLsn > second.CheckpointLsn)
		{
			errMsg = ExceptionTemplatesImpl.BackupsNotInSequence;
			errSource = second;
			return false;
		}
		if (second.BackupSetType == BackupSetType.Differential)
		{
			if (first.BackupSetType != BackupSetType.Database)
			{
				errMsg = ExceptionTemplatesImpl.WrongDiffbackup;
				errSource = second;
				return false;
			}
			if (first.BackupSetGuid != second.DifferentialBaseGuid)
			{
				errMsg = ExceptionTemplatesImpl.DiffBackupNotCompatible;
				errSource = second;
				return false;
			}
			return true;
		}
		if (second.BackupSetType == BackupSetType.Log)
		{
			if (first.BackupSetType == BackupSetType.Database || first.BackupSetType == BackupSetType.Differential)
			{
				if (first.LastLsn >= second.FirstLsn && first.LastLsn <= second.LastLsn)
				{
					if (second.ForkPointLsn == 0m && first.FamilyGuid == second.FamilyGuid)
					{
						return true;
					}
					if (second.ForkPointLsn != 0m)
					{
						if (first.LastLsn <= second.ForkPointLsn && first.RecoveryForkID == second.FirstRecoveryForkID)
						{
							return true;
						}
						if (first.LastLsn > second.ForkPointLsn && first.RecoveryForkID == second.RecoveryForkID)
						{
							return true;
						}
					}
				}
			}
			else if (first.BackupSetType == BackupSetType.Log)
			{
				if (first.RecoveryForkID == second.FirstRecoveryForkID && first.LastLsn == second.FirstLsn)
				{
					return true;
				}
				if (second.ForkPointLsn != 0m && first.FirstLsn < second.FirstLsn && first.LastLsn > second.FirstLsn)
				{
					stopAtLsn = second.FirstLsn;
					return true;
				}
			}
			errMsg = ExceptionTemplatesImpl.WrongTLogbackup;
			errSource = second;
			return false;
		}
		return false;
	}
}
