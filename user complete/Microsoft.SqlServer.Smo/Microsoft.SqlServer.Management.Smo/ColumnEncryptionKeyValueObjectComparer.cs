namespace Microsoft.SqlServer.Management.Smo;

internal class ColumnEncryptionKeyValueObjectComparer : ObjectComparerBase
{
	internal ColumnEncryptionKeyValueObjectComparer()
		: base(null)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		return ((ColumnEncryptionKeyValueObjectKey)obj1).ColumnMasterKeyID - ((ColumnEncryptionKeyValueObjectKey)obj2).ColumnMasterKeyID;
	}
}
