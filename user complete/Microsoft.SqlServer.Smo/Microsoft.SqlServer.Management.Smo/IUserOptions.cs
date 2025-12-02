using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[StateChangeEvent("CREATE_USER", "USER", "CERTIFICATE USER")]
[CLSCompliant(false)]
[StateChangeEvent("CREATE_USER", "USER", "SQL USER")]
[StateChangeEvent("ALTER_USER", "USER", "SQL USER")]
[StateChangeEvent("CREATE_USER", "USER", "WINDOWS USER")]
[StateChangeEvent("ALTER_USER", "USER", "WINDOWS USER")]
[StateChangeEvent("CREATE_USER", "USER", "GROUP USER")]
[StateChangeEvent("ALTER_USER", "USER", "GROUP USER")]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
[DisplayDescriptionKey("IUserOptions_Desc")]
[StateChangeEvent("CREATE_USER", "USER", "ASYMMETRIC KEY USER")]
[StateChangeEvent("ALTER_USER", "USER", "ASYMMETRIC KEY USER")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[StateChangeEvent("ALTER_USER", "USER", "CERTIFICATE USER")]
[TypeConverter(typeof(LocalizableTypeConverter))]
[DisplayNameKey("IUserOptions_Name")]
public interface IUserOptions : IDmfFacet
{
	[DisplayNameKey("User_AsymmetricKeyName")]
	[DisplayDescriptionKey("User_AsymmetricKeyDesc")]
	string AsymmetricKey { get; }

	[DisplayDescriptionKey("User_CertificateDesc")]
	[DisplayNameKey("User_CertificateName")]
	string Certificate { get; }

	[DisplayDescriptionKey("User_CreateDateDesc")]
	[DisplayNameKey("User_CreateDateName")]
	DateTime CreateDate { get; }

	[DisplayDescriptionKey("User_DefaultSchemaDesc")]
	[DisplayNameKey("User_DefaultSchemaName")]
	string DefaultSchema { get; set; }

	[DisplayNameKey("User_IDName")]
	[DisplayDescriptionKey("User_IDDesc")]
	int ID { get; }

	[DisplayNameKey("User_IsSystemObjectName")]
	[DisplayDescriptionKey("User_IsSystemObjectDesc")]
	bool IsSystemObject { get; }

	[DisplayDescriptionKey("User_LoginDesc")]
	[DisplayNameKey("User_LoginName")]
	string Login { get; }

	[DisplayNameKey("User_LoginTypeName")]
	[DisplayDescriptionKey("User_LoginTypeDesc")]
	LoginType LoginType { get; }

	[DisplayDescriptionKey("NamedSmoObject_NameDesc")]
	[DisplayNameKey("NamedSmoObject_NameName")]
	string Name { get; }

	[DisplayNameKey("User_SidName")]
	[DisplayDescriptionKey("User_SidDesc")]
	byte[] Sid { get; }

	[DisplayNameKey("User_UserTypeName")]
	[DisplayDescriptionKey("User_UserTypeDesc")]
	UserType UserType { get; }
}
