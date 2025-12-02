using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcPropertySet
{
	bool Contains(string propertyName);

	bool Contains(ISfcProperty property);

	bool Contains<T>(string name);

	bool TryGetPropertyValue<T>(string name, out T value);

	bool TryGetPropertyValue(string name, out object value);

	bool TryGetProperty(string name, out ISfcProperty property);

	IEnumerable<ISfcProperty> EnumProperties();
}
