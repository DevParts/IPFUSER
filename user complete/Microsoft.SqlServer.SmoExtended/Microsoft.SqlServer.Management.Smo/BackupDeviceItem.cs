using System;
using System.Data;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class BackupDeviceItem : IComparable
{
	private BackupMedia backupMedia;

	internal BackupMedia BackupMedia => backupMedia;

	public string Name
	{
		get
		{
			return BackupMedia.MediaName;
		}
		set
		{
			if (value == null)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.SetName, this, new ArgumentNullException("Name"));
			}
			BackupMedia backupMedia = new BackupMedia(value, BackupMedia.MediaType);
			this.backupMedia = backupMedia;
		}
	}

	public DeviceType DeviceType
	{
		get
		{
			return BackupMedia.MediaType;
		}
		set
		{
			BackupMedia backupMedia = new BackupMedia(BackupMedia.MediaName, value);
			this.backupMedia = backupMedia;
		}
	}

	public string CredentialName
	{
		get
		{
			return BackupMedia.CredentialName;
		}
		set
		{
			BackupMedia backupMedia = new BackupMedia(BackupMedia.MediaName, BackupMedia.MediaType, value);
			this.backupMedia = backupMedia;
		}
	}

	public BackupDeviceItem()
	{
		backupMedia = new BackupMedia();
	}

	public BackupDeviceItem(string name, DeviceType deviceType)
	{
		if (name == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SetName, this, new ArgumentNullException("Name"));
		}
		backupMedia = new BackupMedia(name, deviceType);
	}

	public BackupDeviceItem(string name, DeviceType deviceType, string credentialName)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SetName, this, new ArgumentNullException("Name"));
		}
		backupMedia = new BackupMedia(name, deviceType, credentialName);
	}

	internal DataTable DeviceHeader(Server server)
	{
		return BackupMedia.MediaHeader(server);
	}

	internal DataTable DeviceLabel(Server server)
	{
		return BackupMedia.MediaLabel(server);
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		CheckType(obj, ExceptionTemplatesImpl.Compare, this);
		return string.Compare(Name, ((BackupDeviceItem)obj).Name, StringComparison.OrdinalIgnoreCase);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is BackupDeviceItem))
		{
			return false;
		}
		return CompareTo(obj) == 0;
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}

	public static bool operator ==(BackupDeviceItem r1, BackupDeviceItem r2)
	{
		return r1.Equals(r2);
	}

	public static bool operator !=(BackupDeviceItem r1, BackupDeviceItem r2)
	{
		return !(r1 == r2);
	}

	public static bool operator <(BackupDeviceItem r1, BackupDeviceItem r2)
	{
		return r1.CompareTo(r2) < 0;
	}

	public static bool operator >(BackupDeviceItem r1, BackupDeviceItem r2)
	{
		return r1.CompareTo(r2) > 0;
	}

	internal static void CheckType(object obj, string operation, object thisptr)
	{
		TraceHelper.Assert(operation != null && operation.Length > 0);
		TraceHelper.Assert(null != thisptr);
		if (obj == null)
		{
			throw new FailedOperationException(operation, thisptr, new ArgumentNullException());
		}
		if (!(obj is BackupDeviceItem))
		{
			throw new FailedOperationException(operation, thisptr, new NotSupportedException(ExceptionTemplatesImpl.InvalidType(obj.GetType().ToString())));
		}
	}
}
