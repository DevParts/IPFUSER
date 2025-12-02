namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcApplication
{
	public delegate void SfcObjectCreatedEventHandler(object sender, SfcObjectCreatedEventArgs e);

	public delegate void SfcObjectDroppedEventHandler(object sender, SfcObjectDroppedEventArgs e);

	public delegate void SfcBeforeObjectRenamedEventHandler(object sender, SfcBeforeObjectRenamedEventArgs e);

	public delegate void SfcAfterObjectRenamedEventHandler(object sender, SfcAfterObjectRenamedEventArgs e);

	public delegate void SfcBeforeObjectMovedEventHandler(object sender, SfcBeforeObjectMovedEventArgs e);

	public delegate void SfcAfterObjectMovedEventHandler(object sender, SfcAfterObjectMovedEventArgs e);

	public delegate void SfcObjectAlteredEventHandler(object sender, SfcObjectAlteredEventArgs e);

	public static readonly SfcApplicationEvents Events = new SfcApplicationEvents();

	internal static readonly string ModuleName = "Sfc";
}
