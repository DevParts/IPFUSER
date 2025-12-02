using System;
using System.ComponentModel;

namespace IPFUser;

[AttributeUsage(AttributeTargets.All)]
internal class LocalizableDescriptionAttribute : DescriptionAttribute
{
	private Type t;

	private bool Localized;

	public override string Description
	{
		get
		{
			if (!Localized)
			{
				Localized = true;
				string Txt = AppCSIUser.Rm.GetString(base.Description);
				if (Txt != null)
				{
					base.DescriptionValue = Txt;
				}
			}
			return base.Description;
		}
	}

	public LocalizableDescriptionAttribute(string Name, Type resBase)
		: base(Name)
	{
		Localized = false;
		t = resBase;
	}
}
