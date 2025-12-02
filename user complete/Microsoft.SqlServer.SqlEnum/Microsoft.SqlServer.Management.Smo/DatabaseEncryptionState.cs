namespace Microsoft.SqlServer.Management.Smo;

public enum DatabaseEncryptionState
{
	None,
	Unencrypted,
	EncryptionInProgress,
	Encrypted,
	EncryptionKeyChangesInProgress,
	DecryptionInProgress
}
