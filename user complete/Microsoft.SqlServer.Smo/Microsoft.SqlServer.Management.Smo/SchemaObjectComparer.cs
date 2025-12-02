using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class SchemaObjectComparer : ObjectComparerBase
{
	internal SchemaObjectComparer(IComparer stringComparer)
		: base(stringComparer)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		SchemaObjectKey schemaObjectKey = obj1 as SchemaObjectKey;
		SchemaObjectKey schemaObjectKey2 = obj2 as SchemaObjectKey;
		if (schemaObjectKey2.Schema != null)
		{
			if (schemaObjectKey.Schema == null)
			{
				return 1;
			}
			int num = stringComparer.Compare(schemaObjectKey.Schema, schemaObjectKey2.Schema);
			if (num != 0)
			{
				return num;
			}
		}
		return stringComparer.Compare(schemaObjectKey.Name, schemaObjectKey2.Name);
	}
}
