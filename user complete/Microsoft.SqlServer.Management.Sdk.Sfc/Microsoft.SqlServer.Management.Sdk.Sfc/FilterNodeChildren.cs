using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public abstract class FilterNodeChildren : FilterNode
{
	private FilterNode[] children;

	internal FilterNode[] Children => children ?? new FilterNode[0];

	internal FilterNodeChildren()
	{
	}

	internal FilterNodeChildren(FilterNode[] children)
	{
		this.children = children;
	}

	internal void Add(FilterNode x)
	{
		if (children == null)
		{
			children = new FilterNode[1] { x };
			return;
		}
		int num = children.Length;
		FilterNode[] sourceArray = children;
		children = new FilterNode[num + 1];
		Array.Copy(sourceArray, children, num);
		children[num] = x;
	}

	internal static bool Compare(FilterNodeChildren f1, FilterNodeChildren f2, CompareOptions compInfo, CultureInfo cultureInfo)
	{
		if (f1.Children.Length != f2.Children.Length)
		{
			return false;
		}
		for (int num = f1.Children.Length - 1; num >= 0; num--)
		{
			if (!FilterNode.Compare(f1.Children[num], f2.Children[num], compInfo, cultureInfo))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		int num = -1;
		for (int i = 0; i < children.Length; i++)
		{
			num ^= children[i].GetHashCode();
		}
		return num;
	}
}
