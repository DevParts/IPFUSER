using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DatabaseMaintenanceAdapter : DatabaseAdapter, IDatabaseMaintenanceFacet, IDmfFacet
{
	public bool DataAndBackupOnSeparateLogicalVolumes
	{
		get
		{
			List<string> list = new List<string>();
			try
			{
				DataRowCollection rows = base.Database.EnumBackupSets().Rows;
				List<string> list2 = new List<string>();
				foreach (DataRow item in rows)
				{
					string text = Convert.ToString(item["MediaSetId"], CultureInfo.InvariantCulture);
					if (!list2.Contains(text))
					{
						list2.Add(text);
						AddVolumesFromMediaFamily(text, list);
					}
				}
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.UnableToRetrieveBackupHistory, this, ex);
			}
			if (list.Count > 0)
			{
				return DataFileVolumeNotIn(list);
			}
			return false;
		}
	}

	public DatabaseMaintenanceAdapter(Database obj)
		: base(obj)
	{
	}

	private void AddVolumesFromMediaFamily(string mediaSetId, List<string> backupFileVolumes)
	{
		Request request = new Request();
		request.Urn = "Server/BackupMediaSet[@ID='" + Urn.EscapeString(mediaSetId) + "']/MediaFamily";
		DataTable enumeratorData = base.Database.ExecutionManager.GetEnumeratorData(request);
		foreach (DataRow row in enumeratorData.Rows)
		{
			string text = Convert.ToString(row["PhysicalDeviceName"], CultureInfo.InvariantCulture);
			if (!string.IsNullOrEmpty(text))
			{
				string volume = GetVolume(text);
				if (!string.IsNullOrEmpty(volume) && !backupFileVolumes.Contains(volume))
				{
					backupFileVolumes.Add(volume);
				}
			}
		}
	}
}
