namespace Microsoft.SqlServer.Management.Smo;

internal class FileGroupTypeConverter : EnumToDisplayNameConverter
{
	public FileGroupTypeConverter()
		: base(typeof(FileGroupType))
	{
	}
}
