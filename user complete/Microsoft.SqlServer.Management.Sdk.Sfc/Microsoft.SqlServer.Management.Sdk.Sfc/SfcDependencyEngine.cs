using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public sealed class SfcDependencyEngine : ISfcDependencyDiscoveryObjectSink, IDisposable
{
	public class DependencyListEnumerator : IEnumerator<SfcDependencyNode>, IDisposable, IEnumerator, IEnumerable<SfcDependencyNode>, IEnumerable
	{
		private Stack<DepStackNode> visitStack;

		private DepNode curNode;

		private SfcDependencyEngine depEngine;

		private List<DepNode> startNodes;

		private Dictionary<SfcKeyChain, bool> visited;

		public SfcDependencyNode Current => new SfcDependencyNode(curNode);

		object IEnumerator.Current => new SfcDependencyNode(curNode);

		internal DependencyListEnumerator(SfcDependencyEngine depEngine)
		{
			this.depEngine = depEngine;
			Reset();
		}

		public void Dispose()
		{
			visitStack = null;
			curNode = null;
			depEngine = null;
			startNodes = null;
			visited = null;
		}

		public bool MoveNext()
		{
			curNode = null;
			do
			{
				if (visitStack.Count == 0)
				{
					DepNode depNode = null;
					foreach (DepNode startNode in startNodes)
					{
						if (!visited.ContainsKey(startNode.Keychain))
						{
							depNode = startNode;
							break;
						}
					}
					if (depNode == null)
					{
						return false;
					}
					visitStack.Push(new DepStackNode(depNode));
				}
				DepStackNode depStackNode = visitStack.Peek();
				DepNode node = depStackNode.Node;
				switch (depStackNode.State)
				{
				case DepStackNode.ProcessingState.OnAncestors:
				{
					bool flag2 = false;
					if (node.Ancestors != null)
					{
						int count2 = node.Ancestors.Count;
						while (depStackNode.Index < count2)
						{
							DepNode depNode3 = node.Ancestors[depStackNode.Index];
							depStackNode.Index++;
							if (!visited.ContainsKey(depNode3.Keychain))
							{
								visitStack.Push(new DepStackNode(depNode3));
								visited.Add(depNode3.Keychain, value: true);
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						depStackNode.Index = 0;
						depStackNode.State = DepStackNode.ProcessingState.OnSelf;
					}
					break;
				}
				case DepStackNode.ProcessingState.OnSelf:
					curNode = depStackNode.Node;
					depStackNode.State = DepStackNode.ProcessingState.OnChildren;
					if (!visited.ContainsKey(depStackNode.Node.Keychain))
					{
						visited.Add(depStackNode.Node.Keychain, value: true);
					}
					break;
				case DepStackNode.ProcessingState.OnChildren:
				{
					bool flag = false;
					if (node.Children != null)
					{
						int count = node.Children.Count;
						while (depStackNode.Index < count)
						{
							DepNode depNode2 = node.Children[depStackNode.Index];
							depStackNode.Index++;
							if (!visited.ContainsKey(depNode2.Keychain))
							{
								visitStack.Push(new DepStackNode(depNode2));
								visited.Add(depNode2.Keychain, value: true);
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						visitStack.Pop();
					}
					break;
				}
				}
			}
			while (curNode == null);
			return true;
		}

		public void Reset()
		{
			visitStack = new Stack<DepStackNode>();
			curNode = null;
			startNodes = depEngine.FindUnparentedNodes();
			visited = new Dictionary<SfcKeyChain, bool>();
		}

		IEnumerator<SfcDependencyNode> IEnumerable<SfcDependencyNode>.GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}
	}

	public class DependencyTreeEnumerator : IEnumerator<SfcDependencyNode>, IDisposable, IEnumerator, IEnumerable<SfcDependencyNode>, IEnumerable
	{
		private SfcDependencyEngine depEngine;

		private IEnumerator<DepNode> nodeEnumerator;

		SfcDependencyNode IEnumerator<SfcDependencyNode>.Current => new SfcDependencyNode(nodeEnumerator.Current);

		object IEnumerator.Current => ((IEnumerator<SfcDependencyNode>)this).Current;

		internal DependencyTreeEnumerator(SfcDependencyEngine depEngine)
		{
			this.depEngine = depEngine;
			nodeEnumerator = depEngine.FindPhysicallyUnparentedNodes().GetEnumerator();
			Reset();
		}

		public void Dispose()
		{
			depEngine = null;
			nodeEnumerator = null;
		}

		public bool MoveNext()
		{
			return nodeEnumerator.MoveNext();
		}

		public void Reset()
		{
			nodeEnumerator.Reset();
		}

		IEnumerator<SfcDependencyNode> IEnumerable<SfcDependencyNode>.GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}
	}

	public class DependencyNodeEnumerator : IEnumerator<SfcDependencyNode>, IDisposable, IEnumerator, IEnumerable<SfcDependencyNode>, IEnumerable
	{
		private IEnumerator<DepNode> nodeEnumerator;

		private List<bool> physicalMask;

		private int index;

		SfcDependencyNode IEnumerator<SfcDependencyNode>.Current => new SfcDependencyNode(nodeEnumerator.Current);

		object IEnumerator.Current => ((IEnumerator<SfcDependencyNode>)this).Current;

		internal DependencyNodeEnumerator(IEnumerator<DepNode> nodeEnumerator)
		{
			this.nodeEnumerator = nodeEnumerator;
			physicalMask = null;
			Reset();
		}

		internal DependencyNodeEnumerator(IEnumerator<DepNode> nodeEnumerator, List<bool> physicalMask)
		{
			this.nodeEnumerator = nodeEnumerator;
			this.physicalMask = physicalMask;
			Reset();
		}

		public void Dispose()
		{
			nodeEnumerator = null;
		}

		public bool MoveNext()
		{
			if (physicalMask == null)
			{
				return nodeEnumerator.MoveNext();
			}
			while (nodeEnumerator.MoveNext())
			{
				if (physicalMask[index])
				{
					index++;
					return true;
				}
				index++;
			}
			return false;
		}

		public void Reset()
		{
			nodeEnumerator.Reset();
			index = 0;
		}

		IEnumerator<SfcDependencyNode> IEnumerable<SfcDependencyNode>.GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}
	}

	private SfcDependencyDiscoveryMode m_mode;

	private SfcDependencyAction m_action;

	private SfcDependencyRootList m_DependencyRootList = new SfcDependencyRootList();

	private NodeGraph nodeDict = new NodeGraph();

	private Queue<SfcKeyChain> nodeQueue = new Queue<SfcKeyChain>();

	private DepNode m_currentNode;

	public SfcDependencyAction Action => m_action;

	internal SfcDependencyRootList SfcDependencyRootList
	{
		get
		{
			return new SfcDependencyRootList(m_DependencyRootList);
		}
		set
		{
			m_DependencyRootList = new SfcDependencyRootList(value);
		}
	}

	private SfcDependencyNode this[SfcKeyChain kc]
	{
		get
		{
			if (!nodeDict.TryGetValue(kc, out var node))
			{
				return null;
			}
			return new SfcDependencyNode(node);
		}
	}

	public SfcDependencyEngine(SfcDependencyDiscoveryMode mode, SfcDependencyAction action)
	{
		m_mode = mode;
		m_action = action;
	}

	private List<DepNode> FindUnparentedNodes()
	{
		List<DepNode> list = new List<DepNode>();
		foreach (DepNode item in nodeDict)
		{
			if (item.Ancestors == null || item.Ancestors.Count == 0)
			{
				list.Add(item);
			}
		}
		return list;
	}

	private List<DepNode> FindPhysicallyUnparentedNodes()
	{
		List<DepNode> list = new List<DepNode>();
		foreach (DepNode item in nodeDict)
		{
			if (item.Ancestors == null || item.Ancestors.Count == 0 || item.CountPhysicalAncestors == 0)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public void Add(SfcInstance objParent, SfcInstance objChild, SfcTypeRelation relation)
	{
		if (objParent == null || objChild == null)
		{
			return;
		}
		if (!nodeDict.TryGetValue(objParent.KeyChain, out var node))
		{
			node = new DepNode(objParent);
			nodeDict.Add(node);
			if (m_currentNode == null || m_currentNode.Keychain != objParent.KeyChain)
			{
				nodeQueue.Enqueue(objParent.KeyChain);
			}
		}
		if (!nodeDict.TryGetValue(objChild.KeyChain, out var node2))
		{
			node2 = new DepNode(objChild);
			nodeDict.Add(node2);
			if (m_currentNode == null || m_currentNode.Keychain != objChild.KeyChain)
			{
				nodeQueue.Enqueue(objChild.KeyChain);
			}
		}
		bool isPhysicalRelation = relation == SfcTypeRelation.ContainedChild || relation == SfcTypeRelation.RequiredChild;
		if (relation != SfcTypeRelation.WeakReference)
		{
			node2.AddAncestor(node, isPhysicalRelation);
			node.AddChild(node2, isPhysicalRelation);
		}
	}

	public void Add(SfcInstance obj)
	{
		if (!nodeDict.TryGetValue(obj.KeyChain, out var node))
		{
			node = new DepNode(obj);
			nodeDict.Add(node);
			if (m_currentNode == null || m_currentNode.Keychain != obj.KeyChain)
			{
				nodeQueue.Enqueue(obj.KeyChain);
			}
		}
	}

	public void Discover()
	{
		foreach (SfcInstance dependencyRoot in m_DependencyRootList)
		{
			Add(dependencyRoot);
		}
		Dictionary<ISfcDomain, List<SfcDependencyNode>> dictionary = new Dictionary<ISfcDomain, List<SfcDependencyNode>>();
		List<SfcDependencyNode> value = null;
		ISfcDomain sfcDomain = null;
		foreach (DepNode item in nodeDict)
		{
			if (value == null || sfcDomain != item.Keychain.Domain)
			{
				if (!dictionary.TryGetValue(item.Keychain.Domain, out value))
				{
					value = new List<SfcDependencyNode>();
					dictionary.Add(item.Keychain.Domain, value);
				}
				sfcDomain = item.Keychain.Domain;
			}
			value.Add(new SfcDependencyNode(item));
		}
		while (nodeQueue.Count > 0)
		{
			SfcKeyChain kc = nodeQueue.Dequeue();
			if (!nodeDict.TryGetValue(kc, out var node) || node.Discovered)
			{
				continue;
			}
			SfcInstance instance = node.Instance;
			if (instance != null)
			{
				ISfcDiscoverObject sfcDiscoverObject = instance;
				if (sfcDiscoverObject != null)
				{
					m_currentNode = node;
					sfcDiscoverObject.Discover(this);
					m_currentNode = null;
					node.Discovered = true;
				}
			}
		}
	}

	public void Dispose()
	{
		nodeDict.Clear();
		nodeQueue.Clear();
		m_DependencyRootList.Clear();
		m_currentNode = null;
	}

	void ISfcDependencyDiscoveryObjectSink.Add(SfcDependencyDirection direction, SfcInstance targetObject, SfcTypeRelation relation, bool discovered)
	{
		if (direction == SfcDependencyDirection.Inbound)
		{
			Add(m_currentNode.Instance, targetObject, relation);
		}
		else
		{
			Add(targetObject, m_currentNode.Instance, relation);
		}
		if (discovered)
		{
			nodeDict[targetObject.KeyChain].Discovered = true;
		}
	}

	void ISfcDependencyDiscoveryObjectSink.Add(SfcDependencyDirection direction, IEnumerator targetObjects, SfcTypeRelation relation, bool discovered)
	{
		while (targetObjects.MoveNext())
		{
			SfcInstance targetObject = (SfcInstance)targetObjects.Current;
			((ISfcDependencyDiscoveryObjectSink)this).Add(direction, targetObject, relation, discovered);
		}
	}

	void ISfcDependencyDiscoveryObjectSink.Add<T>(SfcDependencyDirection direction, IEnumerable<T> targetObjects, SfcTypeRelation relation, bool discovered)
	{
		foreach (T targetObject in targetObjects)
		{
			((ISfcDependencyDiscoveryObjectSink)this).Add(direction, (SfcInstance)targetObject, relation, discovered);
		}
	}

	public DependencyListEnumerator GetListEnumerator()
	{
		return new DependencyListEnumerator(this);
	}

	public DependencyTreeEnumerator GetTreeEnumerator()
	{
		return new DependencyTreeEnumerator(this);
	}
}
