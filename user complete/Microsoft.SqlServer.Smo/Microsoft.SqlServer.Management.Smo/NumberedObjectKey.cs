using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal class NumberedObjectKey : ObjectKeyBase
{
	protected short number;

	internal static readonly StringCollection fields;

	public short Number
	{
		get
		{
			return number;
		}
		set
		{
			number = value;
		}
	}

	public override string UrnFilter => string.Format(SmoApplication.DefaultCulture, "@Number={0}", new object[1] { number });

	public override bool IsNull => false;

	public NumberedObjectKey(short number)
	{
		this.number = number;
	}

	static NumberedObjectKey()
	{
		fields = new StringCollection();
		fields.Add("Number");
	}

	public override string ToString()
	{
		return string.Format(SmoApplication.DefaultCulture, "{0}", new object[1] { number });
	}

	public override StringCollection GetFieldNames()
	{
		return fields;
	}

	public override ObjectKeyBase Clone()
	{
		return new NumberedObjectKey(Number);
	}

	public override ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new NumberedObjectComparer();
	}
}
