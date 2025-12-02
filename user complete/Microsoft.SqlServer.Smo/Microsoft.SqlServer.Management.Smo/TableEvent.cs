namespace Microsoft.SqlServer.Management.Smo;

public sealed class TableEvent
{
	private TableEventValues m_value;

	internal TableEventValues Value => m_value;

	public static TableEvent AlterFulltextIndex => new TableEvent(TableEventValues.AlterFulltextIndex);

	public static TableEvent AlterIndex => new TableEvent(TableEventValues.AlterIndex);

	public static TableEvent AlterTable => new TableEvent(TableEventValues.AlterTable);

	public static TableEvent CreateFulltextIndex => new TableEvent(TableEventValues.CreateFulltextIndex);

	public static TableEvent CreateIndex => new TableEvent(TableEventValues.CreateIndex);

	public static TableEvent CreateSpatialIndex => new TableEvent(TableEventValues.CreateSpatialIndex);

	public static TableEvent CreateStatistics => new TableEvent(TableEventValues.CreateStatistics);

	public static TableEvent CreateXmlIndex => new TableEvent(TableEventValues.CreateXmlIndex);

	public static TableEvent DropFulltextIndex => new TableEvent(TableEventValues.DropFulltextIndex);

	public static TableEvent DropIndex => new TableEvent(TableEventValues.DropIndex);

	public static TableEvent DropStatistics => new TableEvent(TableEventValues.DropStatistics);

	public static TableEvent DropTable => new TableEvent(TableEventValues.DropTable);

	public static TableEvent UpdateStatistics => new TableEvent(TableEventValues.UpdateStatistics);

	internal TableEvent(TableEventValues eventValue)
	{
		m_value = eventValue;
	}

	public static implicit operator TableEventSet(TableEvent eventValue)
	{
		return new TableEventSet(eventValue);
	}

	public static TableEventSet operator +(TableEvent eventLeft, TableEvent eventRight)
	{
		TableEventSet tableEventSet = new TableEventSet(eventLeft);
		tableEventSet.SetBit(eventRight);
		return tableEventSet;
	}

	public static TableEventSet Add(TableEvent eventLeft, TableEvent eventRight)
	{
		return eventLeft + eventRight;
	}

	public static TableEventSet operator |(TableEvent eventLeft, TableEvent eventRight)
	{
		TableEventSet tableEventSet = new TableEventSet(eventLeft);
		tableEventSet.SetBit(eventRight);
		return tableEventSet;
	}

	public static TableEventSet BitwiseOr(TableEvent eventLeft, TableEvent eventRight)
	{
		return eventLeft | eventRight;
	}

	public override string ToString()
	{
		return m_value.ToString();
	}

	public static bool operator ==(TableEvent a, TableEvent b)
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

	public static bool operator !=(TableEvent a, TableEvent b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return this == obj as TableEvent;
	}

	public override int GetHashCode()
	{
		return m_value.GetHashCode();
	}
}
