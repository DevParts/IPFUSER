using System.Xml;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IXmlSerializationAdapter
{
	void ReadXml(XmlReader reader, out object deserializedObject);

	void WriteXml(XmlWriter writer, object objectToSerialize);
}
