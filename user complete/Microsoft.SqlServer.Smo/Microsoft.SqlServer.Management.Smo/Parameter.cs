using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public class Parameter : ParameterBase, ISfcSupportsDesignMode
{
	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string DefaultValue
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultValue");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultValue", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsReadOnly
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsReadOnly");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsReadOnly", value);
		}
	}

	internal Parameter(AbstractCollectionBase parent, ObjectKeyBase key, SqlSmoState state)
		: base(parent, key, state)
	{
	}

	protected Parameter()
	{
	}
}
