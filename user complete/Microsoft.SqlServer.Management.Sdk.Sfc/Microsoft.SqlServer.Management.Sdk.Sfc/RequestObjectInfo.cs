using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
[ComVisible(false)]
public class RequestObjectInfo
{
	[Flags]
	public enum Flags
	{
		None = 0,
		Properties = 1,
		Children = 2,
		Parents = 4,
		ResultTypes = 8,
		UrnProperties = 0x10,
		All = 0xF
	}

	private Urn m_urn;

	private Flags m_flags;

	[XmlElement]
	public Urn Urn
	{
		get
		{
			return m_urn;
		}
		set
		{
			m_urn = value;
		}
	}

	[XmlAttribute]
	public Flags InfoType
	{
		get
		{
			return m_flags;
		}
		set
		{
			m_flags = value;
		}
	}

	public RequestObjectInfo()
	{
	}

	public RequestObjectInfo(Urn urn, Flags infoType)
	{
		Urn = urn;
		InfoType = infoType;
	}
}
