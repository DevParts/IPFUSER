namespace Microsoft.SqlServer.Management.Smo;

public sealed class ViewEvent
{
	private ViewEventValues m_value;

	internal ViewEventValues Value => m_value;

	public static ViewEvent AlterFulltextIndex => new ViewEvent(ViewEventValues.AlterFulltextIndex);

	public static ViewEvent AlterIndex => new ViewEvent(ViewEventValues.AlterIndex);

	public static ViewEvent AlterView => new ViewEvent(ViewEventValues.AlterView);

	public static ViewEvent CreateFulltextIndex => new ViewEvent(ViewEventValues.CreateFulltextIndex);

	public static ViewEvent CreateIndex => new ViewEvent(ViewEventValues.CreateIndex);

	public static ViewEvent CreateSpatialIndex => new ViewEvent(ViewEventValues.CreateSpatialIndex);

	public static ViewEvent CreateStatistics => new ViewEvent(ViewEventValues.CreateStatistics);

	public static ViewEvent CreateXmlIndex => new ViewEvent(ViewEventValues.CreateXmlIndex);

	public static ViewEvent DropFulltextIndex => new ViewEvent(ViewEventValues.DropFulltextIndex);

	public static ViewEvent DropIndex => new ViewEvent(ViewEventValues.DropIndex);

	public static ViewEvent DropStatistics => new ViewEvent(ViewEventValues.DropStatistics);

	public static ViewEvent DropView => new ViewEvent(ViewEventValues.DropView);

	public static ViewEvent UpdateStatistics => new ViewEvent(ViewEventValues.UpdateStatistics);

	internal ViewEvent(ViewEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator ViewEventSet(ViewEvent eventValue)
	{
		return new ViewEventSet(eventValue);
	}

	public static ViewEventSet operator +(ViewEvent eventLeft, ViewEvent eventRight)
	{
		ViewEventSet viewEventSet = new ViewEventSet(eventLeft);
		viewEventSet.SetBit(eventRight);
		return viewEventSet;
	}

	public static ViewEventSet Add(ViewEvent eventLeft, ViewEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static ViewEventSet operator |(ViewEvent eventLeft, ViewEvent eventRight)
	{
		ViewEventSet viewEventSet = new ViewEventSet(eventLeft);
		viewEventSet.SetBit(eventRight);
		return viewEventSet;
	}

	public static ViewEventSet BitwiseOr(ViewEvent eventLeft, ViewEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}

	public static bool operator ==(ViewEvent a, ViewEvent b)
	{
		if ((object)a == null && (object)b == null)
		{
			return true;
		}
		if ((object)a == null || (object)b == null)
		{
			return false;
		}
		return a.m_value == b.m_value;
	}

	public static bool operator !=(ViewEvent a, ViewEvent b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return this == obj as ViewEvent;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
