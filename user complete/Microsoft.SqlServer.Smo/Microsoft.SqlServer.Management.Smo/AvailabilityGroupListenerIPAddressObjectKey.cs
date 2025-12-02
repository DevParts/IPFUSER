using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityGroupListenerIPAddressObjectKey : ObjectKeyBase
{
	internal static StringCollection fields;

	public string IPAddress { get; set; }

	public string SubnetMask { get; set; }

	public string SubnetIP { get; set; }

	public override string UrnFilter => string.Format(SmoApplication.DefaultCulture, "@IPAddress='{0}' and @SubnetMask='{1}' and @SubnetIP='{2}'", new object[3]
	{
		SqlSmoObject.SqlString(IPAddress),
		SqlSmoObject.SqlString(SubnetMask),
		SqlSmoObject.SqlString(SubnetIP)
	});

	public override bool IsNull
	{
		get
		{
			if (IPAddress != null && SubnetMask != null)
			{
				return null == SubnetIP;
			}
			return true;
		}
	}

	public AvailabilityGroupListenerIPAddressObjectKey()
	{
	}

	public AvailabilityGroupListenerIPAddressObjectKey(string ipAddress, string subnetMask, string subnetIP)
	{
		IPAddress = ipAddress;
		SubnetMask = subnetMask;
		SubnetIP = subnetIP;
	}

	static AvailabilityGroupListenerIPAddressObjectKey()
	{
		fields = new StringCollection();
		fields.Add("IPAddress");
		fields.Add("SubnetMask");
		fields.Add("SubnetIP");
	}

	public override StringCollection GetFieldNames()
	{
		return fields;
	}

	internal override void Validate(Type objectType)
	{
	}

	public override string GetExceptionName()
	{
		return string.Format(SmoApplication.DefaultCulture, "IPaddress {0} of Subnet mask {1} and SubnetIP {2}", new object[3] { IPAddress, SubnetMask, SubnetIP });
	}

	public override ObjectKeyBase Clone()
	{
		return new AvailabilityGroupListenerIPAddressObjectKey(IPAddress, SubnetMask, SubnetIP);
	}

	public override ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new AvailabilityGroupListenerIPAddressObjectComparer(stringComparer);
	}

	public override string ToString()
	{
		return string.Format(SmoApplication.DefaultCulture, "{0}/{1}/{2}", new object[3] { IPAddress, SubnetMask, SubnetIP });
	}
}
