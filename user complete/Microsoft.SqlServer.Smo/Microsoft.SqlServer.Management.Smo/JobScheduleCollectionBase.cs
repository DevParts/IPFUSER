using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.Agent;

namespace Microsoft.SqlServer.Management.Smo;

public class JobScheduleCollectionBase : ArrayListCollectionBase
{
	public JobSchedule this[Guid scheduleuid]
	{
		get
		{
			if (!initialized && base.ParentInstance.State == SqlSmoState.Existing)
			{
				InitializeChildCollection();
			}
			foreach (JobSchedule item in base.InternalStorage)
			{
				if (item.ScheduleUid == scheduleuid)
				{
					return item;
				}
			}
			return null;
		}
	}

	internal JobScheduleCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoArrayList(new ScheduleObjectComparer(StringComparer), this);
	}

	internal void RemoveObject(string name, int id)
	{
		base.InternalStorage.Remove(new ScheduleObjectKey(name, id));
	}

	public bool Contains(string name)
	{
		return Contains(new ScheduleObjectKey(name, GetDefaultID()));
	}

	public bool Contains(string name, int id)
	{
		return Contains(new ScheduleObjectKey(name, id));
	}

	internal static int GetDefaultID()
	{
		return -1;
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("Name");
		if (attribute == null || (attribute.Length == 0 && !CanHaveEmptyName(urn)))
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("Name", urn.Type));
		}
		int id = GetDefaultID();
		string attribute2 = urn.GetAttribute("ID");
		if (attribute2 != null && attribute2.Length > 0)
		{
			id = int.Parse(attribute2, SmoApplication.DefaultCulture);
		}
		return new ScheduleObjectKey(attribute, id);
	}
}
