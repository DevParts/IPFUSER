using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityGroupListenerIPAddressObjectComparer : ObjectComparerBase
{
	public AvailabilityGroupListenerIPAddressObjectComparer(IComparer stringComparer)
		: base(stringComparer)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		AvailabilityGroupListenerIPAddressObjectKey availabilityGroupListenerIPAddressObjectKey = obj1 as AvailabilityGroupListenerIPAddressObjectKey;
		AvailabilityGroupListenerIPAddressObjectKey availabilityGroupListenerIPAddressObjectKey2 = obj2 as AvailabilityGroupListenerIPAddressObjectKey;
		if (availabilityGroupListenerIPAddressObjectKey == null && availabilityGroupListenerIPAddressObjectKey2 == null)
		{
			return 0;
		}
		if (availabilityGroupListenerIPAddressObjectKey == null)
		{
			return -1;
		}
		if (availabilityGroupListenerIPAddressObjectKey2 == null)
		{
			return 1;
		}
		int num = stringComparer.Compare(availabilityGroupListenerIPAddressObjectKey.IPAddress, availabilityGroupListenerIPAddressObjectKey2.IPAddress);
		if (num == 0)
		{
			int num2 = stringComparer.Compare(availabilityGroupListenerIPAddressObjectKey.SubnetMask, availabilityGroupListenerIPAddressObjectKey2.SubnetMask);
			if (num2 == 0)
			{
				return stringComparer.Compare(availabilityGroupListenerIPAddressObjectKey.SubnetIP, availabilityGroupListenerIPAddressObjectKey2.SubnetIP);
			}
			return num2;
		}
		return num;
	}
}
