using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IDynamicReadOnly
{
	event EventHandler<ReadOnlyPropertyChangedEventArgs> ReadOnlyPropertyChanged;

	void OverrideReadOnly(IList<LocalizablePropertyDescriptor> properties, ITypeDescriptorContext context, object value, Attribute[] attributes);
}
