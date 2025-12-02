using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DependencyTree : DependencyTreeNode
{
	private DependencyChainCollection dependencies;

	private DependencyChainCollection roots;

	private bool dependsOnParents;

	private Server server;

	protected internal DependencyChainCollection Dependencies
	{
		get
		{
			return dependencies;
		}
		set
		{
			dependencies = value;
		}
	}

	protected internal DependencyChainCollection Roots
	{
		get
		{
			return roots;
		}
		set
		{
			roots = value;
		}
	}

	protected internal bool DependsOnParents
	{
		get
		{
			return dependsOnParents;
		}
		set
		{
			dependsOnParents = value;
		}
	}

	public override int NumberOfSiblings => 1;

	public int Count => Dependencies.Count;

	public override Urn Urn => null;

	public override bool HasChildNodes => true;

	public override DependencyTreeNode FirstChild => new DependencyTreeNode(0, Roots);

	public override DependencyTreeNode NextSibling => null;

	internal DependencyTree(Urn[] urns, DependencyChainCollection dependencies, bool fParents, Server server)
	{
		if (0 >= urns.Length)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.EmptyInputParam("urns", "Urn[]")).SetHelpContext("EmptyInputParam");
		}
		this.server = server;
		Dependencies = dependencies;
		DependsOnParents = fParents;
		IsSchemaBound = true;
		Roots = new DependencyChainCollection();
		ArrayList arrayList = new ArrayList(Dependencies.Count);
		for (int i = 0; i < Dependencies.Count; i++)
		{
			arrayList.Add(Dependencies[i]);
		}
		arrayList.Sort(new DependencyComparer(this.server));
		SortedList sortedList = new SortedList(new UrnComparer(this.server));
		for (int j = 0; j < Dependencies.Count; j++)
		{
			sortedList.Add((arrayList[j] as Dependency).Urn, arrayList[j]);
		}
		for (int k = 0; k < urns.Length; k++)
		{
			if (sortedList[urns[k]] is Dependency value)
			{
				Roots.Add(value);
			}
			if (Roots.Count <= k)
			{
				SqlSmoObject smoObject = this.server.GetSmoObject(urns[k]);
				if (smoObject.Properties.Contains("IsSystemObject") && smoObject.Properties["IsSystemObject"].Value != null && (bool)smoObject.Properties["IsSystemObject"].Value)
				{
					throw new FailedOperationException(ExceptionTemplatesImpl.NoDepForSysObjects(urns[k].ToString())).SetHelpContext("NoDepForSysObjects");
				}
				throw new FailedOperationException(ExceptionTemplatesImpl.UrnMissing(urns[k].ToString())).SetHelpContext("UrnMissing");
			}
		}
	}

	public DependencyTree(DependencyTree tree)
	{
		Dependencies = new DependencyChainCollection(tree.Dependencies);
		Roots = new DependencyChainCollection(tree.Roots);
		DependsOnParents = tree.DependsOnParents;
	}

	public DependencyTree Copy()
	{
		return new DependencyTree(this);
	}

	public void Remove(DependencyTreeNode depNode)
	{
		for (int i = 0; i < Dependencies.Count; i++)
		{
			Dependency dependency = Dependencies[i];
			if (depNode.Urn == dependency.Urn)
			{
				Dependencies.RemoveAt(i);
				i--;
				continue;
			}
			for (int j = 0; j < dependency.Links.Count; j++)
			{
				if (depNode.Urn == dependency.Links[j].Urn)
				{
					dependency.Links.RemoveAt(j);
					break;
				}
			}
		}
		for (int i = 0; i < Roots.Count; i++)
		{
			Dependency dependency = Roots[i];
			if (depNode.Urn == dependency.Urn)
			{
				Roots.RemoveAt(i);
				break;
			}
		}
	}
}
