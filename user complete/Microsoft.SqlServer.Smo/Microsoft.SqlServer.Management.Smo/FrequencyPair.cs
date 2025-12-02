using System;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class FrequencyPair
{
	private int count;

	private TimeSpan duration = new TimeSpan(0L);

	public int Count
	{
		get
		{
			return count;
		}
		set
		{
			count = value;
		}
	}

	public TimeSpan Duration
	{
		get
		{
			return duration;
		}
		set
		{
			duration = value;
		}
	}
}
