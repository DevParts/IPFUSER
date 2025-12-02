using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SchedulerCollection : ICollection, IEnumerable
{
	private AffinityInfoBase parent;

	private NumaCPUCollectionBase<Scheduler> schedulerCollection;

	internal SortedList schedulerList;

	private ICollection iCol;

	internal bool setByUser;

	private Dictionary<int, Scheduler> schedulerCollectionFromId;

	private int maxSchedulerId = -1;

	private int minSchedulerId = int.MaxValue;

	public int Count => iCol.Count;

	public bool IsSynchronized => iCol.IsSynchronized;

	public object SyncRoot => iCol.SyncRoot;

	public Scheduler this[int index] => schedulerCollection[index];

	private Dictionary<int, Scheduler> SchedulerCollectionFromId
	{
		get
		{
			if (schedulerCollectionFromId == null)
			{
				schedulerCollectionFromId = new Dictionary<int, Scheduler>();
				for (int i = 0; i < Count; i++)
				{
					schedulerCollectionFromId.Add(this[i].Id, this[i]);
				}
			}
			return schedulerCollectionFromId;
		}
	}

	private int MaxSchedulerId
	{
		get
		{
			if (maxSchedulerId == -1)
			{
				for (int i = 0; i < Count; i++)
				{
					if (this[i].Id > maxSchedulerId)
					{
						maxSchedulerId = this[i].Id;
					}
				}
			}
			return maxSchedulerId;
		}
	}

	private int MinSchedulerId
	{
		get
		{
			if (minSchedulerId == int.MaxValue)
			{
				for (int i = 0; i < Count; i++)
				{
					if (this[i].Id < minSchedulerId)
					{
						minSchedulerId = this[i].Id;
					}
				}
			}
			return minSchedulerId;
		}
	}

	internal SchedulerCollection(AffinityInfoBase parent)
	{
		this.parent = parent;
		schedulerCollection = new NumaCPUCollectionBase<Scheduler>(parent);
		schedulerList = schedulerCollection.cpuNumaCol;
		iCol = schedulerCollection;
	}

	public void CopyTo(Array array, int index)
	{
		iCol.CopyTo(array, index);
	}

	public void CopyTo(NumaNode[] array, int index)
	{
		iCol.CopyTo(array, index);
	}

	public IEnumerator GetEnumerator()
	{
		return schedulerCollection.GetEnumerator();
	}

	internal StringCollection AddSchedulerInDdl(StringBuilder sb)
	{
		bool flag = false;
		int num = 0;
		int num2 = 0;
		sb.AppendFormat(SmoApplication.DefaultCulture, "SCHEDULER = (");
		for (int i = 0; i <= MaxSchedulerId; i++)
		{
			if (SchedulerCollectionFromId.TryGetValue(i, out var value) && value.AffinityMask)
			{
				num = i;
				num2 = num;
				while (SchedulerCollectionFromId.TryGetValue(++i, out value) && value.AffinityMask)
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
		sb.Append(")");
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(sb.ToString());
		return stringCollection;
	}

	public Scheduler GetElementAt(int position)
	{
		return this[position];
	}

	public Scheduler GetByID(int SchedulernodeId)
	{
		Scheduler value = null;
		SchedulerCollectionFromId.TryGetValue(SchedulernodeId, out value);
		return value;
	}

	public void SetAffinityToRange(int startIndex, int endIndex, bool affinityMask)
	{
		if (startIndex < MinSchedulerId || startIndex > MaxSchedulerId)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		if (endIndex < MinSchedulerId || endIndex > MaxSchedulerId)
		{
			throw new ArgumentOutOfRangeException("endIndex");
		}
		if (startIndex > endIndex)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.WrongIndexRangeProvidedScheduler(startIndex, endIndex));
		}
		for (int i = startIndex; i <= endIndex; i++)
		{
			if (SchedulerCollectionFromId.TryGetValue(i, out var value))
			{
				value.AffinityMask = affinityMask;
				value.Cpu.AffinityMask = affinityMask;
				continue;
			}
			throw new FailedOperationException("Invalid Scheduler range with holes in it.");
		}
	}
}
