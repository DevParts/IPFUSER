using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ExternalResourcePoolAffinityInfo : AffinityInfoBase
{
	internal ExternalResourcePool externalResourcePool;

	private CpuCollection cpuCol;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public ExternalResourcePool Parent => externalResourcePool;

	internal override SqlSmoObject SmoParent => Parent;

	public static string UrnSuffix => "ExternalResourcePoolAffinityInfo";

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

	public ExternalResourcePoolAffinityInfo(ExternalResourcePool parent)
	{
		externalResourcePool = parent;
		PopulateDataTable();
	}

	public override void Refresh()
	{
		base.Refresh();
		cpuCol = null;
	}

	internal override void PopulateDataTable()
	{
		int num;
		bool cPUAndNumaValues;
		if (externalResourcePool.State == SqlSmoState.Creating)
		{
			num = 2;
			cPUAndNumaValues = true;
		}
		else
		{
			num = externalResourcePool.ID;
			cPUAndNumaValues = false;
		}
		Request req = new Request("Server/ResourceGovernor/ExternalResourcePool/ExternalResourcePoolAffinityInfo[@PoolID=" + num + "]");
		table = externalResourcePool.Parent.ExecutionManager.GetEnumeratorData(req);
		if (table.Rows.Count == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.ResourceGovernorPoolMissing);
		}
		base.AffinityType = AffinityType.Auto;
		SetCPUAndNumaValues(cPUAndNumaValues);
	}

	private void SetCPUAndNumaValues(bool isCreating)
	{
		int num = 0;
		int num2 = 0;
		foreach (DataRow row in table.Rows)
		{
			long num3 = (long)row["CpuIds"];
			char[] array = Convert.ToString(num3, 2).PadLeft(64, '0').ToCharArray();
			Array.Reverse(array);
			num3 = (long)row["CpuAffinityMask"];
			char[] array2 = Convert.ToString(num3, 2).PadLeft(64, '0').ToCharArray();
			Array.Reverse(array2);
			int num4 = (int)row["NumaNodeId"];
			int num5 = (int)row["GroupID"];
			if ((int)row["AffinityType"] == 1 && !isCreating)
			{
				base.AffinityType = AffinityType.Manual;
			}
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

	internal override StringCollection DoAlter(ScriptingPreferences sp)
	{
		if (sp.TargetServerVersion < SqlServerVersion.Version130)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER EXTERNAL RESOURCE POOL {0} WITH (", new object[1] { Parent.FormatFullNameForScripting(sp) });
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
		if (sp.TargetServerVersion < SqlServerVersion.Version130)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("AFFINITY ");
		if (base.AffinityType == AffinityType.Auto)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CPU = AUTO");
			if (!Cpus.setByUser)
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
			stringCollection2 = Cpus.AddCpuInDdl(stringBuilder);
		}
		return stringCollection2;
	}
}
