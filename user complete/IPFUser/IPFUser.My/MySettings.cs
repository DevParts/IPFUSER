using System;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser.My;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class MySettings : ApplicationSettingsBase
{
	private static MySettings defaultInstance = (MySettings)SettingsBase.Synchronized(new MySettings());

	private static bool addedHandler;

	private static object addedHandlerLockObject = RuntimeHelpers.GetObjectValue(new object());

	public static MySettings Default
	{
		get
		{
			if (!addedHandler)
			{
				object obj = addedHandlerLockObject;
				ObjectFlowControl.CheckForSyncLockOnValueType(obj);
				bool lockTaken = false;
				try
				{
					Monitor.Enter(obj, ref lockTaken);
					if (!addedHandler)
					{
						MyProject.Application.Shutdown += AutoSaveSettings;
						addedHandler = true;
					}
				}
				finally
				{
					if (lockTaken)
					{
						Monitor.Exit(obj);
					}
				}
			}
			return defaultInstance;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string ConnectionString
	{
		get
		{
			return Conversions.ToString(this["ConnectionString"]);
		}
		set
		{
			this["ConnectionString"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("IPFEu")]
	public string Catalog
	{
		get
		{
			return Conversions.ToString(this["Catalog"]);
		}
		set
		{
			this["Catalog"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("(local)\\sqlexpress")]
	public string DataServer
	{
		get
		{
			return Conversions.ToString(this["DataServer"]);
		}
		set
		{
			this["DataServer"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string SqlUser
	{
		get
		{
			return Conversions.ToString(this["SqlUser"]);
		}
		set
		{
			this["SqlUser"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string SqlPassword
	{
		get
		{
			return Conversions.ToString(this["SqlPassword"]);
		}
		set
		{
			this["SqlPassword"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool UseWindowsAuthentication
	{
		get
		{
			return Conversions.ToBoolean(this["UseWindowsAuthentication"]);
		}
		set
		{
			this["UseWindowsAuthentication"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("192.168.0.180")]
	public string LaserIP
	{
		get
		{
			return Conversions.ToString(this["LaserIP"]);
		}
		set
		{
			this["LaserIP"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("100")]
	public int LaserBufferSize
	{
		get
		{
			return Conversions.ToInteger(this["LaserBufferSize"]);
		}
		set
		{
			this["LaserBufferSize"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("50")]
	public int WaitTimeOnLaserQueueFull
	{
		get
		{
			return Conversions.ToInteger(this["WaitTimeOnLaserQueueFull"]);
		}
		set
		{
			this["WaitTimeOnLaserQueueFull"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("5")]
	public int WaitTime
	{
		get
		{
			return Conversions.ToInteger(this["WaitTime"]);
		}
		set
		{
			this["WaitTime"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string AppKey
	{
		get
		{
			return Conversions.ToString(this["AppKey"]);
		}
		set
		{
			this["AppKey"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("50")]
	public int LowCodes
	{
		get
		{
			return Conversions.ToInteger(this["LowCodes"]);
		}
		set
		{
			this["LowCodes"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("25")]
	public string VeryLowCodes
	{
		get
		{
			return Conversions.ToString(this["VeryLowCodes"]);
		}
		set
		{
			this["VeryLowCodes"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool ShowLowCodes
	{
		get
		{
			return Conversions.ToBoolean(this["ShowLowCodes"]);
		}
		set
		{
			this["ShowLowCodes"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool ShowVeryLowCodes
	{
		get
		{
			return Conversions.ToBoolean(this["ShowVeryLowCodes"]);
		}
		set
		{
			this["ShowVeryLowCodes"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("English")]
	public string Language
	{
		get
		{
			return Conversions.ToString(this["Language"]);
		}
		set
		{
			this["Language"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string LicenseAppKey
	{
		get
		{
			return Conversions.ToString(this["LicenseAppKey"]);
		}
		set
		{
			this["LicenseAppKey"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string LicenseCrc
	{
		get
		{
			return Conversions.ToString(this["LicenseCrc"]);
		}
		set
		{
			this["LicenseCrc"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string LicenseType
	{
		get
		{
			return Conversions.ToString(this["LicenseType"]);
		}
		set
		{
			this["LicenseType"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <string>192.168.1.100</string>\r\n  <string>502</string>\r\n</ArrayOfString>")]
	public StringCollection PLC
	{
		get
		{
			return (StringCollection)this["PLC"];
		}
		set
		{
			this["PLC"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public int Session
	{
		get
		{
			return Conversions.ToInteger(this["Session"]);
		}
		set
		{
			this["Session"] = value;
		}
	}

	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	private static void AutoSaveSettings(object sender, EventArgs e)
	{
		if (MyProject.Application.SaveMySettingsOnExit)
		{
			MySettingsProperty.Settings.Save();
		}
	}
}
