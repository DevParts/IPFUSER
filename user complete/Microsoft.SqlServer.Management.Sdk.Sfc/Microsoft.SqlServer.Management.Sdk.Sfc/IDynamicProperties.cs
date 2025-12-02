using System;
using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IDynamicProperties
{
	void AddProperties(PropertyDescriptorCollection properties, ITypeDescriptorContext context, object value, Attribute[] attributes);
}
