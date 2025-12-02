using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
public class BackupDeviceList : List<BackupDeviceItem>
{
	public BackupDeviceList()
	{
	}

	public BackupDeviceList(IEnumerable<BackupDeviceItem> collection)
		: base(collection)
	{
	}

	public BackupDeviceList(int capacity)
		: base(capacity)
	{
	}

	public void AddDevice(string name, DeviceType deviceType)
	{
		if (name == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddDevice, this, new ArgumentNullException("name"));
		}
		Add(new BackupDeviceItem(name, deviceType));
	}
}
