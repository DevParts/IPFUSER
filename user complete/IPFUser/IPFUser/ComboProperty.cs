using System.ComponentModel;
using Microsoft.VisualBasic;

namespace IPFUser;

public class ComboProperty : StringConverter
{
	public static Collection CurrentCol;

	public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	{
		return new StandardValuesCollection(CurrentCol);
	}

	public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
	{
		return true;
	}
}
