using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Smo;

internal class DependencyObjects
{
	private Dictionary<SqlSmoObject, DependencyObject> nodeDict = new Dictionary<SqlSmoObject, DependencyObject>();

	private List<SqlSmoObject> dependencyList = new List<SqlSmoObject>();

	public void Add(SqlSmoObject node, SqlSmoObject dependent)
	{
		if (!nodeDict.TryGetValue(node, out var value))
		{
			value = new DependencyObject();
			nodeDict.Add(node, value);
		}
		value.AddChild(dependent);
		if (!nodeDict.TryGetValue(dependent, out value))
		{
			value = new DependencyObject();
			nodeDict.Add(dependent, value);
		}
		value.AddAncestor(node);
	}

	public void Add(SqlSmoObject node)
	{
		if (!nodeDict.TryGetValue(node, out var value))
		{
			value = new DependencyObject();
			nodeDict.Add(node, value);
		}
	}

	public void Clear()
	{
		nodeDict.Clear();
		dependencyList.Clear();
	}

	public List<SqlSmoObject> GetDependencies()
	{
		for (SqlSmoObject sqlSmoObject = StartNode(); sqlSmoObject != null; sqlSmoObject = StartNode())
		{
			VisitNode(sqlSmoObject);
		}
		dependencyList.Reverse();
		return dependencyList;
	}

	private SqlSmoObject StartNode()
	{
		foreach (KeyValuePair<SqlSmoObject, DependencyObject> item in nodeDict)
		{
			if (!item.Value.Visited && (item.Value.Ancestors == null || item.Value.Ancestors.Count == 0))
			{
				return item.Key;
			}
		}
		return null;
	}

	private void VisitNode(SqlSmoObject node)
	{
		DependencyObject dependencyObject = nodeDict[node];
		if (dependencyObject.Visited)
		{
			return;
		}
		dependencyObject.Visited = true;
		if (dependencyObject.Ancestors != null)
		{
			foreach (SqlSmoObject ancestor in dependencyObject.Ancestors)
			{
				VisitNode(ancestor);
			}
		}
		dependencyList.Add(node);
		if (dependencyObject.Children == null)
		{
			return;
		}
		foreach (SqlSmoObject child in dependencyObject.Children)
		{
			VisitNode(child);
		}
	}
}
