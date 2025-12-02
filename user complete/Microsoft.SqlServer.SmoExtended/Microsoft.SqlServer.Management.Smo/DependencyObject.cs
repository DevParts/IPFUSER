using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Smo;

internal class DependencyObject
{
	private bool visited;

	private List<SqlSmoObject> ancestors;

	private List<SqlSmoObject> children;

	public bool Visited
	{
		get
		{
			return visited;
		}
		set
		{
			visited = value;
		}
	}

	public List<SqlSmoObject> Ancestors
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

	public List<SqlSmoObject> Children
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

	public void AddAncestor(SqlSmoObject obj)
	{
		if (Ancestors == null)
		{
			Ancestors = new List<SqlSmoObject>();
		}
		Ancestors.Add(obj);
	}

	public void AddChild(SqlSmoObject obj)
	{
		if (Children == null)
		{
			Children = new List<SqlSmoObject>();
		}
		Children.Add(obj);
	}
}
