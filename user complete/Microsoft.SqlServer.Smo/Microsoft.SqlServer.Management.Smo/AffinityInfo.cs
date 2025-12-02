using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class AffinityInfo : AffinityInfoBase
{
	internal Server server;

	private CpuCollection cpuCol;

	public static string UrnSuffix => "AffinityInfo";

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent => server;

	public CpuCollection Cpus
	{
		get
		{
			if (table == null)
			{
				PopulateDataTable();
			}
			if (cpuCol == null)
			{
				cpuCol = new CpuCollection(this);
			}
			return cpuCol;
		}
	}

	internal override SqlSmoObject SmoParent => Parent;

	internal AffinityInfo(Server parentsrv)
	{
		server = parentsrv;
		PopulateDataTable();
	}

	public override void Refresh()
	{
		base.Refresh();
		cpuCol = null;
	}

	internal override void PopulateDataTable()
	{
		Request req = new Request("Server/AffinityInfo");
		new Enumerator();
		table = server.ExecutionManager.GetEnumeratorData(req);
		if (1 == (int)table.Rows[0]["AffinityType"])
		{
			base.AffinityType = AffinityType.Manual;
		}
		else
		{
			base.AffinityType = AffinityType.Auto;
		}
		SetCPUAndNumaValues();
	}

	internal override StringCollection DoAlter(ScriptingPreferences sp)
	{
		StringCollection stringCollection = new StringCollection();
		if (sp.TargetServerVersion < SqlServerVersion.Version105)
		{
			return stringCollection;
		}
		if (base.AffinityInfoTable == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER SERVER CONFIGURATION SET PROCESS AFFINITY ");
		if (base.AffinityType == AffinityType.Auto)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CPU = AUTO");
			if (!Cpus.setByUser)
			{
				stringCollection.Add(stringBuilder.ToString());
				return stringCollection;
			}
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.AffinityTypeCannotBeSet);
		}
		stringCollection.Add(stringBuilder.ToString());
		StringCollection stringCollection2 = base.NumaNodes.AddNumaInDdl(stringBuilder);
		if (stringCollection2 == null)
		{
			stringCollection2 = Cpus.AddCpuInDdl(stringBuilder);
		}
		return stringCollection2;
	}

	internal void SetCPUAndNumaValues()
	{
		int num = 0;
		int num2 = 0;
		foreach (DataRow row in table.Rows)
		{
			if (string.Compare(row["NodeStateDesc"].ToString(), "ONLINE DAC", StringComparison.OrdinalIgnoreCase) == 0)
			{
				continue;
			}
			long num3 = (long)row["CpuIds"];
			char[] array = Convert.ToString(num3, 2).PadLeft(64, '0').ToCharArray();
			Array.Reverse(array);
			num3 = (long)row["CpuAffinityMask"];
			char[] array2 = Convert.ToString(num3, 2).PadLeft(64, '0').ToCharArray();
			Array.Reverse(array2);
			int num4 = (int)row["ID"];
			int num5 = (int)row["GroupID"];
			NumaNode numaNode = new NumaNode(num4, num5, this);
			base.NumaNodes.numaNodeCol.Add(num2++, numaNode);
			int num6 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == '1')
				{
					int id = i + num5 * 64;
					Cpu value = ((base.AffinityType != AffinityType.Auto) ? new Cpu(id, num4, num5, array2[i].Equals('1'), Cpus) : new Cpu(id, num4, num5, affinityMask: false, Cpus));
					Cpus.cpuCol.Add(num++, value);
					numaNode.Cpus.cpuCol.Add(num6++, value);
				}
			}
		}
	}
}
