using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
[StateChangeEvent("AUDIT_SERVER_OPERATION_EVENT", "SERVER")]
[StateChangeEvent("CREATE_ENDPOINT", "SERVER")]
[StateChangeEvent("ALTER_ENDPOINT", "SERVER")]
[StateChangeEvent("SAC_ENDPOINT_CHANGE", "SERVER")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.FacetSR")]
[DisplayNameKey("ServerSurfaceAreaConfigurationName")]
[DisplayDescriptionKey("ServerSurfaceAreaConfigurationDesc")]
public interface ISurfaceAreaFacet : IDmfFacet
{
	[DisplayNameKey("AdHocRemoteQueriesEnabledName")]
	[DisplayDescriptionKey("AdHocRemoteQueriesEnabledDesc")]
	bool AdHocRemoteQueriesEnabled { get; set; }

	[DisplayDescriptionKey("DatabaseMailEnabledDesc")]
	[DisplayNameKey("DatabaseMailEnabledName")]
	bool DatabaseMailEnabled { get; set; }

	[DisplayNameKey("ClrIntegrationEnabledName")]
	[DisplayDescriptionKey("ClrIntegrationEnabledDesc")]
	bool ClrIntegrationEnabled { get; set; }

	[DisplayNameKey("OleAutomationEnabledName")]
	[DisplayDescriptionKey("OleAutomationEnabledDesc")]
	bool OleAutomationEnabled { get; set; }

	[DisplayNameKey("RemoteDacEnabledName")]
	[DisplayDescriptionKey("RemoteDacEnabledDesc")]
	bool RemoteDacEnabled { get; set; }

	[DisplayDescriptionKey("SqlMailEnabledDesc")]
	[DisplayNameKey("SqlMailEnabledName")]
	bool SqlMailEnabled { get; set; }

	[DisplayDescriptionKey("WebAssistantEnabledDesc")]
	[DisplayNameKey("WebAssistantEnabledName")]
	bool WebAssistantEnabled { get; set; }

	[DisplayDescriptionKey("XPCmdShellEnabledDesc")]
	[DisplayNameKey("XPCmdShellEnabledName")]
	bool XPCmdShellEnabled { get; set; }

	[DisplayNameKey("ServiceBrokerEndpointActiveName")]
	[DisplayDescriptionKey("ServiceBrokerEndpointActiveDesc")]
	bool ServiceBrokerEndpointActive { get; set; }

	[DisplayDescriptionKey("SoapEndpointsEnabledDesc")]
	[DisplayNameKey("SoapEndpointsEnabledName")]
	bool SoapEndpointsEnabled { get; set; }
}
