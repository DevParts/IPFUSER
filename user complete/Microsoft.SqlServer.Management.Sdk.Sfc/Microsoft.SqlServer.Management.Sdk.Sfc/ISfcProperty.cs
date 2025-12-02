using System;
using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcProperty
{
	string Name { get; }

	Type Type { get; }

	bool Enabled { get; }

	object Value { get; set; }

	bool Required { get; }

	bool Writable { get; }

	bool Dirty { get; }

	bool IsNull { get; }

	AttributeCollection Attributes { get; }
}
