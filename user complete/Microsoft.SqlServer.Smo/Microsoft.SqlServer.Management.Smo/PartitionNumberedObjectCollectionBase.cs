using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class PartitionNumberedObjectCollectionBase : SortedListCollectionBase
{
	internal PartitionNumberedObjectCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoSortedList(new PartitionNumberedObjectComparer());
	}

	public bool Contains(int number)
	{
		return Contains(new PartitionNumberedObjectKey(number));
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		int number = short.Parse(urn.GetAttribute("PartitionNumber"), SmoApplication.DefaultCulture);
		return new PartitionNumberedObjectKey(number);
	}
}
