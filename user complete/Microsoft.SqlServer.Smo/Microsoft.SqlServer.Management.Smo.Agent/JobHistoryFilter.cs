using System;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class JobHistoryFilter
{
	private Guid jobID = Guid.Empty;

	private string jobName;

	private int minimumRetries;

	private int minimumRunDuration;

	private bool oldestFirst;

	private CompletionResult outcomeTypes;

	private bool outcomeDirty;

	private int messageID = -1;

	private int severity = -1;

	private bool startRunDateDirty;

	private DateTime startRunDate;

	private bool endRunDateDirty;

	private DateTime endRunDate;

	public Guid JobID
	{
		get
		{
			return jobID;
		}
		set
		{
			jobID = value;
		}
	}

	public string JobName
	{
		get
		{
			return jobName;
		}
		set
		{
			jobName = value;
		}
	}

	public int MinimumRetries
	{
		get
		{
			return minimumRetries;
		}
		set
		{
			minimumRetries = value;
		}
	}

	public int MinimumRunDuration
	{
		get
		{
			return minimumRunDuration;
		}
		set
		{
			minimumRunDuration = value;
		}
	}

	public bool OldestFirst
	{
		get
		{
			return oldestFirst;
		}
		set
		{
			oldestFirst = value;
		}
	}

	public CompletionResult OutcomeTypes
	{
		get
		{
			return outcomeTypes;
		}
		set
		{
			outcomeDirty = true;
			outcomeTypes = value;
		}
	}

	public int SqlMessageID
	{
		get
		{
			return messageID;
		}
		set
		{
			messageID = value;
		}
	}

	public int SqlSeverity
	{
		get
		{
			return severity;
		}
		set
		{
			severity = value;
		}
	}

	public DateTime StartRunDate
	{
		get
		{
			return startRunDate;
		}
		set
		{
			startRunDateDirty = true;
			startRunDate = value;
		}
	}

	public DateTime EndRunDate
	{
		get
		{
			return endRunDate;
		}
		set
		{
			endRunDateDirty = true;
			endRunDate = value;
		}
	}

	internal Request GetEnumRequest(Job job)
	{
		Request request = new Request();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append(job.ParentColl.ParentInstance.Urn);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "/Job[@JobID= '{0}']/History", new object[1] { job.JobIDInternal });
		GetRequestFilter(stringBuilder);
		request.Urn = stringBuilder.ToString();
		return request;
	}

	internal Request GetEnumRequest(JobServer jobServer)
	{
		Request request = new Request();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append(jobServer.Urn);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "/Job/History");
		GetRequestFilter(stringBuilder);
		request.Urn = stringBuilder.ToString();
		return request;
	}

	internal int GetDateInt(DateTime dateTime)
	{
		return dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day;
	}

	internal int GetTimeInt(DateTime dateTime)
	{
		return dateTime.Hour * 10000 + dateTime.Minute * 100 + dateTime.Second;
	}

	private void GetRequestFilter(StringBuilder builder)
	{
		int num = 0;
		if (Guid.Empty != jobID)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@JobID='{0}'", new object[1] { jobID.ToString("D", SmoApplication.DefaultCulture) });
		}
		if (jobName != null && jobName.Length > 0)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@JobName='{0}'", new object[1] { Urn.EscapeString(jobName) });
		}
		if (minimumRetries > 0)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@RetriesAttempted > {0}", new object[1] { minimumRetries });
		}
		if (minimumRunDuration > 0)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@RunDuration > {0}", new object[1] { minimumRunDuration });
		}
		if (outcomeDirty)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@RunStatus = {0}", new object[1] { Enum.Format(typeof(CompletionResult), outcomeTypes, "d") });
		}
		if (messageID > 0)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@SqlMessageID = {0}", new object[1] { messageID });
		}
		if (severity > 0)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@SqlSeverity= {0}", new object[1] { severity });
		}
		if (startRunDateDirty)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@RunDate >= '{0}'", new object[1] { SqlSmoObject.SqlDateString(startRunDate) });
		}
		if (endRunDateDirty)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@RunDate <= '{0}'", new object[1] { SqlSmoObject.SqlDateString(endRunDate) });
		}
		if (num > 0)
		{
			builder.Append("]");
		}
	}

	internal string GetPurgeFilter()
	{
		string text = string.Empty;
		if (Guid.Empty != jobID)
		{
			text = string.Format(SmoApplication.DefaultCulture, " @job_id=N'{0}'", new object[1] { jobID.ToString("D", SmoApplication.DefaultCulture) });
		}
		else if (jobName != null && jobName.Length > 0)
		{
			if (0 < text.Length)
			{
				text += ",";
			}
			text += string.Format(SmoApplication.DefaultCulture, " @job_name='{0}'", new object[1] { SqlSmoObject.SqlString(jobName) });
		}
		if (endRunDateDirty)
		{
			if (0 < text.Length)
			{
				text += ",";
			}
			text += string.Format(SmoApplication.DefaultCulture, " @oldest_date='{0}'", new object[1] { SqlSmoObject.SqlDateString(endRunDate) });
		}
		return text;
	}
}
