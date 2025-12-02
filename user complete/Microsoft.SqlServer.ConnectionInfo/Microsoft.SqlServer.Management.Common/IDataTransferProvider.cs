namespace Microsoft.SqlServer.Management.Common;

public interface IDataTransferProvider
{
	event DataTransferEventHandler TransferEvent;

	void Configure(ITransferMetadataProvider metadataProvider);

	void ExecuteTransfer();
}
