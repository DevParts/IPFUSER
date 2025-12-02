using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DatabaseReplicaStateCollectionBase : SortedListCollectionBase
{
	internal DatabaseReplicaStateCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoSortedList(new DatabaseReplicaStateObjectComparer(StringComparer));
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(DatabaseReplicaState);
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("AvailabilityDatabaseName");
		string attribute2 = urn.GetAttribute("AvailabilityReplicaServerName");
		if (string.IsNullOrEmpty(attribute))
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("AvailabilityDatabaseName", urn.Type));
		}
		if (string.IsNullOrEmpty(attribute2))
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("AvailabilityReplicaServerName", urn.Type));
		}
		return new DatabaseReplicaStateObjectKey(attribute2, attribute);
	}
}
