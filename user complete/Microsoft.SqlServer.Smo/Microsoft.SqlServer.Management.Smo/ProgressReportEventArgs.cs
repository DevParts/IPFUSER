using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ProgressReportEventArgs : EventArgs
{
	private bool schemaBound;

	private Urn current;

	private Urn parent;

	private int subTotalCount;

	private int subTotal;

	private int totalCount;

	private int total;

	public bool IsSchemaBound => schemaBound;

	public Urn Current => current;

	public Urn Parent => parent;

	public int SubTotalCount => subTotalCount;

	public int SubTotal => subTotal;

	public int TotalCount => totalCount;

	public int Total => total;

	public ProgressReportEventArgs(Urn current, Urn parent, int subTotalCount, int subTotal, int totalCount, int total)
	{
		this.current = current;
		this.parent = parent;
		schemaBound = false;
		this.subTotalCount = subTotalCount;
		this.subTotal = subTotal;
		this.totalCount = totalCount;
		this.total = total;
	}

	public ProgressReportEventArgs(Urn current, Urn parent, bool isSchemaBound, int subTotalCount, int subTotal, int totalCount, int total)
	{
		this.current = current;
		this.parent = parent;
		schemaBound = isSchemaBound;
		this.subTotalCount = subTotalCount;
		this.subTotal = subTotal;
		this.totalCount = totalCount;
		this.total = total;
	}
}
