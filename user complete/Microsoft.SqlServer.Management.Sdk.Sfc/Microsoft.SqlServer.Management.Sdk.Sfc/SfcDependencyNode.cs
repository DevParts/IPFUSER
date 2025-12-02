using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public sealed class SfcDependencyNode
{
	private DepNode depNode;

	internal SfcKeyChain SfcKeyChain => depNode.Keychain;

	public SfcInstance Instance => depNode.Instance;

	public bool Discovered
	{
		get
		{
			return depNode.Discovered;
		}
		set
		{
			depNode.Discovered = value;
		}
	}

	public IEnumerable<SfcDependencyNode> Children
	{
		get
		{
			if (depNode.Children == null || depNode.Children.Count == 0)
			{
				return null;
			}
			return new SfcDependencyEngine.DependencyNodeEnumerator(depNode.Children.GetEnumerator());
		}
	}

	public int ChildCount
	{
		get
		{
			if (depNode.Children != null)
			{
				return depNode.Children.Count;
			}
			return 0;
		}
	}

	public IEnumerable<SfcDependencyNode> Ancestors
	{
		get
		{
			if (depNode.Ancestors == null || depNode.Ancestors.Count == 0)
			{
				return null;
			}
			return new SfcDependencyEngine.DependencyNodeEnumerator(depNode.Ancestors.GetEnumerator());
		}
	}

	public int AncestorCount
	{
		get
		{
			if (depNode.Ancestors != null)
			{
				return depNode.Ancestors.Count;
			}
			return 0;
		}
	}

	internal SfcDependencyNode(DepNode depNode)
	{
		this.depNode = depNode;
	}

	public bool IsPhysicalAncestor(int index)
	{
		return depNode.IsPhysicalAncestor(index);
	}

	public bool IsPhysicalChild(int index)
	{
		return depNode.IsPhysicalChild(index);
	}
}
