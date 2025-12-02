using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class NumaNodeCollection : ICollection, IEnumerable
{
	private AffinityInfoBase parent;

	private NumaCPUCollectionBase<NumaNode> numaCollection;

	internal SortedList numaNodeCol;

	private ICollection iCol;

	private Dictionary<int, NumaNode> numaCollectionFromId;

	private int maxNumaId = -1;

	private int minNumaId = int.MaxValue;

	public int Count => iCol.Count;

	public bool IsSynchronized => iCol.IsSynchronized;

	public object SyncRoot => iCol.SyncRoot;

	public NumaNode this[int index] => numaCollection[index];

	private Dictionary<int, NumaNode> NumaCollectionFromId
	{
		get
		{
			if (numaCollectionFromId == null)
			{
				numaCollectionFromId = new Dictionary<int, NumaNode>();
				for (int i = 0; i < Count; i++)
				{
					numaCollectionFromId.Add(this[i].ID, this[i]);
				}
			}
			return numaCollectionFromId;
		}
	}

	private int MaxNumaId
	{
		get
		{
			if (maxNumaId == -1)
			{
				for (int i = 0; i < Count; i++)
				{
					if (this[i].ID > maxNumaId)
					{
						maxNumaId = this[i].ID;
					}
				}
			}
			return maxNumaId;
		}
	}

	private int MinNumaId
	{
		get
		{
			if (minNumaId == int.MaxValue)
			{
				for (int i = 0; i < Count; i++)
				{
					if (this[i].ID < minNumaId)
					{
						minNumaId = this[i].ID;
					}
				}
			}
			return minNumaId;
		}
	}

	internal NumaNodeCollection(AffinityInfoBase parent)
	{
		this.parent = parent;
		numaCollection = new NumaCPUCollectionBase<NumaNode>(parent);
		iCol = numaCollection;
		numaNodeCol = numaCollection.cpuNumaCol;
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
		return numaCollection.GetEnumerator();
	}

	public NumaNode GetElementAt(int position)
	{
		return this[position];
	}

	public NumaNode GetByID(int numanodeId)
	{
		NumaNode value = null;
		NumaCollectionFromId.TryGetValue(numanodeId, out value);
		return value;
	}

	public void SetAffinityToRange(int startNumaNodeId, int endNumaNodeId, NumaNodeAffinity affinityMask)
	{
		SetAffinityToRange(startNumaNodeId, endNumaNodeId, affinityMask, ignoreMissingIds: false);
	}

	public void SetAffinityToRange(int startNumaNodeId, int endNumaNodeId, NumaNodeAffinity affinityMask, bool ignoreMissingIds)
	{
		if (startNumaNodeId < MinNumaId || startNumaNodeId > MaxNumaId)
		{
			throw new ArgumentOutOfRangeException("startNumaNodeId");
		}
		if (endNumaNodeId < MinNumaId || endNumaNodeId > MaxNumaId)
		{
			throw new ArgumentOutOfRangeException("endNumaNodeId");
		}
		if (startNumaNodeId > endNumaNodeId)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.WrongIndexRangeProvidedNuma(startNumaNodeId, endNumaNodeId));
		}
		for (int i = startNumaNodeId; i <= endNumaNodeId; i++)
		{
			if (NumaCollectionFromId.TryGetValue(i, out var value))
			{
				value.AffinityMask = affinityMask;
			}
			else if (!ignoreMissingIds)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.HoleInIndexRangeProvidedNumaNode(i));
			}
		}
	}

	internal StringCollection AddNumaInDdl(StringBuilder stringBuilder)
	{
		int num = 0;
		int num2 = 0;
		bool flag = false;
		StringBuilder stringBuilder2 = new StringBuilder(stringBuilder.ToString());
		string value = stringBuilder2.ToString();
		bool flag2 = parent is ResourcePoolAffinityInfo || parent is ExternalResourcePoolAffinityInfo;
		int i;
		for (i = 0; i <= MaxNumaId; i++)
		{
			if (!NumaCollectionFromId.TryGetValue(i, out var value2))
			{
				continue;
			}
			if (value2.AffinityMask == NumaNodeAffinity.Full)
			{
				num = i;
				num2 = num;
				while (NumaCollectionFromId.TryGetValue(++i, out value2) && value2.AffinityMask == NumaNodeAffinity.Full)
				{
				}
				num2 = --i;
				if (flag)
				{
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, ",");
				}
				else
				{
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, flag2 ? "NUMANODE = (" : "NUMANODE = ");
					flag = true;
				}
				if (num != num2)
				{
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} TO {1}", new object[2] { num, num2 });
				}
				else
				{
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { num });
				}
			}
			else if (value2.AffinityMask.CompareTo(NumaNodeAffinity.Partial) == 0)
			{
				stringBuilder2 = new StringBuilder(value);
				break;
			}
		}
		if (i == MaxNumaId + 1)
		{
			if (!flag)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoCPUAffinitized);
			}
			if (flag2)
			{
				stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, ")");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(stringBuilder2.ToString());
			return stringCollection;
		}
		return null;
	}

	internal bool IsManuallySet()
	{
		bool flag = false;
		foreach (NumaNode item in numaCollection)
		{
			flag = flag || item.Cpus.setByUser;
		}
		return flag;
	}
}
