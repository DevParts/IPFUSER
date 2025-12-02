using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class PerformanceCountersSingleton
{
	internal bool doCount;

	internal TimeSpan enumQueriesDuration = new TimeSpan(0L);

	internal int enumQueriesCount;

	internal Hashtable urnSkeletonsPerf = new Hashtable();

	internal TimeSpan sqlExecutionDuration = new TimeSpan(0L);

	internal TimeSpan dependencyDiscoveryDuration = new TimeSpan(0L);

	internal int objectInfoRequestCount;

	internal int initializeCallsCount;

	internal int urnCallsCount;

	internal int urnSkelCallsCount;

	internal TimeSpan dscoverDependenciesDuration = new TimeSpan(0L);

	internal TimeSpan walkDependenciesDuration = new TimeSpan(0L);
}
