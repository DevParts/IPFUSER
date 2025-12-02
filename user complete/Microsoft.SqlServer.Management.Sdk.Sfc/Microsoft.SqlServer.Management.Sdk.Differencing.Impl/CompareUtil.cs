using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.Impl;

internal class CompareUtil
{
	public static int CompareUrns(Urn left, Urn right)
	{
		if (left == null && right == null)
		{
			return 0;
		}
		if (left == null)
		{
			return -1;
		}
		if (right == null)
		{
			return 1;
		}
		int num = CompareUrns(left.Parent, right.Parent);
		if (num != 0)
		{
			return num;
		}
		return CompareStrings(left.Value, right.Value);
	}

	public static int CompareUrnLeaves(Urn left, Urn right)
	{
		if (left == null && right == null)
		{
			return 0;
		}
		if (left == null)
		{
			return -1;
		}
		if (right == null)
		{
			return 1;
		}
		return CompareStrings(left.Value, right.Value);
	}

	public static bool CompareObjects(object left, object right)
	{
		if (left == null && right == null)
		{
			return true;
		}
		if (left == null)
		{
			return false;
		}
		if (right == null)
		{
			return false;
		}
		return left.Equals(right);
	}

	public static int CompareStrings(string left, string right)
	{
		if (left == null && right == null)
		{
			return 0;
		}
		if (left == null)
		{
			return -1;
		}
		if (right == null)
		{
			return 1;
		}
		return StringComparer.Ordinal.Compare(left, right);
	}
}
