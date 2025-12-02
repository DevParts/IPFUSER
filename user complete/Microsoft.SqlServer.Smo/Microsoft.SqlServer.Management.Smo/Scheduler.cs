namespace Microsoft.SqlServer.Management.Smo;

public sealed class Scheduler
{
	private int id;

	private Cpu cpu;

	private bool affinityMask;

	private SchedulerCollection parent;

	public int Id => id;

	public Cpu Cpu => cpu;

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

	internal Scheduler(int id, Cpu cpu, bool affinityMask, SchedulerCollection parent)
	{
		this.id = id;
		this.cpu = cpu;
		this.affinityMask = affinityMask;
		this.parent = parent;
	}
}
