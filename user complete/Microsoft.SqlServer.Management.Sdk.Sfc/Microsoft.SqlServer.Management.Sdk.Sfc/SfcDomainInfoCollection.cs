using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcDomainInfoCollection : ReadOnlyCollection<SfcDomainInfo>
{
	public SfcDomainInfo this[string domainName]
	{
		get
		{
			foreach (SfcDomainInfo item in base.Items)
			{
				if (string.Compare(item.Name, domainName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return item;
				}
			}
			throw new SfcMetadataException(SfcStrings.DomainNotFound(domainName));
		}
	}

	public SfcDomainInfoCollection(IList<SfcDomainInfo> list)
		: base(list)
	{
	}

	public bool Contains(string domainName)
	{
		foreach (SfcDomainInfo item in base.Items)
		{
			if (string.Compare(item.Name, domainName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
		}
		return false;
	}

	public SfcDomainInfo GetDomainForNamespaceQualifier(string namespaceQualifier)
	{
		foreach (SfcDomainInfo item in base.Items)
		{
			if (string.Compare(item.NamespaceQualifier, namespaceQualifier, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return item;
			}
		}
		throw new SfcMetadataException(SfcStrings.DomainNotFound(namespaceQualifier));
	}
}
