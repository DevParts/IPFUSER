using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.SqlServer.Management.Sdk.Differencing.SPI;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SmoCollectionSortingProvider : ContainerSortingProvider
{
	private static System.StringComparer DEFAULT_COMPARER = System.StringComparer.Ordinal;

	public override bool AreGraphsSupported(ISfcSimpleNode source, ISfcSimpleNode target)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (source.ObjectReference is SqlSmoObject && target.ObjectReference is SqlSmoObject)
		{
			return true;
		}
		return false;
	}

	public override IComparer<ISfcSimpleNode> GetComparer(ISfcSimpleList source, ISfcSimpleList target)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (!AreListsComparable(source, target))
		{
			throw new ArgumentException("The specified lists are not comparable.");
		}
		IEnumerator<ISfcSimpleNode> enumerator = source.GetEnumerator();
		IEnumerator<ISfcSimpleNode> enumerator2 = target.GetEnumerator();
		IComparer comparer = DEFAULT_COMPARER;
		if (enumerator.MoveNext() && enumerator2.MoveNext())
		{
			ISfcSimpleNode current = enumerator.Current;
			ISfcSimpleNode current2 = enumerator2.Current;
			if (current != null && current.ObjectReference is SqlSmoObject && current2 != null && current2.ObjectReference is SqlSmoObject)
			{
				SqlSmoObject sqlSmoObject = (SqlSmoObject)current.ObjectReference;
				SqlSmoObject sqlSmoObject2 = (SqlSmoObject)current2.ObjectReference;
				string parentCollation = GetParentCollation(sqlSmoObject);
				string parentCollation2 = GetParentCollation(sqlSmoObject2);
				if (string.IsNullOrEmpty(parentCollation) || string.IsNullOrEmpty(parentCollation2) || System.StringComparer.Ordinal.Compare(parentCollation, parentCollation2) == 0)
				{
					comparer = ((!sqlSmoObject.IsDesignMode && !string.IsNullOrEmpty(parentCollation)) ? sqlSmoObject.GetComparerFromCollation(parentCollation) : ((sqlSmoObject2.IsDesignMode || string.IsNullOrEmpty(parentCollation2)) ? ((IComparer)NetCoreHelpers.InvariantCulture.GetStringComparer(ignoreCase: false)) : ((IComparer)sqlSmoObject2.GetComparerFromCollation(parentCollation2))));
				}
			}
		}
		return new SmoCollectionCompararer(comparer);
	}

	private bool AreListsComparable(ISfcSimpleList source, ISfcSimpleList target)
	{
		IEnumerator<ISfcSimpleNode> enumerator = source.GetEnumerator();
		IEnumerator<ISfcSimpleNode> enumerator2 = target.GetEnumerator();
		bool result = false;
		if (enumerator.MoveNext() && enumerator2.MoveNext())
		{
			ISfcSimpleNode current = enumerator.Current;
			ISfcSimpleNode current2 = enumerator2.Current;
			if (current != null && current.ObjectReference is SqlSmoObject && current2 != null && current2.ObjectReference is SqlSmoObject)
			{
				result = true;
			}
		}
		return result;
	}

	private string GetParentCollation(SqlSmoObject obj)
	{
		object obj2 = obj;
		object obj3 = null;
		string result = string.Empty;
		while (obj3 == null)
		{
			Type type = obj2.GetType();
			PropertyInfo property = type.GetProperty("Parent");
			obj2 = property.GetValue(obj2, null);
			if (obj2.GetType() == typeof(Database))
			{
				obj3 = obj2;
				result = ((Database)obj3).Properties.GetPropertyObject("Collation", doNotLoadPropertyValues: true).Value as string;
			}
			else if (obj2.GetType() == typeof(Server))
			{
				obj3 = obj2;
				result = ((Server)obj3).Properties.GetPropertyObject("Collation", doNotLoadPropertyValues: true).Value as string;
			}
		}
		return result;
	}
}
