namespace Microsoft.SqlServer.Management.Smo;

internal class DataTypePreferences
{
	public bool UserDefinedDataTypesToBaseType { get; set; }

	public bool TimestampToBinary { get; set; }

	public bool XmlNamespaces { get; set; }

	internal DataTypePreferences()
	{
		Init();
	}

	private void Init()
	{
		XmlNamespaces = true;
	}

	internal object Clone()
	{
		return MemberwiseClone();
	}
}
