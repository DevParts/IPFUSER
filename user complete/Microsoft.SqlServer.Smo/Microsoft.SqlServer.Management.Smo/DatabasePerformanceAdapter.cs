using System.Collections.Generic;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DatabasePerformanceAdapter : DatabaseAdapterBase, IDmfAdapter, IDatabasePerformanceFacet, IDmfFacet
{
	public bool DataAndLogFilesOnSeparateLogicalVolumes
	{
		get
		{
			List<string> list = new List<string>();
			foreach (LogFile logFile in base.Database.LogFiles)
			{
				string volume = GetVolume(logFile.FileName);
				if (!string.IsNullOrEmpty(volume) && !list.Contains(volume))
				{
					list.Add(volume);
				}
			}
			return DataFileVolumeNotIn(list);
		}
	}

	public bool CollationMatchesModelOrMaster
	{
		get
		{
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != base.Database.Parent, "Database Performance facet Database Parent object is null");
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != base.Database.Parent.Databases["master"], "Database Performance facet master database is null");
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != base.Database.Parent.Databases["model"], "Database Performance facet model database is null");
			if (!(base.Database.Collation == base.Database.Parent.Databases["master"].Collation))
			{
				return base.Database.Collation == base.Database.Parent.Databases["model"].Collation;
			}
			return true;
		}
	}

	public DatabasePerformanceAdapter(Database obj)
		: base(obj)
	{
	}

	public override void Refresh()
	{
		base.Database.Refresh();
		base.Database.LogFiles.Refresh();
		base.Database.FileGroups.Refresh();
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != base.Database.Parent, "Database Performance facet Database Parent object is null");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != base.Database.Parent.Databases["master"], "Database Performance facet master database is null");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != base.Database.Parent.Databases["model"], "Database Performance facet master database is null");
		base.Database.Parent.Databases["master"].Refresh();
		base.Database.Parent.Databases["model"].Refresh();
	}
}
