using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

public class DependencyWalker
{
	private Server server;

	private DependencyTree tree;

	private int totalCount;

	private int total;

	private Hashtable knownObjectsList;

	private ProgressReportEventHandler discoveryProgress;

	private ScriptingFilter filterCallbackFunction;

	public Server Server
	{
		get
		{
			return server;
		}
		set
		{
			if (value == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("Server"));
			}
			server = value;
		}
	}

	protected internal DependencyTree DependencyTree
	{
		get
		{
			return tree;
		}
		set
		{
			tree = value;
		}
	}

	protected internal int TotalCount
	{
		get
		{
			return totalCount;
		}
		set
		{
			totalCount = value;
		}
	}

	protected internal int Total
	{
		get
		{
			return total;
		}
		set
		{
			total = value;
		}
	}

	protected internal Hashtable KnownObjectsList
	{
		get
		{
			return knownObjectsList;
		}
		set
		{
			knownObjectsList = value;
		}
	}

	public ScriptingFilter FilterCallbackFunction
	{
		get
		{
			return filterCallbackFunction;
		}
		set
		{
			filterCallbackFunction = value;
		}
	}

	public event ProgressReportEventHandler DiscoveryProgress
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				discoveryProgress = (ProgressReportEventHandler)Delegate.Combine(discoveryProgress, value);
			}
		}
		remove
		{
			discoveryProgress = (ProgressReportEventHandler)Delegate.Remove(discoveryProgress, value);
		}
	}

	public DependencyWalker()
	{
		server = null;
	}

	public DependencyWalker(Server server)
	{
		if (server == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("server"));
		}
		Server = server;
	}

	protected Server GetServerObject()
	{
		if (Server == null)
		{
			throw new PropertyNotSetException("Server");
		}
		return Server;
	}

	public DependencyTree DiscoverDependencies(UrnCollection list, DependencyType dependencyType)
	{
		return DiscoverDependencies(list, dependencyType == DependencyType.Parents);
	}

	public DependencyTree DiscoverDependencies(UrnCollection list, bool parents)
	{
		Urn[] array = new Urn[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			array[i] = list[i];
		}
		return DiscoverDependencies(array, parents);
	}

	public DependencyTree DiscoverDependencies(SqlSmoObject[] objects, DependencyType dependencyType)
	{
		return DiscoverDependencies(objects, dependencyType == DependencyType.Parents);
	}

	public DependencyTree DiscoverDependencies(SqlSmoObject[] objects, bool parents)
	{
		Urn[] array = new Urn[objects.Length];
		for (int i = 0; i < objects.Length; i++)
		{
			SqlSmoObject sqlSmoObject = objects[i];
			if (sqlSmoObject.Properties.Contains("ID"))
			{
				Property property = sqlSmoObject.Properties.Get("ID");
				if (property.Value != null)
				{
					array[i] = sqlSmoObject.UrnWithId;
				}
				else
				{
					array[i] = sqlSmoObject.Urn;
				}
			}
			else
			{
				array[i] = sqlSmoObject.Urn;
			}
		}
		return DiscoverDependencies(array, parents);
	}

	public DependencyTree DiscoverDependencies(Urn[] urns, DependencyType dependencyType)
	{
		return DiscoverDependencies(urns, dependencyType == DependencyType.Parents);
	}

	public DependencyTree DiscoverDependencies(Urn[] urns, bool parents)
	{
		try
		{
			for (int i = 0; i < urns.Length; i++)
			{
				SqlSmoObject smoObject = GetServerObject().GetSmoObject(urns[i]);
				urns[i] = smoObject.Urn;
				string nameForType = urns[i].GetNameForType("Server");
				if (NetCoreHelpers.StringCompare(nameForType, GetServerObject().ExecutionManager.TrueServerName, ignoreCase: true, SmoApplication.DefaultCulture) != 0)
				{
					throw new ArgumentException(ExceptionTemplatesImpl.MismatchingServerName(GetServerObject().ExecutionManager.TrueServerName, nameForType));
				}
			}
			DependencyRequest dependencyRequest = new DependencyRequest();
			dependencyRequest.Urns = urns;
			dependencyRequest.ParentDependencies = parents;
			DependencyChainCollection dependencies = GetServerObject().ExecutionManager.GetDependencies(dependencyRequest);
			return new DependencyTree(urns, dependencies, parents, GetServerObject());
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DiscoverDependencies, this, ex);
		}
	}

	private bool ObjectEncounteredBefore(Urn newUrn)
	{
		return KnownObjectsList.ContainsKey(newUrn);
	}

	private void WalkDependentChildren(DependencyTree tree, DependencyTreeNode depParent, DependencyCollection depList)
	{
		int num = 0;
		for (DependencyTreeNode dependencyTreeNode = depParent.FirstChild; dependencyTreeNode != null; dependencyTreeNode = dependencyTreeNode.NextSibling)
		{
			if (!ObjectEncounteredBefore(dependencyTreeNode.Urn))
			{
				KnownObjectsList[dependencyTreeNode.Urn] = dependencyTreeNode.Urn;
				bool flag = false;
				for (DependencyTreeNode dependencyTreeNode2 = tree.FirstChild; dependencyTreeNode2 != null; dependencyTreeNode2 = dependencyTreeNode2.NextSibling)
				{
					if (dependencyTreeNode2.Urn == dependencyTreeNode.Urn)
					{
						flag = true;
						break;
					}
				}
				bool flag2 = false;
				if (FilterCallbackFunction != null)
				{
					flag2 = FilterCallbackFunction(dependencyTreeNode.Urn);
				}
				if (flag2)
				{
					Total--;
				}
				else
				{
					if (discoveryProgress != null)
					{
						num++;
						TotalCount++;
						discoveryProgress(this, new ProgressReportEventArgs(dependencyTreeNode.Urn, flag ? dependencyTreeNode.Urn : depParent.Urn, dependencyTreeNode.IsSchemaBound, num, dependencyTreeNode.NumberOfSiblings, TotalCount, Total));
					}
					if (dependencyTreeNode.HasChildNodes)
					{
						WalkDependentChildren(tree, dependencyTreeNode, depList);
					}
					depList.Add(new DependencyCollectionNode(dependencyTreeNode.Urn, dependencyTreeNode.IsSchemaBound, flag));
				}
			}
			else if (discoveryProgress != null)
			{
				num++;
			}
		}
	}

	public DependencyCollection WalkDependencies(DependencyTree tree)
	{
		DependencyTree = tree;
		TotalCount = 0;
		Total = tree.Count;
		KnownObjectsList = new Hashtable();
		DependencyCollection dependencyCollection = new DependencyCollection();
		WalkDependentChildren(tree, tree, dependencyCollection);
		DependencyTree = null;
		TotalCount = 0;
		Total = 0;
		KnownObjectsList = null;
		return dependencyCollection;
	}
}
