using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class PerformanceCounters
{
	private static readonly PerformanceCountersSingleton performanceCountersSingleton = new PerformanceCountersSingleton();

	public static bool DoCount
	{
		get
		{
			return performanceCountersSingleton.doCount;
		}
		set
		{
			performanceCountersSingleton.doCount = value;
		}
	}

	public static TimeSpan EnumQueriesDuration
	{
		get
		{
			return performanceCountersSingleton.enumQueriesDuration;
		}
		set
		{
			performanceCountersSingleton.enumQueriesDuration = value;
		}
	}

	public static int EnumQueriesCount
	{
		get
		{
			return performanceCountersSingleton.enumQueriesCount;
		}
		set
		{
			performanceCountersSingleton.enumQueriesCount = value;
		}
	}

	public static Hashtable UrnSkeletonsPerf => performanceCountersSingleton.urnSkeletonsPerf;

	public static TimeSpan SqlExecutionDuration
	{
		get
		{
			return performanceCountersSingleton.sqlExecutionDuration;
		}
		set
		{
			performanceCountersSingleton.sqlExecutionDuration = value;
		}
	}

	public static TimeSpan DependencyDiscoveryDuration
	{
		get
		{
			return performanceCountersSingleton.dependencyDiscoveryDuration;
		}
		set
		{
			performanceCountersSingleton.dependencyDiscoveryDuration = value;
		}
	}

	public static int ObjectInfoRequestCount
	{
		get
		{
			return performanceCountersSingleton.objectInfoRequestCount;
		}
		set
		{
			performanceCountersSingleton.objectInfoRequestCount = value;
		}
	}

	public static int InitializeCallsCount
	{
		get
		{
			return performanceCountersSingleton.initializeCallsCount;
		}
		set
		{
			performanceCountersSingleton.initializeCallsCount = value;
		}
	}

	public static int UrnCallsCount
	{
		get
		{
			return performanceCountersSingleton.urnCallsCount;
		}
		set
		{
			performanceCountersSingleton.urnCallsCount = value;
		}
	}

	public static int UrnSkelCallsCount
	{
		get
		{
			return performanceCountersSingleton.urnSkelCallsCount;
		}
		set
		{
			performanceCountersSingleton.urnSkelCallsCount = value;
		}
	}

	public static TimeSpan DiscoverDependenciesDuration
	{
		get
		{
			return performanceCountersSingleton.dscoverDependenciesDuration;
		}
		set
		{
			performanceCountersSingleton.dscoverDependenciesDuration = value;
		}
	}

	public static TimeSpan WalkDependenciesDuration
	{
		get
		{
			return performanceCountersSingleton.walkDependenciesDuration;
		}
		set
		{
			performanceCountersSingleton.walkDependenciesDuration = value;
		}
	}

	public static void Dump(bool toLogFile)
	{
		StringEnumerator enumerator = GetDumpStrings(header: true).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				if (toLogFile)
				{
					TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, current);
				}
				else
				{
					Console.WriteLine(current);
				}
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	public static void Dump(string fileName)
	{
		Dump(fileName, header: true);
	}

	public static void Dump(string fileName, bool header)
	{
		Stream stream = ((!File.Exists(fileName)) ? File.Create(fileName) : File.Open(fileName, FileMode.Truncate));
		StreamWriter streamWriter = new StreamWriter(stream);
		StringEnumerator enumerator = GetDumpStrings(header).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				streamWriter.WriteLine(current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		streamWriter.Close();
	}

	private static StringCollection GetDumpStrings(bool header)
	{
		StringCollection stringCollection = new StringCollection();
		if (header)
		{
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EnumQueriesDuration={0}", new object[1] { EnumQueriesDuration }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DependencyDiscoveryDuration={0}", new object[1] { DependencyDiscoveryDuration }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EnumQueriesCount={0}", new object[1] { EnumQueriesCount }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ObjectInfoRequestCount={0}", new object[1] { ObjectInfoRequestCount }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "SqlExecutionDuration={0}", new object[1] { SqlExecutionDuration }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "InitializeCallsCount={0}", new object[1] { InitializeCallsCount }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "UrnCallsCount={0}", new object[1] { UrnCallsCount }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "UrnSkelCallsCount={0}", new object[1] { UrnSkelCallsCount }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DiscoverDependenciesDuration={0}", new object[1] { DiscoverDependenciesDuration }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "WalkDependenciesDuration={0}", new object[1] { WalkDependenciesDuration }));
			stringCollection.Add("\nStatistic of enumerator requests");
		}
		stringCollection.Add("TotalDuration\tRequestCount\tUrn+Fields");
		foreach (DictionaryEntry item in UrnSkeletonsPerf)
		{
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "{0}\t{1}\t{2}", new object[3]
			{
				((FrequencyPair)item.Value).Duration.Ticks / 10000,
				((FrequencyPair)item.Value).Count,
				item.Key.ToString()
			}));
		}
		return stringCollection;
	}

	public static void Reset()
	{
		performanceCountersSingleton.enumQueriesDuration = new TimeSpan(0L);
		performanceCountersSingleton.enumQueriesCount = 0;
		performanceCountersSingleton.urnSkeletonsPerf = new Hashtable();
		performanceCountersSingleton.sqlExecutionDuration = new TimeSpan(0L);
		performanceCountersSingleton.objectInfoRequestCount = 0;
		performanceCountersSingleton.initializeCallsCount = 0;
		performanceCountersSingleton.urnCallsCount = 0;
		performanceCountersSingleton.urnSkelCallsCount = 0;
	}
}
