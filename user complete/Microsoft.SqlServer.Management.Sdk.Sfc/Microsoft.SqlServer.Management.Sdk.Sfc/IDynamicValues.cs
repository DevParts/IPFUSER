using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IDynamicValues
{
	TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context);
}
