using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal class PartitionNumberedObjectKey : ObjectKeyBase
{
	protected int number;

	internal static StringCollection fields;

	public int Number
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

	public override string UrnFilter => string.Format(SmoApplication.DefaultCulture, "@PartitionNumber={0}", new object[1] { number });

	public override bool IsNull => false;

	public PartitionNumberedObjectKey(int number)
	{
		this.number = number;
	}

	static PartitionNumberedObjectKey()
	{
		fields = new StringCollection();
		fields.Add("PartitionNumber");
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
		return new PartitionNumberedObjectKey(Number);
	}

	public override ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new PartitionNumberedObjectComparer();
	}
}
