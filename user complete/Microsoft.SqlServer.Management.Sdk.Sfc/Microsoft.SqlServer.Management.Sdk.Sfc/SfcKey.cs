using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class SfcKey : IEquatable<SfcKey>
{
	public virtual Type InstanceType => GetType().DeclaringType;

	public abstract override bool Equals(object obj);

	public abstract override int GetHashCode();

	public override string ToString()
	{
		return GetUrnFragment();
	}

	public abstract bool Equals(SfcKey akey);

	public abstract string GetUrnFragment();
}
