using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[StateChangeEvent("ALTER_SCHEMA", "VIEW")]
[StateChangeEvent("ALTER_VIEW", "VIEW")]
[StateChangeEvent("RENAME", "VIEW")]
[StateChangeEvent("ALTER_AUTHORIZATION_DATABASE", "VIEW")]
[DisplayDescriptionKey("IViewOptions_Desc")]
[DisplayNameKey("IViewOptions_Name")]
[StateChangeEvent("CREATE_VIEW", "VIEW")]
[TypeConverter(typeof(LocalizableTypeConverter))]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[CLSCompliant(false)]
public interface IViewOptions : IDmfFacet
{
	[DisplayNameKey("View_AnsiNullsStatusName")]
	[DisplayDescriptionKey("View_AnsiNullsStatusDesc")]
	bool AnsiNullsStatus { get; }

	[DisplayDescriptionKey("View_CreateDateDesc")]
	[DisplayNameKey("View_CreateDateName")]
	DateTime CreateDate { get; }

	[DisplayDescriptionKey("View_IDDesc")]
	[DisplayNameKey("View_IDName")]
	int ID { get; }

	[DisplayDescriptionKey("View_IsEncryptedDesc")]
	[DisplayNameKey("View_IsEncryptedName")]
	bool IsEncrypted { get; }

	[DisplayNameKey("View_IsSchemaBoundName")]
	[DisplayDescriptionKey("View_IsSchemaBoundDesc")]
	bool IsSchemaBound { get; }

	[DisplayDescriptionKey("View_IsSchemaOwnedDesc")]
	[DisplayNameKey("View_IsSchemaOwnedName")]
	bool IsSchemaOwned { get; }

	[DisplayNameKey("View_IsSystemObjectName")]
	[DisplayDescriptionKey("View_IsSystemObjectDesc")]
	bool IsSystemObject { get; }

	[DisplayNameKey("NamedSmoObject_NameName")]
	[DisplayDescriptionKey("NamedSmoObject_NameDesc")]
	string Name { get; }

	[DisplayNameKey("View_OwnerName")]
	[DisplayDescriptionKey("View_OwnerDesc")]
	string Owner { get; }

	[DisplayDescriptionKey("ScriptSchemaObjectBase_SchemaDesc")]
	[DisplayNameKey("ScriptSchemaObjectBase_SchemaName")]
	string Schema { get; }

	[DisplayNameKey("View_QuotedIdentifierStatusName")]
	[DisplayDescriptionKey("View_QuotedIdentifierStatusDesc")]
	bool QuotedIdentifierStatus { get; }

	[DisplayDescriptionKey("View_ReturnsViewMetadataDesc")]
	[DisplayNameKey("View_ReturnsViewMetadataName")]
	bool ReturnsViewMetadata { get; }
}
