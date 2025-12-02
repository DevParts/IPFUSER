namespace Microsoft.SqlServer.Management.Smo;

public class ResumableOperationStateTypeConverter : EnumToDisplayNameConverter
{
	public ResumableOperationStateTypeConverter()
		: base(typeof(ResumableOperationStateType))
	{
	}
}
