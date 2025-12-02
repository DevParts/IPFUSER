namespace Microsoft.SqlServer.Management.Smo;

public sealed class NumaNode
{
	private int id;

	private int groupID;

	private AffinityInfoBase affInfo;

	private CpuCollection cpuCol;

	public CpuCollection Cpus
	{
		get
		{
			if (cpuCol == null)
			{
				cpuCol = new CpuCollection(affInfo);
			}
			return cpuCol;
		}
	}

	public int ID => id;

	public int GroupID => groupID;

	public NumaNodeAffinity AffinityMask
	{
		get
		{
			int num = 0;
			foreach (Cpu cpu in Cpus)
			{
				if (!cpu.AffinityMask)
				{
					num++;
				}
			}
			if (num == 0)
			{
				return NumaNodeAffinity.Full;
			}
			if (num == Cpus.Count)
			{
				return NumaNodeAffinity.None;
			}
			return NumaNodeAffinity.Partial;
		}
		set
		{
			switch (value)
			{
			case NumaNodeAffinity.Full:
			{
				foreach (Cpu cpu3 in Cpus)
				{
					cpu3.AffinityMask = true;
				}
				return;
			}
			case NumaNodeAffinity.Partial:
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.AffinityValueCannotBeSet);
			}
			foreach (Cpu cpu4 in Cpus)
			{
				cpu4.AffinityMask = false;
			}
		}
	}

	internal NumaNode(int id, int groupID, AffinityInfoBase parent)
	{
		this.id = id;
		this.groupID = groupID;
		affInfo = parent;
	}
}
