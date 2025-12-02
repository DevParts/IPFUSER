using System;
using System.Xml;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class VersionSerializationAdapter : IXmlSerializationAdapter
{
	public void WriteXml(XmlWriter writer, object objectToSerialize)
	{
		if (objectToSerialize == null)
		{
			throw new ArgumentNullException("objectToSerialize");
		}
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		Version version = objectToSerialize as Version;
		if (version == null)
		{
			throw new ArgumentException(ExceptionTemplatesImpl.InvalidSerializerAdapterFound("VersionSerializationAdapter", typeof(Version).FullName, objectToSerialize.GetType().FullName));
		}
		writer.WriteElementString(typeof(Version).FullName, version.ToString());
	}

	public void ReadXml(XmlReader reader, out object deserializedObject)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		reader.MoveToContent();
		string text = reader.ReadElementContentAsString();
		try
		{
			Version version = new Version(text);
			deserializedObject = version;
		}
		catch (ArgumentNullException)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.InvalidConversionError("VersionSerializationAdapter", "null", typeof(Version).FullName));
		}
		catch (ArgumentException)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.InvalidConversionError("VersionSerializationAdapter", text, typeof(Version).FullName));
		}
	}
}
