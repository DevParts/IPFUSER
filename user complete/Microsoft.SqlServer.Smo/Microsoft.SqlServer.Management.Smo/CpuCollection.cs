using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class CpuCollection : ICollection, IEnumerable
{
	private AffinityInfoBase parent;

	private NumaCPUCollectionBase<Cpu> cpuCollection;

	internal SortedList cpuCol;

	internal bool setByUser;

	private ICollection iCol;

	private Dictionary<int, Cpu> cpuCollectionFromId;

	private int maxCpuId = -1;

	private int minCpuId = int.MaxValue;

	public int Count => iCol.Count;

	public bool IsSynchronized => iCol.IsSynchronized;

	public object SyncRoot => iCol.SyncRoot;

	public Cpu this[int index] => cpuCollection[index];

	private Dictionary<int, Cpu> CpuCollectionFromId
	{
		get
		{
			if (cpuCollectionFromId == null)
			{
				cpuCollectionFromId = new Dictionary<int, Cpu>();
				for (int i = 0; i < Count; i++)
				{
					cpuCollectionFromId.Add(this[i].ID, this[i]);
				}
			}
			return cpuCollectionFromId;
		}
	}

	private int MaxCpuId
	{
		get
		{
			if (maxCpuId == -1)
			{
				for (int i = 0; i < Count; i++)
				{
					if (this[i].ID > maxCpuId)
					{
						maxCpuId = this[i].ID;
					}
				}
			}
			return maxCpuId;
		}
	}

	private int MinCpuId
	{
		get
		{
			if (minCpuId == int.MaxValue)
			{
				for (int i = 0; i < Count; i++)
				{
					if (this[i].ID < minCpuId)
					{
						minCpuId = this[i].ID;
					}
				}
			}
			return minCpuId;
		}
	}

	public IEnumerable AffitinizedCPUs
	{
		get
		{
			for (int i = 0; i < cpuCol.Count; i++)
			{
				if (this[i].AffinityMask)
				{
					yield return cpuCol[i];
				}
			}
		}
	}

	internal CpuCollection(AffinityInfoBase parent)
	{
		this.parent = parent;
		cpuCollection = new NumaCPUCollectionBase<Cpu>(parent);
		iCol = cpuCollection;
		cpuCol = cpuCollection.cpuNumaCol;
		setByUser = false;
	}

	public void CopyTo(Array array, int index)
	{
		iCol.CopyTo(array, index);
	}

	public void CopyTo(Cpu[] array, int index)
	{
		iCol.CopyTo(array, index);
	}

	public IEnumerator GetEnumerator()
	{
		return cpuCollection.GetEnumerator();
	}

	public Cpu GetElementAt(int position)
	{
		return this[position];
	}

	public Cpu GetByID(int cpuId)
	{
		Cpu value = null;
		CpuCollectionFromId.TryGetValue(cpuId, out value);
		return value;
	}

	public void SetAffinityToAll(bool affinityMask)
	{
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Cpu cpu = (Cpu)enumerator.Current;
				cpu.AffinityMask = affinityMask;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void SetAffinityToRange(int startCpuId, int endCpuId, bool affinityMask)
	{
		SetAffinityToRange(startCpuId, endCpuId, affinityMask, ignoreMissingIds: false);
	}

	public void SetAffinityToRange(int startCpuId, int endCpuId, bool affinityMask, bool ignoreMissingIds)
	{
		if (startCpuId < MinCpuId || startCpuId > MaxCpuId)
		{
			throw new ArgumentOutOfRangeException("startCpuId");
		}
		if (endCpuId < MinCpuId || endCpuId > MaxCpuId)
		{
			throw new ArgumentOutOfRangeException("endCpuId");
		}
		if (startCpuId > endCpuId)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.WrongIndexRangeProvidedCPU(startCpuId, endCpuId));
		}
		for (int i = startCpuId; i <= endCpuId; i++)
		{
			if (CpuCollectionFromId.TryGetValue(i, out var value))
			{
				value.AffinityMask = affinityMask;
			}
			else if (!ignoreMissingIds)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.HoleInIndexRangeProvidedCPU(i));
			}
		}
	}

	internal StringCollection AddCpuInDdl(StringBuilder sb)
	{
		bool flag = false;
		int num = 0;
		int num2 = 0;
		bool flag2 = parent is ExternalResourcePoolAffinityInfo;
		sb.AppendFormat(SmoApplication.DefaultCulture, flag2 ? "CPU = (" : "CPU = ");
		for (int i = 0; i <= MaxCpuId; i++)
		{
			if (CpuCollectionFromId.TryGetValue(i, out var value) && value.AffinityMask)
			{
				num = i;
				num2 = num;
				while (CpuCollectionFromId.TryGetValue(++i, out value) && value.AffinityMask)
				{
				}
				num2 = --i;
				if (flag)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, ",");
				}
				else
				{
					flag = true;
				}
				if (num != num2)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "{0} TO {1}", new object[2] { num, num2 });
				}
				else
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { num });
				}
			}
		}
		if (!flag)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoCPUAffinitized);
		}
		if (flag2)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, ")");
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(sb.ToString());
		return stringCollection;
	}
}
