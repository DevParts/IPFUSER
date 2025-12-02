using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ServerSetupAdapter : ServerAdapterBase, IDmfAdapter, IServerSetupFacet, IDmfFacet
{
	public string ServiceInstanceIdSuffix
	{
		get
		{
			string serviceInstanceId = base.Server.ServiceInstanceId;
			int num = serviceInstanceId.IndexOf(".", StringComparison.OrdinalIgnoreCase);
			return serviceInstanceId.Substring(num + 1);
		}
	}

	public string[] WindowsUsersAndGroupsInSysadminRole
	{
		get
		{
			StringCollection stringCollection = new StringCollection();
			try
			{
				string format = "Server/Role[@Name='{0}']/Member[@LoginType={1} or @LoginType={2}]";
				string text = string.Format(SmoApplication.DefaultCulture, format, new object[3] { "sysadmin", 1, 0 });
				Request req = new Request(text);
				foreach (DataRow row in base.Server.ExecutionManager.GetEnumeratorData(req).Rows)
				{
					stringCollection.Add(Convert.ToString(row["Name"], SmoApplication.DefaultCulture));
				}
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.EnumMembers, this, ex);
			}
			string[] array = new string[stringCollection.Count];
			stringCollection.CopyTo(array, 0);
			return array;
		}
	}

	public string TempdbPrimaryFilePath => base.Server.Databases["tempdb"].PrimaryFilePath;

	public string TempdbLogPath => Path.GetDirectoryName(base.Server.Databases["tempdb"].LogFiles[0].FileName);

	public ServiceStartMode AgentStartMode => base.Server.JobServer.ServiceStartMode;

	public string AgentServiceAccount => base.Server.JobServer.ServiceAccount;

	public string AgentDomainGroup => base.Server.JobServer.AgentDomainGroup;

	public string EngineServiceAccount => base.Server.ServiceAccount;

	public ServerSetupAdapter(Server obj)
		: base(obj)
	{
	}

	public override void Refresh()
	{
		base.Refresh();
		base.Server.Databases["tempdb"].Refresh();
		try
		{
			base.Server.JobServer.Refresh();
		}
		catch (UnsupportedFeatureException)
		{
		}
	}
}
