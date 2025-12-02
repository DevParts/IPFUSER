namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class DepStackNode
{
	public enum ProcessingState
	{
		OnSelf,
		OnAncestors,
		OnChildren
	}

	private DepNode node;

	private ProcessingState state;

	private int index;

	public DepNode Node
	{
		get
		{
			return node;
		}
		set
		{
			node = value;
		}
	}

	public ProcessingState State
	{
		get
		{
			return state;
		}
		set
		{
			state = value;
		}
	}

	public int Index
	{
		get
		{
			return index;
		}
		set
		{
			index = value;
		}
	}

	private DepStackNode()
	{
		node = null;
		state = ProcessingState.OnAncestors;
		index = 0;
	}

	public DepStackNode(DepNode node)
	{
		this.node = node;
		state = ProcessingState.OnAncestors;
		index = 0;
	}
}
