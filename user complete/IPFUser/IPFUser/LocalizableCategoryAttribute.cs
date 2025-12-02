using System;
using System.ComponentModel;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser;

[AttributeUsage(AttributeTargets.All)]
internal class LocalizableCategoryAttribute : CategoryAttribute
{
	public LocalizableCategoryAttribute(string Name)
		: base(Name)
	{
	}

	protected override string GetLocalizedString(string Value)
	{
		string GetLocalizedString;
		try
		{
			GetLocalizedString = AppCSIUser.Rm.GetString(Value);
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			GetLocalizedString = Value;
			ProjectData.ClearProjectError();
		}
		return GetLocalizedString;
	}
}
