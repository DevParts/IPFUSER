using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class DomainRegistrationEncapsulation
{
	private static SfcDomainInfoCollection domains = null;

	private static SfcDomainInfo[] registeredDomains = new SfcDomainInfo[10]
	{
		new SfcDomainInfo("DMF", "Microsoft.SqlServer.Management.Dmf.PolicyStore", "Microsoft.SqlServer.Dmf", "SQLSERVER:\\SQLPolicy"),
		new SfcDomainInfo("DC", "Microsoft.SqlServer.Management.Collector.CollectorConfigStore", "Microsoft.SqlServer.Management.Collector", "SQLSERVER:\\DataCollection"),
		new SfcDomainInfo("XEvent", "Microsoft.SqlServer.Management.XEvent.XEStore", "Microsoft.SqlServer.Management.XEvent", "SQLSERVER:\\XEvent"),
		new SfcDomainInfo("DatabaseXEvent", "Microsoft.SqlServer.Management.XEventDbScoped.DatabaseXEStore", "Microsoft.SqlServer.Management.XEventDbScoped", "SQLSERVER:\\DatabaseXEvent"),
		new SfcDomainInfo("SMO", "Microsoft.SqlServer.Management.Smo.Server", "Microsoft.SqlServer.Smo", "SQLSERVER:\\SQL"),
		new SfcDomainInfo("RegisteredServers", "Microsoft.SqlServer.Management.RegisteredServers.RegisteredServersStore", "Microsoft.SqlServer.Management.RegisteredServers", "SQLSERVER:\\SQLRegistration"),
		new SfcDomainInfo("Utility", "Microsoft.SqlServer.Management.Utility.Utility", "Microsoft.SqlServer.Management.Utility", "SQLSERVER:\\Utility"),
		new SfcDomainInfo("SSIS", "Microsoft.SqlServer.Management.IntegrationServices.IntegrationServices", "Microsoft.SqlServer.Management.IntegrationServices", "SQLSERVER:\\SSIS"),
		new SfcDomainInfo("DependencyServices", "Microsoft.SqlServer.Management.DependencyServices.DependencyServicesStore", "Microsoft.SqlServer.Management.DependencyServices", "SQLSERVER:\\DependencyServices"),
		new SfcDomainInfo("DAC", "Microsoft.SqlServer.Management.Dac.DacDomain", "Microsoft.SqlServer.Management.Dac", "SQLSERVER:\\DAC")
	};

	private static SfcDomainInfo[] testDomains = new SfcDomainInfo[2]
	{
		new SfcDomainInfo("ACME", "Microsoft.SqlServer.Test.ManagementSDKTests.AcmePrototype.ACMEServer", "Microsoft.SqlServer.Test.ManagementSDKTests.AcmePrototype", null, assemblyInGAC: false),
		new SfcDomainInfo("FlashSFC", "Microsoft.SqlServer.Test.ManagementSDKTests.FlashSfc.FlashSfcServer", "Microsoft.SqlServer.Test.ManagementSDKTests.FlashSfc", null, assemblyInGAC: false)
	};

	internal static SfcDomainInfoCollection Domains
	{
		get
		{
			if (domains == null)
			{
				domains = new SfcDomainInfoCollection(registeredDomains);
			}
			return domains;
		}
	}

	internal static void AddTestDomainsToDomainsList()
	{
		List<SfcDomainInfo> list = new List<SfcDomainInfo>();
		list.AddRange(registeredDomains);
		list.AddRange(testDomains);
		domains = new SfcDomainInfoCollection(list);
	}
}
