using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class DynamicValueTypeConverter : StringConverter
{
	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	{
		if (context.Instance is IDynamicValues)
		{
			return ((IDynamicValues)context.Instance).GetStandardValues(context);
		}
		return base.GetStandardValues(context);
	}

	public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
	{
		return true;
	}
}
