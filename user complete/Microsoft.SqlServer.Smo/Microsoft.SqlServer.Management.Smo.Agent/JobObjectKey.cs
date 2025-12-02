using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Agent;

internal class JobObjectKey : SimpleObjectKey
{
	private int categoryID;

	private static readonly JobObjectKeySingleton jobObjectKeySingleton;

	internal static StringCollection jobKeyFields => jobObjectKeySingleton.jobKeyFields;

	public int CategoryID
	{
		get
		{
			return categoryID;
		}
		set
		{
			categoryID = value;
		}
	}

	public override string UrnFilter
	{
		get
		{
			if (categoryID == -1)
			{
				return string.Format(SmoApplication.DefaultCulture, "@Name='{0}'", new object[1] { Urn.EscapeString(name) });
			}
			return string.Format(SmoApplication.DefaultCulture, "@Name='{0}' and @CategoryID='{1}'", new object[2]
			{
				Urn.EscapeString(name),
				categoryID.ToString(SmoApplication.DefaultCulture)
			});
		}
	}

	public override bool IsNull => false;

	public JobObjectKey(string name, int categoryID)
		: base(name)
	{
		this.categoryID = categoryID;
	}

	static JobObjectKey()
	{
		jobObjectKeySingleton = new JobObjectKeySingleton();
		jobObjectKeySingleton.jobKeyFields = new StringCollection();
		jobObjectKeySingleton.jobKeyFields.Add("Name");
		jobObjectKeySingleton.jobKeyFields.Add("CategoryID");
	}

	public override string ToString()
	{
		return name;
	}

	public override StringCollection GetFieldNames()
	{
		return jobObjectKeySingleton.jobKeyFields;
	}

	public override ObjectKeyBase Clone()
	{
		return new JobObjectKey(name, categoryID);
	}

	internal override void Validate(Type objectType)
	{
	}

	public override ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new JobObjectComparer(stringComparer);
	}
}
