using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ResourcePoolAffinityInfo : AffinityInfoBase
{
	internal ResourcePool resourcePool;

	private SchedulerCollection schedulerCollection;

	private DataTable schedulerTable;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public ResourcePool Parent => resourcePool;

	internal override SqlSmoObject SmoParent => Parent;

	public static string UrnSuffix => "ResourcePoolAffinityInfo";

	public SchedulerCollection Schedulers
	{
		get
		{
			if (table == null)
			{
				PopulateDataTable();
			}
			if (schedulerCollection == null)
			{
				schedulerCollection = new SchedulerCollection(this);
			}
			return schedulerCollection;
		}
	}

	internal ResourcePoolAffinityInfo(ResourcePool parent)
	{
		resourcePool = parent;
		PopulateDataTable();
	}

	public override void Refresh()
	{
		base.Refresh();
		schedulerTable = null;
		schedulerCollection = null;
	}

	internal override void PopulateDataTable()
	{
		int num = ((resourcePool.State == SqlSmoState.Creating) ? 1 : resourcePool.ID);
		Request req = new Request("Server/ResourceGovernor/ResourcePool/ResourcePoolAffinityInfo[@PoolID=" + num + "]");
		table = resourcePool.Parent.ExecutionManager.GetEnumeratorData(req);
		if (table.Rows.Count == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.ResourceGovernorPoolMissing);
		}
		if (1 == (int)table.Rows[0]["AffinityType"])
		{
			base.AffinityType = AffinityType.Manual;
		}
		else
		{
			base.AffinityType = AffinityType.Auto;
		}
		SetNumaValues();
		req = new Request("Server/ResourceGovernor/ResourcePool/ResourcePoolScheduler[@PoolId=" + num + "]");
		schedulerTable = resourcePool.Parent.ExecutionManager.GetEnumeratorData(req);
		SetSchedulerValues();
	}

	internal void SetSchedulerValues()
	{
		long num = 0L;
		char[] array = Convert.ToString(num, 2).PadLeft(64, '0').ToCharArray();
		Array.Reverse(array);
		foreach (DataRow row in schedulerTable.Rows)
		{
			int numanodeId = (int)row["NumaNodeId"];
			int cpuId = (int)row["CpuId"];
			int num2 = (int)row["SchedulerId"];
			long num3 = (long)row["SchedulerMask"];
			if (num3 != num)
			{
				num = num3;
				array = Convert.ToString(num, 2).PadLeft(64, '0').ToCharArray();
				Array.Reverse(array);
			}
			bool flag = base.AffinityType != AffinityType.Auto && array[num2 % 64] == '1';
			Scheduler scheduler = new Scheduler(num2, base.NumaNodes.GetByID(numanodeId).Cpus.GetByID(cpuId), flag, Schedulers);
			Schedulers.schedulerList.Add(num2, scheduler);
			scheduler.Cpu.InitAffinityMask(flag);
		}
	}

	internal void SetNumaValues()
	{
		int num = 0;
		foreach (DataRow row in table.Rows)
		{
			long num2 = (long)row["CpuIds"];
			char[] array = Convert.ToString(num2, 2).PadLeft(64, '0').ToCharArray();
			Array.Reverse(array);
			num2 = (long)row["CpuAffinityMask"];
			char[] array2 = Convert.ToString(num2, 2).PadLeft(64, '0').ToCharArray();
			Array.Reverse(array2);
			int num3 = (int)row["ID"];
			int num4 = (int)row["GroupID"];
			NumaNode numaNode = new NumaNode(num3, num4, this);
			base.NumaNodes.numaNodeCol.Add(num++, numaNode);
			int num5 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == '1')
				{
					int id = i + num4 * 64;
					Cpu value = new Cpu(id, num3, num4, affinityMask: false, numaNode.Cpus);
					numaNode.Cpus.cpuCol.Add(num5++, value);
				}
			}
		}
	}

	internal override StringCollection DoAlter(ScriptingPreferences sp)
	{
		if (sp.TargetServerVersion < SqlServerVersion.Version110)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER RESOURCE POOL {0} WITH (", new object[1] { Parent.FormatFullNameForScripting(sp) });
		StringCollection stringCollection = DoAlterInternal(sp);
		StringEnumerator enumerator = stringCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ")");
		StringCollection stringCollection2 = new StringCollection();
		stringCollection2.Add(stringBuilder.ToString());
		return stringCollection2;
	}

	internal StringCollection DoAlterInternal(ScriptingPreferences sp)
	{
		if (sp.TargetServerVersion < SqlServerVersion.Version110)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("AFFINITY ");
		if (base.AffinityType == AffinityType.Auto)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "SCHEDULER = AUTO");
			if (!Schedulers.setByUser && !base.NumaNodes.IsManuallySet())
			{
				StringCollection stringCollection = new StringCollection();
				stringCollection.Add(stringBuilder.ToString());
				return stringCollection;
			}
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.AffinityTypeCannotBeSet);
		}
		StringCollection stringCollection2 = base.NumaNodes.AddNumaInDdl(stringBuilder);
		if (stringCollection2 == null)
		{
			stringCollection2 = Schedulers.AddSchedulerInDdl(stringBuilder);
		}
		return stringCollection2;
	}
}
