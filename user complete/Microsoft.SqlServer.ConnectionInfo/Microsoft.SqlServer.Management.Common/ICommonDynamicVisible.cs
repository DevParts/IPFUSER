using System.Collections;
using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Common;

public interface ICommonDynamicVisible
{
	ICollection ConfigureVisibleEnumFields(ITypeDescriptorContext context, ArrayList values);
}
