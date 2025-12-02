using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal class ColumnEncryptionKeyValueObjectKey : ObjectKeyBase
{
	public int ColumnMasterKeyID;

	internal static readonly StringCollection fields;

	public override string UrnFilter => string.Format(SmoApplication.DefaultCulture, "@ColumnMasterKeyID={0}", new object[1] { ColumnMasterKeyID });

	public override bool IsNull => false;

	public ColumnEncryptionKeyValueObjectKey(int columnMasterKeyID)
	{
		ColumnMasterKeyID = columnMasterKeyID;
	}

	static ColumnEncryptionKeyValueObjectKey()
	{
		fields = new StringCollection();
		fields.Add("ColumnMasterKeyID");
	}

	public override string ToString()
	{
		return string.Format(SmoApplication.DefaultCulture, "{0}", new object[1] { ColumnMasterKeyID });
	}

	public override StringCollection GetFieldNames()
	{
		return fields;
	}

	public override ObjectKeyBase Clone()
	{
		return new ColumnEncryptionKeyValueObjectKey(ColumnMasterKeyID);
	}

	public override ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new ColumnEncryptionKeyValueObjectComparer();
	}
}
