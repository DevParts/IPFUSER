namespace Microsoft.SqlServer.Management.Smo;

public sealed class Cpu
{
	private int id;

	private int numaNodeID;

	private int groupID;

	private bool affinityMask;

	private CpuCollection parent;

	public int ID => id;

	public int NumaNodeID => numaNodeID;

	public int GroupID => groupID;

	public bool AffinityMask
	{
		get
		{
			return affinityMask;
		}
		set
		{
			affinityMask = value;
			parent.setByUser = true;
		}
	}

	internal Cpu(int id, int numaNodeID, int groupID, bool affinityMask, CpuCollection parent)
	{
		this.id = id;
		this.numaNodeID = numaNodeID;
		this.groupID = groupID;
		this.affinityMask = affinityMask;
		this.parent = parent;
	}

	internal void InitAffinityMask(bool value)
	{
		affinityMask = value;
	}
}
