using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class DepNode
{
	private SfcInstance sfcobj;

	private SfcKeyChain keychain;

	private List<DepNode> ancestors;

	private List<bool> physicalAncestorMask;

	private List<DepNode> children;

	private List<bool> physicalChildMask;

	private bool discovered;

	public SfcInstance Instance => sfcobj;

	internal SfcKeyChain Keychain => keychain;

	public List<DepNode> Ancestors
	{
		get
		{
			return ancestors;
		}
		set
		{
			ancestors = value;
		}
	}

	public List<bool> PhysicalAncestorMask
	{
		get
		{
			return physicalAncestorMask;
		}
		set
		{
			physicalAncestorMask = value;
		}
	}

	public int CountPhysicalAncestors
	{
		get
		{
			int num = 0;
			foreach (bool item in physicalAncestorMask)
			{
				if (item)
				{
					num++;
				}
			}
			return num;
		}
	}

	public List<DepNode> Children
	{
		get
		{
			return children;
		}
		set
		{
			children = value;
		}
	}

	public List<bool> PhysicalChildMask
	{
		get
		{
			return physicalChildMask;
		}
		set
		{
			physicalChildMask = value;
		}
	}

	public int CountPhysicalChildren
	{
		get
		{
			int num = 0;
			foreach (bool item in physicalChildMask)
			{
				if (item)
				{
					num++;
				}
			}
			return num;
		}
	}

	public bool Discovered
	{
		get
		{
			return discovered;
		}
		set
		{
			discovered = value;
		}
	}

	private DepNode()
	{
	}

	public DepNode(SfcInstance obj)
	{
		sfcobj = obj;
		keychain = obj.KeyChain;
	}

	internal DepNode(SfcKeyChain kc)
	{
		sfcobj = kc.GetObject();
		keychain = kc;
	}

	public void AddAncestor(DepNode node, bool isPhysicalRelation)
	{
		if (ancestors == null)
		{
			physicalAncestorMask = new List<bool>();
			ancestors = new List<DepNode>();
		}
		physicalAncestorMask.Add(isPhysicalRelation);
		ancestors.Add(node);
	}

	public void AddChild(DepNode node, bool isPhysicalRelation)
	{
		if (children == null)
		{
			physicalChildMask = new List<bool>();
			children = new List<DepNode>();
		}
		physicalChildMask.Add(isPhysicalRelation);
		children.Add(node);
	}

	public bool IsPhysicalAncestor(int i)
	{
		return physicalAncestorMask[i];
	}

	public void SetPhysicalAncestor(int i, bool b)
	{
		physicalAncestorMask[i] = b;
	}

	public bool IsPhysicalChild(int i)
	{
		return physicalChildMask[i];
	}

	public void SetPhysicalChild(int i, bool b)
	{
		physicalChildMask[i] = b;
	}
}
