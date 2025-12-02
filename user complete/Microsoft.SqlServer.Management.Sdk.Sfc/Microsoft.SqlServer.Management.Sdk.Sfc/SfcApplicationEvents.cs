namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcApplicationEvents
{
	public event SfcApplication.SfcObjectCreatedEventHandler ObjectCreated;

	public event SfcApplication.SfcObjectAlteredEventHandler ObjectAltered;

	public event SfcApplication.SfcObjectDroppedEventHandler ObjectDropped;

	public event SfcApplication.SfcBeforeObjectRenamedEventHandler BeforeObjectRenamed;

	public event SfcApplication.SfcAfterObjectRenamedEventHandler AfterObjectRenamed;

	public event SfcApplication.SfcBeforeObjectMovedEventHandler BeforeObjectMoved;

	public event SfcApplication.SfcAfterObjectMovedEventHandler AfterObjectMoved;

	public void OnObjectCreated(SfcInstance obj, SfcObjectCreatedEventArgs e)
	{
		if (this.ObjectCreated != null)
		{
			this.ObjectCreated(obj, e);
		}
	}

	public void OnObjectAltered(SfcInstance obj, SfcObjectAlteredEventArgs e)
	{
		if (this.ObjectAltered != null)
		{
			this.ObjectAltered(obj, e);
		}
	}

	public void OnObjectDropped(SfcInstance obj, SfcObjectDroppedEventArgs e)
	{
		if (this.ObjectDropped != null)
		{
			this.ObjectDropped(obj, e);
		}
	}

	public void OnBeforeObjectRenamed(SfcInstance obj, SfcBeforeObjectRenamedEventArgs e)
	{
		if (this.BeforeObjectRenamed != null)
		{
			this.BeforeObjectRenamed(obj, e);
		}
	}

	public void OnAfterObjectRenamed(SfcInstance obj, SfcAfterObjectRenamedEventArgs e)
	{
		if (this.AfterObjectRenamed != null)
		{
			this.AfterObjectRenamed(obj, e);
		}
	}

	public void OnBeforeObjectMoved(SfcInstance obj, SfcBeforeObjectMovedEventArgs e)
	{
		if (this.BeforeObjectMoved != null)
		{
			this.BeforeObjectMoved(obj, e);
		}
	}

	public void OnAfterObjectMoved(SfcInstance obj, SfcAfterObjectMovedEventArgs e)
	{
		if (this.AfterObjectMoved != null)
		{
			this.AfterObjectMoved(obj, e);
		}
	}
}
