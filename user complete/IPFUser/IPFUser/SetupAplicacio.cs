using System.ComponentModel;
using System.Windows.Forms;
using IPFUser.My;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser;

public class SetupAplicacio
{
	/// <summary>
	/// Versión de Aplicación
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	[LocalizableDescription("String64", typeof(string))]
	[LocalizableCategory("String65")]
	[ReadOnly(true)]
	public string AppVersion => Application.ProductVersion;

	[LocalizableDescription("String84", typeof(string))]
	[LocalizableCategory("String65")]
	[TypeConverter(typeof(ComboProperty))]
	public string Language
	{
		get
		{
			ComboProperty.CurrentCol = CTRL_PORTS.Idiomas;
			return MySettingsProperty.Settings.Language;
		}
		set
		{
			MySettingsProperty.Settings.Language = value;
		}
	}

	[LocalizableDescription("String67", typeof(string))]
	[LocalizableCategory("String66")]
	[ReadOnly(true)]
	public string ConnectionString => MySettingsProperty.Settings.ConnectionString;

	[LocalizableDescription("String68", typeof(string))]
	[LocalizableCategory("String66")]
	[ReadOnly(false)]
	public string Catalog
	{
		get
		{
			return MySettingsProperty.Settings.Catalog;
		}
		set
		{
			MySettingsProperty.Settings.Catalog = value;
		}
	}

	[LocalizableDescription("String69", typeof(string))]
	[LocalizableCategory("String66")]
	[ReadOnly(false)]
	public string DataSource
	{
		get
		{
			return MySettingsProperty.Settings.DataServer;
		}
		set
		{
			MySettingsProperty.Settings.DataServer = value;
		}
	}

	[LocalizableDescription("String70", typeof(string))]
	[LocalizableCategory("String66")]
	[ReadOnly(false)]
	public string User
	{
		get
		{
			return MySettingsProperty.Settings.SqlUser;
		}
		set
		{
			MySettingsProperty.Settings.SqlUser = value;
		}
	}

	[LocalizableDescription("String71", typeof(string))]
	[LocalizableCategory("String66")]
	[ReadOnly(false)]
	public string Password
	{
		get
		{
			return MySettingsProperty.Settings.SqlPassword;
		}
		set
		{
			MySettingsProperty.Settings.SqlPassword = value;
		}
	}

	[LocalizableDescription("String72", typeof(string))]
	[LocalizableCategory("String66")]
	[ReadOnly(false)]
	public bool UseWindowsAuthentication
	{
		get
		{
			return MySettingsProperty.Settings.UseWindowsAuthentication;
		}
		set
		{
			MySettingsProperty.Settings.UseWindowsAuthentication = value;
		}
	}

	[LocalizableDescription("String74", typeof(string))]
	[LocalizableCategory("String73")]
	public int WaitTime
	{
		get
		{
			return MySettingsProperty.Settings.WaitTime;
		}
		set
		{
			MySettingsProperty.Settings.WaitTime = value;
		}
	}

	[LocalizableDescription("String75", typeof(string))]
	[LocalizableCategory("String73")]
	public int WaitTimeBufferFull
	{
		get
		{
			return MySettingsProperty.Settings.WaitTimeOnLaserQueueFull;
		}
		set
		{
			MySettingsProperty.Settings.WaitTimeOnLaserQueueFull = value;
		}
	}

	[LocalizableDescription("String77", typeof(string))]
	[LocalizableCategory("String76")]
	public string Laser_IP
	{
		get
		{
			return MySettingsProperty.Settings.LaserIP;
		}
		set
		{
			MySettingsProperty.Settings.LaserIP = value;
		}
	}

	[LocalizableDescription("String78", typeof(string))]
	[LocalizableCategory("String76")]
	public int LaserBufferSize
	{
		get
		{
			return MySettingsProperty.Settings.LaserBufferSize;
		}
		set
		{
			MySettingsProperty.Settings.LaserBufferSize = value;
		}
	}

	[LocalizableDescription("String80", typeof(string))]
	[LocalizableCategory("String79")]
	public int LowLevelWarning
	{
		get
		{
			return MySettingsProperty.Settings.LowCodes;
		}
		set
		{
			if (value > 100)
			{
				value = 100;
			}
			if (value < 0)
			{
				value = 0;
			}
			MySettingsProperty.Settings.LowCodes = value;
		}
	}

	[LocalizableDescription("String82", typeof(string))]
	[LocalizableCategory("String79")]
	public bool ShowLowLevels
	{
		get
		{
			return MySettingsProperty.Settings.ShowLowCodes;
		}
		set
		{
			MySettingsProperty.Settings.ShowLowCodes = value;
		}
	}

	[LocalizableDescription("String81", typeof(string))]
	[LocalizableCategory("String79")]
	public int VeryLowLevelWarning
	{
		get
		{
			return Conversions.ToInteger(MySettingsProperty.Settings.VeryLowCodes);
		}
		set
		{
			if (value > 100)
			{
				value = 100;
			}
			if (value < 0)
			{
				value = 0;
			}
			MySettingsProperty.Settings.VeryLowCodes = Conversions.ToString(value);
		}
	}

	[LocalizableDescription("String83", typeof(string))]
	[LocalizableCategory("String79")]
	public bool ShowVeryLowLevels
	{
		get
		{
			return MySettingsProperty.Settings.ShowVeryLowCodes;
		}
		set
		{
			MySettingsProperty.Settings.ShowVeryLowCodes = value;
		}
	}

	[LocalizableDescription("String91", typeof(string))]
	[Category("PLC")]
	public string PLCIp
	{
		get
		{
			return MySettingsProperty.Settings.PLC[0];
		}
		set
		{
			MySettingsProperty.Settings.PLC[0] = value;
		}
	}

	[LocalizableDescription("String92", typeof(string))]
	[Category("PLC")]
	public string PLCPort
	{
		get
		{
			return MySettingsProperty.Settings.PLC[1];
		}
		set
		{
			MySettingsProperty.Settings.PLC[1] = value;
		}
	}

	public SetupAplicacio()
	{
		CTRL_PORTS.SpeedPorts.Clear();
		CTRL_PORTS.SpeedPorts.Add("9600");
		CTRL_PORTS.SpeedPorts.Add("19200");
		CTRL_PORTS.SpeedPorts.Add("38400");
		CTRL_PORTS.SpeedPorts.Add("57600");
		CTRL_PORTS.SpeedPorts.Add("115200");
		CTRL_PORTS.ConnectionPortType.Clear();
		CTRL_PORTS.ConnectionPortType.Add("RS232");
		CTRL_PORTS.ConnectionPortType.Add("TCP/IP");
		CTRL_PORTS.ComPorts.Clear();
		foreach (string Port in MyProject.Computer.Ports.SerialPortNames)
		{
			CTRL_PORTS.ComPorts.Add(Port.ToString());
		}
		CTRL_PORTS.TipoLinx.Clear();
		CTRL_PORTS.TipoLinx.Add("6200");
		CTRL_PORTS.TipoLinx.Add("6800");
		CTRL_PORTS.TipoLinx.Add("6900");
		CTRL_PORTS.PinsIO.Clear();
		int i = 1;
		do
		{
			CTRL_PORTS.PinsIO.Add(i.ToString());
			i = checked(i + 1);
		}
		while (i <= 8);
		CTRL_PORTS.Idiomas.Clear();
		CTRL_PORTS.Idiomas.Add("English");
		CTRL_PORTS.Idiomas.Add("Español");
	}
}
