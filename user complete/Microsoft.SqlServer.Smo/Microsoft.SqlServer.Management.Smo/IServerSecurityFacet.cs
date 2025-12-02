using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[DisplayNameKey("ServerSecurityName")]
[DisplayDescriptionKey("ServerSecurityDesc")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[CLSCompliant(false)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.FacetSR")]
public interface IServerSecurityFacet : IDmfFacet
{
	[DisplayDescriptionKey("PublicServerRoleIsGrantedPermissionsDesc")]
	[DisplayNameKey("PublicServerRoleIsGrantedPermissionsName")]
	bool PublicServerRoleIsGrantedPermissions { get; }

	[DisplayDescriptionKey("LoginModeDesc")]
	[DisplayNameKey("LoginModeName")]
	ServerLoginMode LoginMode { get; }

	[DisplayNameKey("XPCmdShellEnabledName")]
	[DisplayDescriptionKey("XPCmdShellEnabledDesc")]
	bool XPCmdShellEnabled { get; set; }

	[DisplayNameKey("CrossDBOwnershipChainingEnabledName")]
	[DisplayDescriptionKey("CrossDBOwnershipChainingEnabledDesc")]
	bool CrossDBOwnershipChainingEnabled { get; set; }

	[DisplayDescriptionKey("CommonCriteriaComplianceEnabledDesc")]
	[DisplayNameKey("CommonCriteriaComplianceEnabledName")]
	bool CommonCriteriaComplianceEnabled { get; }

	[DisplayNameKey("IServerSecurityFacet_CmdExecRightsForSystemAdminsOnlyName")]
	[DisplayDescriptionKey("IServerSecurityFacet_CmdExecRightsForSystemAdminsOnlyDesc")]
	bool CmdExecRightsForSystemAdminsOnly { get; }

	[DisplayDescriptionKey("IServerSecurityFacet_ProxyAccountIsGrantedToPublicRoleDesc")]
	[DisplayNameKey("IServerSecurityFacet_ProxyAccountIsGrantedToPublicRoleName")]
	bool ProxyAccountIsGrantedToPublicRole { get; }

	[DisplayNameKey("IServerSecurityFacet_ReplaceAlertTokensEnabledName")]
	[DisplayDescriptionKey("IServerSecurityFacet_ReplaceAlertTokensEnabledDesc")]
	bool ReplaceAlertTokensEnabled { get; set; }

	[DisplayDescriptionKey("IServerSecurityFacet_ProxyAccountEnabledDesc")]
	[DisplayNameKey("IServerSecurityFacet_ProxyAccountEnabledName")]
	bool ProxyAccountEnabled { get; }
}
