using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class BackupMediaSet
{
	public class IncompleteBackupMediaSetException : Exception
	{
		private BackupMediaSet backupMediaSet;

		private int missingFamilyNumber;

		public override string Message
		{
			get
			{
				if (backupMediaSet.BackupMediaList == null || backupMediaSet.BackupMediaList.Count() < 1)
				{
					return ExceptionTemplatesImpl.BackupMediaSetEmpty;
				}
				StringBuilder stringBuilder = new StringBuilder();
				foreach (BackupMedia backupMedia in backupMediaSet.BackupMediaList)
				{
					stringBuilder.Append(backupMedia.MediaName + "; ");
				}
				return ExceptionTemplatesImpl.BackupMediaSetNotComplete(stringBuilder.ToString(0, stringBuilder.Length - 2), backupMediaSet.FamilyCount, missingFamilyNumber);
			}
		}

		internal IncompleteBackupMediaSetException(BackupMediaSet backupMediaSet, int missingFamilyNumber)
		{
			this.backupMediaSet = backupMediaSet;
			this.missingFamilyNumber = missingFamilyNumber;
		}
	}

	internal bool IsPresentInMsdb = true;

	private Server server;

	internal Guid mediaSetGuid = default(Guid);

	internal int mediaSetID = -1;

	private string name;

	private string description;

	private int mirrorCount;

	private byte familyCount;

	private DeviceType mediaType;

	private List<BackupMedia> backupMediaList = new List<BackupMedia>();

	public string Name => Name;

	public string Description => description;

	public int MirrorCount => mirrorCount;

	public byte FamilyCount => familyCount;

	public DeviceType MediaType => mediaType;

	public IEnumerable<BackupMedia> BackupMediaList => backupMediaList;

	internal BackupMediaSet(Server server, int mediaSetID)
	{
		if (server == null)
		{
			throw new ArgumentNullException("Server");
		}
		this.mediaSetID = mediaSetID;
		this.server = server;
		Populate(mediaSetID);
	}

	public BackupMediaSet(Server server, List<BackupMedia> backupMediaList)
	{
		if (server == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.InitObject, this, new ArgumentNullException("Server"));
		}
		if (backupMediaList == null || backupMediaList.Count == 0)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.InitObject, this, new ArgumentNullException("BackupMediaList"));
		}
		this.server = server;
		mediaType = backupMediaList[0].MediaType;
		Populate(backupMediaList);
		CheckMediaSetComplete();
	}

	private void Populate(int mediaSetID)
	{
		string query = null;
		if (server.Version.Major < 9)
		{
			query = string.Format(SmoApplication.DefaultCulture, "SELECT bkms.media_uuid, bkms.name, bkms.description, bkms.media_family_count FROM msdb.dbo.backupmediaset bkms WHERE bkms.media_set_id = {0}", new object[1] { mediaSetID });
		}
		else if (server.Version.Major == 9)
		{
			query = string.Format(SmoApplication.DefaultCulture, "SELECT bkms.media_uuid, bkms.name, bkms.description, bkms.media_family_count, bkms.mirror_count FROM msdb.dbo.backupmediaset bkms WHERE bkms.media_set_id = {0}", new object[1] { mediaSetID });
		}
		else if (server.Version.Major <= 11)
		{
			query = string.Format(SmoApplication.DefaultCulture, "SELECT bkms.media_uuid, bkms.name, bkms.description, bkms.media_family_count, bkms.mirror_count, bkms.is_compressed FROM msdb.dbo.backupmediaset bkms WHERE bkms.media_set_id = {0}", new object[1] { mediaSetID });
		}
		else if (server.Version.Major > 11)
		{
			query = string.Format(SmoApplication.DefaultCulture, "SELECT bkms.media_uuid, bkms.name, bkms.description, bkms.media_family_count, bkms.mirror_count, bkms.is_compressed, bkms.is_encrypted FROM msdb.dbo.backupmediaset bkms WHERE bkms.media_set_id = {0}", new object[1] { mediaSetID });
		}
		DataSet dataSet = server.ExecutionManager.ExecuteWithResults(query);
		DataRow dataRow = dataSet.Tables[0].Rows[0];
		if (dataRow["name"] is string)
		{
			name = (string)dataRow["name"];
		}
		if (dataRow["description"] is string)
		{
			description = (string)dataRow["description"];
		}
		familyCount = (byte)dataRow["media_family_count"];
		if (dataRow["media_uuid"] is Guid)
		{
			mediaSetGuid = (Guid)dataRow["media_uuid"];
		}
		if (server.Version.Major >= 9)
		{
			mirrorCount = (byte)dataRow["mirror_count"];
		}
		query = ((server.Version.Major >= 9) ? string.Format(SmoApplication.DefaultCulture, "SELECT bkms.logical_device_name, bkms.physical_device_name, bkms.device_type, family_sequence_number, mirror FROM msdb.dbo.backupmediafamily bkms WHERE bkms.media_set_id = {0} ORDER BY bkms.family_sequence_number, bkms.mirror", new object[1] { mediaSetID }) : string.Format(SmoApplication.DefaultCulture, "SELECT bkms.logical_device_name, bkms.physical_device_name, bkms.device_type, family_sequence_number FROM msdb.dbo.backupmediafamily bkms WHERE bkms.media_set_id = {0} ORDER BY bkms.family_sequence_number", new object[1] { mediaSetID }));
		DataSet dataSet2 = server.ExecutionManager.ExecuteWithResults(query);
		foreach (DataRow row in dataSet2.Tables[0].Rows)
		{
			backupMediaList.Add(new BackupMedia(row));
		}
		IsPresentInMsdb = true;
	}

	private void Populate(List<BackupMedia> backupMediaList)
	{
		bool flag = true;
		foreach (BackupMedia backupMedia in backupMediaList)
		{
			try
			{
				DataTable dataTable = backupMedia.MediaLabel(server);
				DataRow dataRow = dataTable.Rows[0];
				if (flag)
				{
					if (dataRow["MediaName"] is string)
					{
						name = (string)dataRow["MediaName"];
					}
					if (dataRow["MediaDescription"] is string)
					{
						description = (string)dataRow["MediaDescription"];
					}
					familyCount = (byte)(int)dataRow["FamilyCount"];
					if (server.Version.Major >= 9)
					{
						mirrorCount = (byte)(int)dataRow["MirrorCount"];
					}
					if (dataRow["MediaSetId"] is Guid)
					{
						mediaSetGuid = (Guid)dataRow["MediaSetId"];
					}
				}
				else
				{
					Guid guid = default(Guid);
					if (dataRow["MediaSetId"] is Guid)
					{
						guid = (Guid)dataRow["MediaSetId"];
					}
					if (mediaSetGuid != guid)
					{
						throw new SmoException(ExceptionTemplatesImpl.MediaNotPartOfSet);
					}
				}
				this.backupMediaList.Add(backupMedia);
				flag = false;
			}
			catch (ExecutionFailureException)
			{
			}
		}
	}

	internal List<BackupSet> ReadBackupSetHeader()
	{
		List<BackupSet> list = new List<BackupSet>();
		foreach (BackupMedia backupMedia in BackupMediaList)
		{
			DataTable dataTable = backupMedia.MediaHeader(server);
			if (dataTable == null)
			{
				continue;
			}
			foreach (DataRow row in dataTable.Rows)
			{
				list.Add(new BackupSet(server, this, row));
			}
			break;
		}
		return list;
	}

	internal void CheckMediaSetComplete()
	{
		int i;
		for (i = 0; i < FamilyCount; i++)
		{
			IEnumerable<BackupMedia> enumerable = from BackupMedia bkMedia in BackupMediaList
				where bkMedia.FamilySequenceNumber == i + 1
				select bkMedia;
			if (enumerable == null || enumerable.Count() == 0)
			{
				throw new IncompleteBackupMediaSetException(this, i + 1);
			}
		}
	}
}
