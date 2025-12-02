using System;
using System.Collections.Specialized;
using System.Data;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
public class PlanGuideExtender : SmoObjectExtender<PlanGuide>, ISfcValidate
{
	private StringCollection schemaNames;

	[ExtendedProperty]
	public string Name
	{
		get
		{
			return base.Parent.Name;
		}
		set
		{
			base.Parent.Name = value;
		}
	}

	[ExtendedProperty]
	public StringCollection SchemaNames
	{
		get
		{
			if (schemaNames == null)
			{
				schemaNames = new StringCollection();
				Database database = base.Parent.Parent;
				if (database != null)
				{
					DataTable dataTable = database.EnumObjects(DatabaseObjectTypes.Schema);
					foreach (DataRow row in dataTable.Rows)
					{
						schemaNames.Add(row["Name"].ToString());
					}
				}
			}
			return schemaNames;
		}
	}

	public PlanGuideExtender()
	{
	}

	public PlanGuideExtender(PlanGuide planGuide)
		: base(planGuide)
	{
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		return base.Parent.Validate(methodName, arguments);
	}
}
