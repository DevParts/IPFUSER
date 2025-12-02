using System;
using Microsoft.SqlServer.Management.Sdk.Differencing.Impl;
using Microsoft.SqlServer.Management.Sdk.Differencing.SPI;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing;

public class DifferencingService
{
	private const string ASSEMBLY_NAME = "Microsoft.SqlServer.Smo";

	private const string PROVIDER_NAME_AVALIABLE_VALUE_0 = "Microsoft.SqlServer.Management.Smo.SmoAvaliablePropertyProvider";

	private const string PROVIDER_NAME_AVALIABLE_VALUE_1 = "Microsoft.SqlServer.Management.Smo.OnlineSmoAvailablePropertyProvider";

	private const string PROVIDER_NAME_NODE_ADAPTER_0 = "Microsoft.SqlServer.Management.Smo.SmoNodeAdapterProvider";

	private const string PROVIDER_NAME_COLLECTION_SORTER_0 = "Microsoft.SqlServer.Management.Smo.SmoCollectionSortingProvider";

	private const string PROVIDER_NAME_PROP_COMPARER_0 = "Microsoft.SqlServer.Management.Smo.SmoPropertyComparerProvider";

	private static readonly DifferencingService Singleton = new DifferencingService();

	public static DifferencingService Service => Singleton;

	private DifferencingService()
	{
	}

	private static void RegisterProvider(ProviderRegistry myRegistry, string assemblyName, string name)
	{
		Provider provider = CreateProviderInstance(assemblyName, name);
		if (provider != null)
		{
			RegisterProvider(myRegistry, provider);
		}
	}

	private static void RegisterProvider(ProviderRegistry myRegistry, Provider provider)
	{
		if (provider is SfcNodeAdapterProvider)
		{
			myRegistry.RegisterProvider(provider as SfcNodeAdapterProvider);
			return;
		}
		if (provider is NodeItemNamesAdapterProvider)
		{
			myRegistry.RegisterProvider(provider as NodeItemNamesAdapterProvider);
			return;
		}
		if (provider is ContainerSortingProvider)
		{
			myRegistry.RegisterProvider(provider as ContainerSortingProvider);
			return;
		}
		if (provider is AvailablePropertyValueProvider)
		{
			myRegistry.RegisterProvider(provider as AvailablePropertyValueProvider);
			return;
		}
		if (provider is PropertyComparerProvider)
		{
			myRegistry.RegisterProvider(provider as PropertyComparerProvider);
			return;
		}
		TraceHelper.Trace("Differencing", "The type of provider, '{0}', is not in recognized.", provider.GetType().Name);
	}

	private static Provider CreateProviderInstance(string assemblyName, string name)
	{
		try
		{
			object obj = ObjectCache.CreateObjectInstance(assemblyName, name);
			if (obj is Provider)
			{
				return obj as Provider;
			}
			TraceHelper.Trace("Differencing", "The type of provider, '{0}', is not in recognized.", name);
			return null;
		}
		catch (Exception ex)
		{
			if (Differencer.IsSystemGeneratedException(ex))
			{
				throw ex;
			}
			TraceHelper.Trace("Differencing", "Exception loading provider, '{0}'.", name);
			return null;
		}
	}

	public IDifferencer CreateDifferencer()
	{
		return new Differencer(CreateDefaultRegistry());
	}

	public IDifferencer CreateDifferencer(ProviderRegistry registry)
	{
		if (registry == null)
		{
			throw new ArgumentNullException("register");
		}
		return new Differencer(registry);
	}

	public ProviderRegistry CreateDefaultRegistry()
	{
		ProviderRegistry providerRegistry = new ProviderRegistry();
		RegisterProvider(providerRegistry, new MetadataNodeItemNamesProvider());
		RegisterProvider(providerRegistry, "Microsoft.SqlServer.Smo", "Microsoft.SqlServer.Management.Smo.SmoAvaliablePropertyProvider");
		RegisterProvider(providerRegistry, "Microsoft.SqlServer.Smo", "Microsoft.SqlServer.Management.Smo.OnlineSmoAvailablePropertyProvider");
		RegisterProvider(providerRegistry, "Microsoft.SqlServer.Smo", "Microsoft.SqlServer.Management.Smo.SmoNodeAdapterProvider");
		RegisterProvider(providerRegistry, "Microsoft.SqlServer.Smo", "Microsoft.SqlServer.Management.Smo.SmoCollectionSortingProvider");
		RegisterProvider(providerRegistry, "Microsoft.SqlServer.Smo", "Microsoft.SqlServer.Management.Smo.SmoPropertyComparerProvider");
		return providerRegistry;
	}
}
