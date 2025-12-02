using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[StateChangeEvent("ALTER_TABLE", "TABLE")]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[StateChangeEvent("CREATE_TABLE", "TABLE")]
[StateChangeEvent("RENAME", "TABLE")]
[StateChangeEvent("ALTER_AUTHORIZATION_DATABASE", "TABLE")]
[StateChangeEvent("ALTER_SCHEMA", "TABLE")]
[DisplayNameKey("ITableOptions_Name")]
[DisplayDescriptionKey("ITableOptions_Desc")]
[TypeConverter(typeof(LocalizableTypeConverter))]
[CLSCompliant(false)]
public interface ITableOptions : IDmfFacet
{
	[DisplayDescriptionKey("Table_AnsiNullsStatusDesc")]
	[DisplayNameKey("Table_AnsiNullsStatusName")]
	bool AnsiNullsStatus { get; set; }

	[DisplayNameKey("Table_ChangeTrackingEnabledName")]
	[DisplayDescriptionKey("Table_ChangeTrackingEnabledDesc")]
	bool ChangeTrackingEnabled { get; set; }

	[DisplayNameKey("Table_CreateDateName")]
	[DisplayDescriptionKey("Table_CreateDateDesc")]
	DateTime CreateDate { get; }

	[DisplayDescriptionKey("Table_FakeSystemTableDesc")]
	[DisplayNameKey("Table_FakeSystemTableName")]
	bool FakeSystemTable { get; }

	[DisplayNameKey("Table_IDName")]
	[DisplayDescriptionKey("Table_IDDesc")]
	int ID { get; }

	[DisplayNameKey("Table_IsSchemaOwnedName")]
	[DisplayDescriptionKey("Table_IsSchemaOwnedDesc")]
	bool IsSchemaOwned { get; }

	[DisplayDescriptionKey("Table_IsSystemObjectDesc")]
	[DisplayNameKey("Table_IsSystemObjectName")]
	bool IsSystemObject { get; }

	[DisplayDescriptionKey("Table_LockEscalationDesc")]
	[DisplayNameKey("Table_LockEscalationName")]
	LockEscalationType LockEscalation { get; set; }

	[DisplayNameKey("NamedSmoObject_NameName")]
	[DisplayDescriptionKey("NamedSmoObject_NameDesc")]
	string Name { get; }

	[DisplayNameKey("Table_OwnerName")]
	[DisplayDescriptionKey("Table_OwnerDesc")]
	string Owner { get; set; }

	[DisplayNameKey("Table_QuotedIdentifierStatusName")]
	[DisplayDescriptionKey("Table_QuotedIdentifierStatusDesc")]
	bool QuotedIdentifierStatus { get; }

	[DisplayDescriptionKey("Table_RemoteDataArchiveEnabledDesc")]
	[DisplayNameKey("Table_RemoteDataArchiveEnabledName")]
	bool RemoteDataArchiveEnabled { get; set; }

	[DisplayNameKey("Table_RemoteDataArchiveDataMigrationStateName")]
	[DisplayDescriptionKey("Table_RemoteDataArchiveDataMigrationStateDesc")]
	RemoteDataArchiveMigrationState RemoteDataArchiveDataMigrationState { get; set; }

	[DisplayDescriptionKey("Table_RemoteTableNameDesc")]
	[DisplayNameKey("Table_RemoteTableNameName")]
	string RemoteTableName { get; }

	[DisplayNameKey("Table_RemoteTableProvisionedName")]
	[DisplayDescriptionKey("Table_RemoteTableProvisionedDesc")]
	bool RemoteTableProvisioned { get; }

	[DisplayDescriptionKey("Table_ReplicatedDesc")]
	[DisplayNameKey("Table_ReplicatedName")]
	bool Replicated { get; }

	[DisplayNameKey("ScriptSchemaObjectBase_SchemaName")]
	[DisplayDescriptionKey("ScriptSchemaObjectBase_SchemaDesc")]
	string Schema { get; }

	[DisplayDescriptionKey("Table_TrackColumnsUpdatedEnabledDesc")]
	[DisplayNameKey("Table_TrackColumnsUpdatedEnabledName")]
	bool TrackColumnsUpdatedEnabled { get; set; }
}
