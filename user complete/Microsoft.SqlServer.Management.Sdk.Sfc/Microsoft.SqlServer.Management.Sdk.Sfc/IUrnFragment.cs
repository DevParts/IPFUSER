using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IUrnFragment
{
	string Name { get; }

	Dictionary<string, object> FieldDictionary { get; }
}
