using System.Collections;

namespace Microsoft.SqlServer.Management.Common;

public interface ITransferMetadataProvider
{
	void SaveMetadata();

	SortedList GetOptions();
}
