using System.Collections;
using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IDynamicVisible
{
	ICollection ConfigureVisibleEnumFields(ITypeDescriptorContext context, ArrayList values);
}
