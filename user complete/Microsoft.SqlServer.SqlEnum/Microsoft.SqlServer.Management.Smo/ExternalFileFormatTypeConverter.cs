namespace Microsoft.SqlServer.Management.Smo;

public class ExternalFileFormatTypeConverter : EnumToDisplayNameConverter
{
	public ExternalFileFormatTypeConverter()
		: base(typeof(ExternalFileFormatType))
	{
	}
}
