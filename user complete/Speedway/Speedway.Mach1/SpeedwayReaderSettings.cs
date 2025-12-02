using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Speedway.Mach1;

[Serializable]
public class SpeedwayReaderSettings : MarshalByRefObject
{
	[Serializable]
	public class Gen2Params
	{
		public int inventory_mode;

		public int session = 1;

		public int auto_set_mode = 2;

		public int mode_id = 2;
	}

	[Serializable]
	public class ReaderInformation
	{
		public string reader_name = "default";

		public int region;

		public int frequency_mode;

		public ushort[] frequencies;

		public ushort[] reduced_power_frequencies;

		public ushort frequency;

		public int lbt_time;

		public string software_ver = string.Empty;

		public string firmware_ver = string.Empty;

		public string fpga_ver = string.Empty;
	}

	[Serializable]
	public class Antenna
	{
		public bool enabled;

		public float power = 30f;

		public short rssi = -60;
	}

	public Gen2Params gen2_params;

	public ReaderInformation reader_information;

	public Antenna[] antennas = new Antenna[4];

	public bool maximum_sensitivity = true;

	public bool enable_buffering;

	public SpeedwayReaderSettings()
	{
		gen2_params = new Gen2Params();
		reader_information = new ReaderInformation();
		for (int i = 0; i < 4; i++)
		{
			antennas[i] = new Antenna();
		}
	}

	public SpeedwayReaderSettings(string xmlstring)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(xmlstring);
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(bytes, 0, bytes.Length);
		memoryStream.Position = 0L;
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(SpeedwayReaderSettings));
		SpeedwayReaderSettings speedwayReaderSettings = (SpeedwayReaderSettings)xmlSerializer.Deserialize(memoryStream);
		gen2_params = speedwayReaderSettings.gen2_params;
		reader_information = speedwayReaderSettings.reader_information;
		antennas = speedwayReaderSettings.antennas;
		maximum_sensitivity = speedwayReaderSettings.maximum_sensitivity;
		enable_buffering = speedwayReaderSettings.enable_buffering;
	}

	public override string ToString()
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(SpeedwayReaderSettings));
		MemoryStream memoryStream = new MemoryStream();
		XmlTextWriter xmlWriter = new XmlTextWriter(memoryStream, Encoding.ASCII);
		xmlSerializer.Serialize(xmlWriter, this);
		byte[] buffer = memoryStream.GetBuffer();
		return Encoding.ASCII.GetString(buffer);
	}
}
