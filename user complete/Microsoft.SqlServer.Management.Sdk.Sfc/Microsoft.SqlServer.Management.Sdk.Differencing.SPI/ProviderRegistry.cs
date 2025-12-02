using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.SPI;

public class ProviderRegistry
{
	private static object monitor = new object();

	private List<SfcNodeAdapterProvider> navigators = new List<SfcNodeAdapterProvider>();

	private List<AvailablePropertyValueProvider> props = new List<AvailablePropertyValueProvider>();

	private List<NodeItemNamesAdapterProvider> names = new List<NodeItemNamesAdapterProvider>();

	private List<ContainerSortingProvider> sorters = new List<ContainerSortingProvider>();

	private List<PropertyComparerProvider> propComps = new List<PropertyComparerProvider>();

	public ICollection<SfcNodeAdapterProvider> SfcNodeAdapterProviders
	{
		get
		{
			lock (monitor)
			{
				return new List<SfcNodeAdapterProvider>(navigators);
			}
		}
	}

	public ICollection<AvailablePropertyValueProvider> AvailablePropertyValueProviders
	{
		get
		{
			lock (monitor)
			{
				return new List<AvailablePropertyValueProvider>(props);
			}
		}
	}

	public ICollection<PropertyComparerProvider> PropertyComparerProviders
	{
		get
		{
			lock (monitor)
			{
				return new List<PropertyComparerProvider>(propComps);
			}
		}
	}

	public ICollection<NodeItemNamesAdapterProvider> NodeItemNameAdapterProviders
	{
		get
		{
			lock (monitor)
			{
				return new List<NodeItemNamesAdapterProvider>(names);
			}
		}
	}

	public ICollection<ContainerSortingProvider> ContainerSortingProviders
	{
		get
		{
			lock (monitor)
			{
				return new List<ContainerSortingProvider>(sorters);
			}
		}
	}

	public bool RegisterProvider(SfcNodeAdapterProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		lock (monitor)
		{
			if (navigators.Contains(provider))
			{
				return false;
			}
			navigators.Insert(0, provider);
			return true;
		}
	}

	public bool UnregisterProvider(SfcNodeAdapterProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		lock (monitor)
		{
			if (!navigators.Contains(provider))
			{
				return false;
			}
			navigators.Remove(provider);
			return true;
		}
	}

	public bool RegisterProvider(AvailablePropertyValueProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		lock (monitor)
		{
			if (props.Contains(provider))
			{
				return false;
			}
			props.Insert(0, provider);
			return true;
		}
	}

	public bool UnregisterProvider(AvailablePropertyValueProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		lock (monitor)
		{
			if (!props.Contains(provider))
			{
				return false;
			}
			props.Remove(provider);
			return true;
		}
	}

	public bool RegisterProvider(PropertyComparerProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		lock (monitor)
		{
			if (propComps.Contains(provider))
			{
				return false;
			}
			propComps.Insert(0, provider);
			return true;
		}
	}

	public bool UnregisterProvider(PropertyComparerProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		lock (monitor)
		{
			if (!propComps.Contains(provider))
			{
				return false;
			}
			propComps.Remove(provider);
			return true;
		}
	}

	public bool RegisterProvider(NodeItemNamesAdapterProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		lock (monitor)
		{
			if (names.Contains(provider))
			{
				return false;
			}
			names.Insert(0, provider);
			return true;
		}
	}

	public bool UnregisterProvider(NodeItemNamesAdapterProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		lock (monitor)
		{
			if (!names.Contains(provider))
			{
				return false;
			}
			names.Remove(provider);
			return true;
		}
	}

	public bool RegisterProvider(ContainerSortingProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		lock (monitor)
		{
			if (sorters.Contains(provider))
			{
				return false;
			}
			sorters.Insert(0, provider);
			return true;
		}
	}

	public bool UnregisterProvider(ContainerSortingProvider provider)
	{
		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}
		lock (monitor)
		{
			if (!sorters.Contains(provider))
			{
				return false;
			}
			sorters.Remove(provider);
			return true;
		}
	}
}
