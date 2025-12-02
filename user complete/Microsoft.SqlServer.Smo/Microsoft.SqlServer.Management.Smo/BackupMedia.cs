using System;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class BackupMedia
{
	private string mediaName = string.Empty;

	private DeviceType mediaType = DeviceType.File;

	private byte familySequenceNumber;

	private string credentialName;

	private byte mirrorSequenceNumber;

	internal Guid mediaSetId = default(Guid);

	internal Guid mediaFamilyId = default(Guid);

	internal Exception ReadException;

	private DataTable header;

	private DataTable label;

	public string MediaName => mediaName;

	public DeviceType MediaType => mediaType;

	public byte FamilySequenceNumber
	{
		get
		{
			if (label != null && familySequenceNumber == 0)
			{
				familySequenceNumber = (byte)(int)label.Rows[0]["FamilySequenceNumber"];
			}
			return familySequenceNumber;
		}
	}

	public string CredentialName
	{
		get
		{
			return credentialName;
		}
		set
		{
			credentialName = value;
		}
	}

	public byte MirrorSequenceNumber => mirrorSequenceNumber;

	internal ServerVersion BackupUrlDeviceSupportedServerVersion => new ServerVersion(11, 0, 3339);

	internal BackupMedia(DataRow dr)
	{
		switch ((byte)dr["device_type"])
		{
		case 2:
			mediaType = DeviceType.File;
			break;
		case 5:
			mediaType = DeviceType.Tape;
			break;
		case 6:
			mediaType = DeviceType.Pipe;
			break;
		case 9:
			mediaType = DeviceType.Url;
			break;
		case 102:
		case 105:
		case 106:
			mediaType = DeviceType.LogicalDevice;
			break;
		}
		if (MediaType == DeviceType.LogicalDevice)
		{
			mediaName = (string)dr["logical_device_name"];
		}
		else
		{
			mediaName = (string)dr["physical_device_name"];
		}
		familySequenceNumber = (byte)dr["family_sequence_number"];
		try
		{
			mirrorSequenceNumber = (byte)dr["mirror"];
		}
		catch (Exception)
		{
		}
	}

	internal BackupMedia()
	{
	}

	internal BackupMedia(string name, DeviceType backupMediaType)
	{
		mediaName = name;
		mediaType = backupMediaType;
	}

	internal BackupMedia(string name, DeviceType backupMediaType, string credentialName)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SetName, this, new ArgumentNullException("MediaName"));
		}
		mediaName = name;
		mediaType = backupMediaType;
		this.credentialName = credentialName;
	}

	internal DataTable MediaHeader(Server server)
	{
		if (header == null && ReadException == null)
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "RESTORE HEADERONLY FROM {0}", new object[1] { GetBackupMediaNameForScript(MediaName, MediaType) });
			stringBuilder.Append(" WITH ");
			if (AddCredential(server.ServerVersion, stringBuilder))
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.Append(" NOUNLOAD");
			try
			{
				header = server.ExecutionManager.ExecuteWithResults(stringBuilder.ToString()).Tables[0];
			}
			catch (Exception readException)
			{
				ReadException = readException;
			}
		}
		return header;
	}

	internal DataTable MediaLabel(Server server)
	{
		if (label == null && ReadException == null)
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "RESTORE LABELONLY FROM {0}", new object[1] { GetBackupMediaNameForScript(MediaName, MediaType) });
			stringBuilder.Append(" WITH ");
			if (AddCredential(server.ServerVersion, stringBuilder))
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.Append(" NOUNLOAD");
			try
			{
				label = server.ExecutionManager.ExecuteWithResults(stringBuilder.ToString()).Tables[0];
			}
			catch (Exception readException)
			{
				ReadException = readException;
			}
		}
		return label;
	}

	internal static string GetBackupMediaNameForScript(string name, DeviceType type)
	{
		string text = null;
		bool flag = false;
		switch (type)
		{
		case DeviceType.Tape:
			text = " TAPE = N'{0}'";
			flag = false;
			break;
		case DeviceType.File:
			text = " DISK = N'{0}'";
			flag = false;
			break;
		case DeviceType.LogicalDevice:
			text = " [{0}]";
			flag = true;
			break;
		case DeviceType.VirtualDevice:
			text = " VIRTUAL_DEVICE = N'{0}'";
			flag = false;
			break;
		case DeviceType.Pipe:
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.PipeDeviceNotSupported);
		case DeviceType.Url:
			text = " URL = N'{0}'";
			flag = false;
			break;
		default:
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("DeviceType"));
		}
		return string.Format(SmoApplication.DefaultCulture, text, new object[1] { flag ? SqlSmoObject.SqlBraket(name) : SqlSmoObject.SqlString(name) });
	}

	internal bool AddCredential(ServerVersion targetVersion, StringBuilder sb)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(credentialName))
		{
			if (!IsBackupUrlDeviceSupported(targetVersion))
			{
				throw new UnsupportedFeatureException(ExceptionTemplatesImpl.CredentialNotSupportedError(credentialName, targetVersion.ToString(), BackupUrlDeviceSupportedServerVersion.ToString()));
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, " CREDENTIAL = N'{0}' ", new object[1] { SqlSmoObject.SqlString(credentialName) });
			result = true;
		}
		return result;
	}

	internal bool IsBackupUrlDeviceSupported(ServerVersion currentServerVersion)
	{
		bool result = false;
		if (currentServerVersion.Major > BackupUrlDeviceSupportedServerVersion.Major)
		{
			result = true;
		}
		else if (currentServerVersion.Major == BackupUrlDeviceSupportedServerVersion.Major && currentServerVersion.Minor >= BackupUrlDeviceSupportedServerVersion.Minor && currentServerVersion.BuildNumber >= BackupUrlDeviceSupportedServerVersion.BuildNumber)
		{
			result = true;
		}
		return result;
	}
}
