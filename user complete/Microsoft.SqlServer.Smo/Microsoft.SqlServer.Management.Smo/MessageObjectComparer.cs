using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class MessageObjectComparer : ObjectComparerBase
{
	internal MessageObjectComparer(IComparer stringComparer)
		: base(stringComparer)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		MessageObjectKey messageObjectKey = obj1 as MessageObjectKey;
		MessageObjectKey messageObjectKey2 = obj2 as MessageObjectKey;
		if (messageObjectKey.ID != messageObjectKey2.ID)
		{
			return messageObjectKey.ID - messageObjectKey2.ID;
		}
		return stringComparer.Compare(messageObjectKey.Language, messageObjectKey2.Language);
	}
}
