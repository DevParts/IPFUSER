using System;

namespace Microsoft.SqlServer.Management.Common;

public interface ISfcConnection
{
	bool IsOpen { get; }

	string ServerInstance { get; set; }

	Version ServerVersion { get; }

	bool IsForceDisconnected { get; }

	bool Connect();

	bool Disconnect();

	ISfcConnection Copy();

	object ToEnumeratorObject();

	void ForceDisconnected();
}
