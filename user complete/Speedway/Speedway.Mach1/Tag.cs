using System;
using System.Xml.Serialization;

namespace Speedway.Mach1;

[Serializable]
[XmlInclude(typeof(Tag))]
public class Tag
{
	[XmlElement("EPC")]
	public string epc;

	[XmlElement("Reader")]
	public string reader_name;

	[XmlElement("Antenna")]
	public int antenna;

	[XmlElement("Rssi")]
	public int rssi;

	[XmlElement("PhaseI")]
	public int phaseI;

	[XmlElement("PhaseQ")]
	public int phaseQ;

	[XmlElement("Frequency")]
	public float frequency;

	[XmlElement("TimeStamp")]
	public TIME_STAMP timeStamp = new TIME_STAMP();

	public override string ToString()
	{
		return $"<Tag><ID>{epc}</ID><Reader>{reader_name}</Reader><Antenna>{antenna}</Antenna><RSSI>{rssi}</RSSI><TimeStamp><Second>{timeStamp.seconds}</Second><u_Second>{timeStamp.u_seconds}</u_Second></TimeStamp></Tag>";
	}
}
