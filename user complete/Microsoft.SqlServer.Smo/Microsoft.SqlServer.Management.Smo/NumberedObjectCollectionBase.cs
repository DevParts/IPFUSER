using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class NumberedObjectCollectionBase : SortedListCollectionBase
{
	internal NumberedObjectCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoSortedList(new NumberedObjectComparer());
	}

	public bool Contains(short number)
	{
		return Contains(new NumberedObjectKey(number));
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		short number = short.Parse(urn.GetAttribute("Number"), SmoApplication.DefaultCulture);
		return new NumberedObjectKey(number);
	}
}
