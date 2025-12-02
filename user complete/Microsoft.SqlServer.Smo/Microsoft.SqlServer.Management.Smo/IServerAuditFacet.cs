using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(LocalizableTypeConverter))]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.FacetSR")]
[DisplayNameKey("ServerAuditName")]
[DisplayDescriptionKey("ServerAuditDesc")]
[CLSCompliant(false)]
public interface IServerAuditFacet : IDmfFacet
{
	[DisplayDescriptionKey("DefaultTraceEnabledDesc")]
	[DisplayNameKey("DefaultTraceEnabledName")]
	bool DefaultTraceEnabled { get; set; }

	[DisplayNameKey("C2AuditTracingEnabledName")]
	[DisplayDescriptionKey("C2AuditTracingEnabledDesc")]
	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	bool C2AuditTracingEnabled { get; set; }

	[DisplayDescriptionKey("LoginAuditLevelDesc")]
	[DisplayNameKey("LoginAuditLevelName")]
	AuditLevel LoginAuditLevel { get; set; }
}
