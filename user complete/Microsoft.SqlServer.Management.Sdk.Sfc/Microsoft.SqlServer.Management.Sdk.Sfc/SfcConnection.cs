using System;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class SfcConnection : ISfcConnection
{
	public abstract bool IsOpen { get; }

	public abstract string ServerInstance { get; set; }

	public abstract Version ServerVersion { get; set; }

	public abstract ServerType ConnectionType { get; }

	public abstract int ConnectTimeout { get; set; }

	public abstract int StatementTimeout { get; set; }

	public virtual bool IsForceDisconnected => false;

	public abstract override int GetHashCode();

	public abstract bool Equals(SfcConnection connection);

	public abstract bool Connect();

	public abstract bool Disconnect();

	public abstract ISfcConnection Copy();

	public virtual void ForceDisconnected()
	{
	}

	public virtual object ToEnumeratorObject()
	{
		return this;
	}
}
