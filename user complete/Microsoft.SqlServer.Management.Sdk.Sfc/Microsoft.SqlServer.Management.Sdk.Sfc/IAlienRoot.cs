using System.Collections.Generic;
using System.Data;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IAlienRoot
{
	ServerConnection ConnectionContext { get; }

	string Name { get; }

	DataTable SfcHelper_GetDataTable(object connection, string urn, string[] fields, OrderBy[] orderByFields);

	List<string> SfcHelper_GetSmoObjectQuery(string queryString, string[] fields, OrderBy[] orderByFields);

	object SfcHelper_GetSmoObject(string urn);

	void DesignModeInitialize();
}
