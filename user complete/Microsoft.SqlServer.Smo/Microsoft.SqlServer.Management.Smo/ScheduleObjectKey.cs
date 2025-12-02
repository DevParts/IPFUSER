using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class ScheduleObjectKey : SimpleObjectKey
{
	private int id;

	internal static StringCollection schemaFields;

	public int ID
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public override string UrnFilter
	{
		get
		{
			if (id > -1)
			{
				return string.Format(SmoApplication.DefaultCulture, "@Name='{0}' and @ID='{1}'", new object[2]
				{
					Urn.EscapeString(name),
					id
				});
			}
			return string.Format(SmoApplication.DefaultCulture, "@Name='{0}'", new object[1] { Urn.EscapeString(name) });
		}
	}

	public override bool IsNull => null == name;

	public ScheduleObjectKey(string name, int id)
		: base(name)
	{
		this.id = id;
	}

	static ScheduleObjectKey()
	{
		schemaFields = new StringCollection();
		schemaFields.Add("Name");
		schemaFields.Add("ID");
	}

	public override string ToString()
	{
		return name;
	}

	public override StringCollection GetFieldNames()
	{
		return schemaFields;
	}

	public override ObjectKeyBase Clone()
	{
		return new ScheduleObjectKey(base.Name, ID);
	}

	internal override void Validate(Type objectType)
	{
		if (base.Name == null || base.Name.Length == 0)
		{
			throw new UnsupportedObjectNameException(ExceptionTemplatesImpl.UnsupportedObjectNameExceptionText(objectType.ToString())).SetHelpContext("UnsupportedObjectNameExceptionText");
		}
	}

	public override ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new ScheduleObjectComparer(stringComparer);
	}
}
