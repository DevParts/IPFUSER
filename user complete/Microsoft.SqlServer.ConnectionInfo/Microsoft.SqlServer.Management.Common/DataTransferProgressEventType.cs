namespace Microsoft.SqlServer.Management.Common;

public enum DataTransferProgressEventType
{
	ExecuteNonTransactableSql,
	StartTransaction,
	AllowedToFailPrologueSql,
	ExecutePrologueSql,
	GenerateDataFlow,
	ExecutingDataFlow,
	TransferringRows,
	ExecuteEpilogueSql,
	CommitTransaction,
	RollbackTransaction,
	ExecuteCompensatingSql,
	CancelQuery,
	Error
}
