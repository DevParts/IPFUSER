using System;
using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(SearchPropertyIntIDTypeConverter))]
internal class SearchPropertyIntIDType : IComparable
{
	private string value;

	public SearchPropertyIntIDType(string value)
	{
		this.value = value;
	}

	int IComparable.CompareTo(object obj)
	{
		return ParseInt(value) - ParseInt((obj as SearchPropertyIntIDType).value);
	}

	public override string ToString()
	{
		return value;
	}

	private int ParseInt(string proposedValue)
	{
		int result = int.MinValue;
		if (!string.IsNullOrEmpty(proposedValue))
		{
			try
			{
				result = Convert.ToInt32(proposedValue, SmoApplication.DefaultCulture);
			}
			catch (FormatException)
			{
				result = int.MinValue;
			}
		}
		return result;
	}
}
