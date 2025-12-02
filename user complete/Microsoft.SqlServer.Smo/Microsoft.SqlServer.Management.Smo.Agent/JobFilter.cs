using System;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class JobFilter
{
	private string category;

	private bool enabled;

	private bool enabledDirty;

	private JobExecutionStatus currentExecutionStatus;

	internal bool currentExecutionStatusDirty;

	private string owner;

	private FindOperand dateFindOperand = FindOperand.EqualTo;

	private AgentSubSystem stepSubsystem;

	internal bool stepSubsystemDirty;

	private DateTime dateJobCreated;

	private bool dateJobCreatedDirty;

	private JobType jobType;

	internal bool jobTypeDirty;

	private DateTime dateJobLastModified;

	private bool dateJobLastModifiedDirty;

	public string Category
	{
		get
		{
			return category;
		}
		set
		{
			category = value;
		}
	}

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabledDirty = true;
			enabled = value;
		}
	}

	public JobExecutionStatus CurrentExecutionStatus
	{
		get
		{
			return currentExecutionStatus;
		}
		set
		{
			currentExecutionStatusDirty = true;
			currentExecutionStatus = value;
		}
	}

	public string Owner
	{
		get
		{
			return owner;
		}
		set
		{
			owner = value;
		}
	}

	public FindOperand DateFindOperand
	{
		get
		{
			return dateFindOperand;
		}
		set
		{
			dateFindOperand = value;
		}
	}

	public AgentSubSystem StepSubsystem
	{
		get
		{
			return stepSubsystem;
		}
		set
		{
			stepSubsystemDirty = true;
			stepSubsystem = value;
		}
	}

	public DateTime DateJobCreated
	{
		get
		{
			return dateJobCreated;
		}
		set
		{
			dateJobCreatedDirty = true;
			dateJobCreated = value;
		}
	}

	public JobType JobType
	{
		get
		{
			return jobType;
		}
		set
		{
			jobTypeDirty = true;
			jobType = value;
		}
	}

	public DateTime DateJobLastModified
	{
		get
		{
			return dateJobLastModified;
		}
		set
		{
			dateJobLastModifiedDirty = true;
			dateJobLastModified = value;
		}
	}

	internal Request GetEnumRequest(JobServer jobServer)
	{
		Request request = new Request();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append(jobServer.Urn);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "/Job");
		GetRequestFilter(stringBuilder);
		request.Urn = stringBuilder.ToString();
		return request;
	}

	private void GetRequestFilter(StringBuilder builder)
	{
		int num = 0;
		if (category != null)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@Category='{0}'", new object[1] { Urn.EscapeString(category) });
		}
		if (enabledDirty)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@IsEnabled={0}", new object[1] { enabled ? 1 : 0 });
		}
		if (owner != null)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@OwnerLoginName='{0}'", new object[1] { Urn.EscapeString(owner) });
		}
		if (dateJobCreatedDirty)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@DateCreated {0} '{1}'", new object[2]
			{
				GetStringOperand(dateFindOperand),
				dateJobCreated.ToString(SmoApplication.DefaultCulture)
			});
		}
		if (dateJobLastModifiedDirty)
		{
			if (num++ > 0)
			{
				builder.Append(" and ");
			}
			else
			{
				builder.Append("[");
			}
			builder.AppendFormat(SmoApplication.DefaultCulture, "@DateLastModified {0} '{1}'", new object[2]
			{
				GetStringOperand(dateFindOperand),
				dateJobLastModified.ToString(SmoApplication.DefaultCulture)
			});
		}
		if (num > 0)
		{
			builder.Append("]");
		}
	}

	private string GetStringOperand(FindOperand fo)
	{
		return fo switch
		{
			FindOperand.EqualTo => "=", 
			FindOperand.LessThan => "<", 
			FindOperand.GreaterThan => ">", 
			_ => "=", 
		};
	}
}
