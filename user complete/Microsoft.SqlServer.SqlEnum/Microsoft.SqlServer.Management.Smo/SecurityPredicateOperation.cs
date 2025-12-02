using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(SecurityPredicateOperationConverter))]
public enum SecurityPredicateOperation
{
	[LocDisplayName("securityPredicateOperationAll")]
	[TsqlSyntaxString("")]
	All,
	[LocDisplayName("securityPredicateOperationAfterInsert")]
	[TsqlSyntaxString("AFTER INSERT")]
	AfterInsert,
	[LocDisplayName("securityPredicateOperationAfterUpdate")]
	[TsqlSyntaxString("AFTER UPDATE")]
	AfterUpdate,
	[LocDisplayName("securityPredicateOperationBeforeUpdate")]
	[TsqlSyntaxString("BEFORE UPDATE")]
	BeforeUpdate,
	[TsqlSyntaxString("BEFORE DELETE")]
	[LocDisplayName("securityPredicateOperationBeforeDelete")]
	BeforeDelete
}
