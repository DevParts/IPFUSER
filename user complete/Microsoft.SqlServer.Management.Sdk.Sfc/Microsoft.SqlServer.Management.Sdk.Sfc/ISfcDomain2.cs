using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcDomain2 : ISfcDomain, ISfcDomainLite, ISfcHasConnection
{
	List<string> GetUrnSkeletonsFromType(Type inputType);
}
