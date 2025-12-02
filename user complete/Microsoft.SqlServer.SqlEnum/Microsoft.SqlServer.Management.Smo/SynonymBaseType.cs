namespace Microsoft.SqlServer.Management.Smo;

public enum SynonymBaseType
{
	None,
	Table,
	View,
	SqlStoredProcedure,
	SqlScalarFunction,
	SqlTableValuedFunction,
	SqlInlineTableValuedFunction,
	ExtendedStoredProcedure,
	ReplicationFilterProcedure,
	ClrStoredProcedure,
	ClrScalarFunction,
	ClrTableValuedFunction,
	ClrAggregateFunction
}
