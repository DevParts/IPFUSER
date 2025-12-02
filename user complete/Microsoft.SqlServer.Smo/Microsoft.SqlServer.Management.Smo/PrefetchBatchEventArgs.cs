using System;

namespace Microsoft.SqlServer.Management.Smo;

internal class PrefetchBatchEventArgs : EventArgs
{
	public string UrnType { get; private set; }

	public int BatchSize { get; private set; }

	public int CurrentBatchCount { get; private set; }

	public int TotalBatchCount { get; private set; }

	internal PrefetchBatchEventArgs(string urnType, int batchSize, int currentBatchCount, int totalBatchCount)
	{
		UrnType = urnType;
		BatchSize = batchSize;
		CurrentBatchCount = currentBatchCount;
		TotalBatchCount = totalBatchCount;
	}
}
