namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public struct SfcInstanceSerializedData
{
	private SfcSerializedTypes serializedType;

	private string name;

	private string type;

	private object value;

	public SfcSerializedTypes SerializedType => serializedType;

	public string Name => name;

	public string Type => type;

	public object Value => value;

	public SfcInstanceSerializedData(SfcSerializedTypes serializedType, string name, string type, object value)
	{
		this.serializedType = serializedType;
		this.name = name;
		this.type = type;
		this.value = value;
	}
}
