using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class AvailabilityGroupListenerIPAddressCollectionBase : SortedListCollectionBase
{
	internal AvailabilityGroupListenerIPAddressCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoSortedList(new AvailabilityGroupListenerIPAddressObjectComparer(StringComparer));
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(AvailabilityGroupListenerIPAddress);
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("IPAddress");
		string attribute2 = urn.GetAttribute("SubnetMask");
		string attribute3 = urn.GetAttribute("SubnetIP");
		return new AvailabilityGroupListenerIPAddressObjectKey(attribute, attribute2, attribute3);
	}
}
