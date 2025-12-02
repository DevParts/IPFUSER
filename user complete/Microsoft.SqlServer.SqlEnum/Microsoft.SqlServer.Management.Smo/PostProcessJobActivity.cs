using System;
using System.Data;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessJobActivity : PostProcessCreateDateTime
{
	private DataTable dt;

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		DataRow dataRow = null;
		Guid guid = new Guid(GetTriggeredString(dp, 0));
		if (dt == null)
		{
			if (1 == dp.TableRowCount)
			{
				dt = ExecuteSql.ExecuteWithResults($"exec msdb.dbo.sp_help_job @job_id='{guid}'", base.ConnectionInfo);
			}
			else
			{
				dt = ExecuteSql.ExecuteWithResults("exec msdb.dbo.sp_help_job", base.ConnectionInfo);
			}
		}
		foreach (DataRow row in dt.Rows)
		{
			if ((Guid)row[0] == guid)
			{
				dataRow = row;
				break;
			}
		}
		if (dataRow == null)
		{
			return DBNull.Value;
		}
		return name switch
		{
			"CurrentRunRetryAttempt" => dataRow["current_retry_attempt"], 
			"CurrentRunStatus" => dataRow["current_execution_status"], 
			"CurrentRunStep" => dataRow["current_execution_step"], 
			"HasSchedule" => ((int)dataRow["has_schedule"] != 0) ? true : false, 
			"HasServer" => ((int)dataRow["has_target"] != 0) ? true : false, 
			"HasStep" => ((int)dataRow["has_step"] != 0) ? true : false, 
			"LastRunDate" => GetDateTime(dataRow["last_run_date"], dataRow["last_run_time"]), 
			"LastRunOutcome" => dataRow["last_run_outcome"], 
			"NextRunDate" => GetDateTime(dataRow["next_run_date"], dataRow["next_run_time"]), 
			"NextRunScheduleID" => dataRow["next_run_schedule_id"], 
			"JobType" => dataRow["type"], 
			_ => data, 
		};
	}
}
