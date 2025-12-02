using System;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Differencing.SPI;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SmoPropertyComparerProvider : PropertyComparerProvider
{
	public override bool AreGraphsSupported(ISfcSimpleNode left, ISfcSimpleNode right)
	{
		if (left == null)
		{
			throw new ArgumentNullException("left");
		}
		if (right == null)
		{
			throw new ArgumentNullException("right");
		}
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(left.ObjectReference != null, "Expect non-null left.ObjectReference");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(right.ObjectReference != null, "Expect non-null right.ObjectReference");
		if (left.ObjectReference is SqlSmoObject && right.ObjectReference is SqlSmoObject)
		{
			return true;
		}
		return false;
	}

	public override bool Compare(ISfcSimpleNode left, ISfcSimpleNode right, string propName)
	{
		if (left == null)
		{
			throw new ArgumentNullException("left");
		}
		if (right == null)
		{
			throw new ArgumentNullException("right");
		}
		if (propName == null)
		{
			throw new ArgumentNullException("propName");
		}
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(left.ObjectReference != null, "Expect non-null left.ObjectReference");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(right.ObjectReference != null, "Expect non-null right.ObjectReference");
		object left2 = left.Properties[propName];
		object right2 = right.Properties[propName];
		if (left.ObjectReference is Column && right.ObjectReference is Column)
		{
			Column column = (Column)left.ObjectReference;
			Column column2 = (Column)right.ObjectReference;
			if ("DataType".Equals(propName) && column.IsDesignMode != column2.IsDesignMode)
			{
				return CompareDataTypeWorkaround(column, column2);
			}
		}
		return CompareObjects(left2, right2);
	}

	private static bool CompareDataTypeWorkaround(Column leftCol, Column rightCol)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(leftCol != null, "Expect non-null leftCol");
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(rightCol != null, "Expect non-null rightCol");
		DataType dataType = null;
		DataType dataType2 = null;
		if (leftCol.IsDesignMode)
		{
			dataType = leftCol.DataType;
			dataType2 = rightCol.DataType;
		}
		else
		{
			dataType = rightCol.DataType;
			dataType2 = leftCol.DataType;
		}
		if (System.StringComparer.Ordinal.Compare(dataType.Name, dataType2.Name) != 0)
		{
			return false;
		}
		if (dataType.SqlDataType != dataType2.SqlDataType)
		{
			return false;
		}
		if (dataType.Schema != dataType2.Schema)
		{
			return false;
		}
		if (dataType.NumericPrecision != 0 && dataType.NumericPrecision != dataType2.NumericPrecision)
		{
			return false;
		}
		if (dataType.MaximumLength != 0 && dataType.MaximumLength != dataType2.MaximumLength)
		{
			return false;
		}
		if (dataType.NumericScale != 0 && dataType.NumericScale != dataType2.NumericScale)
		{
			return false;
		}
		return true;
	}

	private static bool CompareObjects(object left, object right)
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
}
