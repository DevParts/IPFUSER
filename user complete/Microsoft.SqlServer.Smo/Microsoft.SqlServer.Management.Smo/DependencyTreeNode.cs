using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DependencyTreeNode : DependencyNode
{
	private int index;

	private DependencyChainCollection siblings;

	protected internal int Index
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

	protected internal DependencyChainCollection Siblings
	{
		get
		{
			return siblings;
		}
		set
		{
			siblings = value;
		}
	}

	public virtual int NumberOfSiblings => Siblings.Count;

	public virtual bool HasChildNodes
	{
		get
		{
			Dependency dependency = Siblings[Index];
			return dependency.Links.Count > 0;
		}
	}

	public virtual DependencyTreeNode FirstChild
	{
		get
		{
			DependencyTreeNode result = null;
			Dependency dependency = Siblings[Index];
			if (dependency.Links.Count > 0)
			{
				result = new DependencyTreeNode(0, dependency.Links);
			}
			return result;
		}
	}

	public virtual DependencyTreeNode NextSibling
	{
		get
		{
			DependencyTreeNode result = null;
			if (Siblings.Count > Index + 1)
			{
				result = new DependencyTreeNode(Index + 1, Siblings);
			}
			return result;
		}
	}

	protected internal DependencyTreeNode()
	{
	}

	internal DependencyTreeNode(int index, DependencyChainCollection siblings)
	{
		if (0 > index)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (0 >= siblings.Count)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.EmptyInputParam("siblings", "DependencyChainCollection")).SetHelpContext("EmptyInputParam");
		}
		Index = index;
		Siblings = siblings;
		Dependency dependency = Siblings[Index];
		Urn = dependency.Urn;
		IsSchemaBound = dependency.IsSchemaBound;
	}
}
