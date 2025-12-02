using System;
using System.Data;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal abstract class PostProcessDdlEvents : PostProcess
{
	private DataTable m_dtEvents;

	protected override bool SupportDataReader => false;

	public PostProcessDdlEvents()
	{
		m_dtEvents = null;
	}

	protected DataTable GetEventsForAllRows()
	{
		if (m_dtEvents == null)
		{
			Request request = new Request();
			request.Urn = base.Request.Urn.Value + "/Event";
			request.ResultType = ResultType.DataTable;
			request.RequestFieldsTypes = RequestFieldsTypes.Request;
			request.Fields = new string[2] { "ObjectIdentifier", "EventTypeDescription" };
			request.OrderByList = new OrderBy[1]
			{
				new OrderBy("ObjectIdentifier", OrderBy.Direction.Asc)
			};
			m_dtEvents = Enumerator.GetData(base.ConnectionInfo, request);
		}
		return m_dtEvents;
	}

	protected abstract object GetTriggerEvents(string objectIdentifier);

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (IsNull(dp, 0))
		{
			return DBNull.Value;
		}
		return GetTriggerEvents(GetTriggeredString(dp, 0));
	}

	public override void CleanRowData()
	{
		m_dtEvents = null;
	}
}
