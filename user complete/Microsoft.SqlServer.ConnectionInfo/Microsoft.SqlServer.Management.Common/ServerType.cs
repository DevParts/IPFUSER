using System;

namespace Microsoft.SqlServer.Management.Common;

public enum ServerType
{
	DatabaseEngine = 0,
	AnalysisServices = 1,
	ReportingServices = 2,
	IntegrationServices = 3,
	[Obsolete("use ServerType.SqlServerCompactEdition")]
	SqlServerEverywhere = 4,
	SqlServerCompactEdition = 4
}
