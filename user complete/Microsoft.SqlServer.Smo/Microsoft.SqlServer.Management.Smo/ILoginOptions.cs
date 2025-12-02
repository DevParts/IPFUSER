using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[StateChangeEvent("ALTER_LOGIN", "LOGIN")]
[DisplayNameKey("ILoginOptions_Name")]
[TypeConverter(typeof(LocalizableTypeConverter))]
[CLSCompliant(false)]
[StateChangeEvent("CREATE_LOGIN", "LOGIN")]
[DisplayDescriptionKey("ILoginOptions_Desc")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
public interface ILoginOptions : IDmfFacet
{
	[DisplayNameKey("Login_AsymmetricKeyName")]
	[DisplayDescriptionKey("Login_AsymmetricKeyDesc")]
	string AsymmetricKey { get; set; }

	[DisplayNameKey("Login_CertificateName")]
	[DisplayDescriptionKey("Login_CertificateDesc")]
	string Certificate { get; set; }

	[DisplayDescriptionKey("Login_CreateDateDesc")]
	[DisplayNameKey("Login_CreateDateName")]
	DateTime CreateDate { get; }

	[DisplayNameKey("Login_CredentialName")]
	[DisplayDescriptionKey("Login_CredentialDesc")]
	string Credential { get; set; }

	[DisplayNameKey("Login_DefaultDatabaseName")]
	[DisplayDescriptionKey("Login_DefaultDatabaseDesc")]
	string DefaultDatabase { get; set; }

	[DisplayDescriptionKey("Login_IDDesc")]
	[DisplayNameKey("Login_IDName")]
	int ID { get; }

	[DisplayNameKey("Login_IsDisabledName")]
	[DisplayDescriptionKey("Login_IsDisabledDesc")]
	bool IsDisabled { get; }

	[DisplayNameKey("Login_IsLockedName")]
	[DisplayDescriptionKey("Login_IsLockedDesc")]
	bool IsLocked { get; }

	[DisplayDescriptionKey("Login_IsSystemObjectDesc")]
	[DisplayNameKey("Login_IsSystemObjectName")]
	bool IsSystemObject { get; }

	[DisplayDescriptionKey("Login_LanguageDesc")]
	[DisplayNameKey("Login_LanguageName")]
	string Language { get; set; }

	[DisplayDescriptionKey("Login_LanguageAliasDesc")]
	[DisplayNameKey("Login_LanguageAliasName")]
	string LanguageAlias { get; }

	[DisplayDescriptionKey("Login_MustChangePasswordDesc")]
	[DisplayNameKey("Login_MustChangePasswordName")]
	bool MustChangePassword { get; }

	[DisplayNameKey("NamedSmoObject_NameName")]
	[DisplayDescriptionKey("NamedSmoObject_NameDesc")]
	string Name { get; set; }

	[DisplayDescriptionKey("Login_PasswordExpirationEnabledDesc")]
	[DisplayNameKey("Login_PasswordExpirationEnabledName")]
	bool PasswordExpirationEnabled { get; set; }

	[DisplayNameKey("Login_PasswordPolicyEnforcedName")]
	[DisplayDescriptionKey("Login_PasswordPolicyEnforcedDesc")]
	bool PasswordPolicyEnforced { get; set; }
}
