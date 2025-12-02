using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SchemaObjectKey : SimpleObjectKey
{
	private string schema;

	internal static StringCollection schemaFields;

	public string Schema
	{
		get
		{
			return schema;
		}
		set
		{
			schema = value;
		}
	}

	public override string UrnFilter
	{
		get
		{
			if (schema != null && schema.Length > 0)
			{
				return string.Format(SmoApplication.DefaultCulture, "@Name='{0}' and @Schema='{1}'", new object[2]
				{
					Urn.EscapeString(name),
					Urn.EscapeString(schema)
				});
			}
			return string.Format(SmoApplication.DefaultCulture, "@Name='{0}'", new object[1] { Urn.EscapeString(name) });
		}
	}

	public override bool IsNull
	{
		get
		{
			if (name != null)
			{
				return null == schema;
			}
			return true;
		}
	}

	public SchemaObjectKey(string name, string schema)
		: base(name)
	{
		this.schema = schema;
	}

	static SchemaObjectKey()
	{
		schemaFields = new StringCollection();
		schemaFields.Add("Schema");
		schemaFields.Add("Name");
	}

	public override string ToString()
	{
		if (schema != null)
		{
			return string.Format(SmoApplication.DefaultCulture, "[{0}].[{1}]", new object[2]
			{
				SqlSmoObject.SqlBraket(schema),
				SqlSmoObject.SqlBraket(name)
			});
		}
		return name;
	}

	public override string GetExceptionName()
	{
		if (schema != null)
		{
			return string.Format(SmoApplication.DefaultCulture, "{0}.{1}", new object[2] { schema, name });
		}
		return name;
	}

	public override StringCollection GetFieldNames()
	{
		return schemaFields;
	}

	public override ObjectKeyBase Clone()
	{
		return new SchemaObjectKey(base.Name, Schema);
	}

	internal override void Validate(Type objectType)
	{
		if (base.Name == null || base.Name.Length == 0)
		{
			throw new UnsupportedObjectNameException(ExceptionTemplatesImpl.UnsupportedObjectNameExceptionText(objectType.ToString())).SetHelpContext("UnsupportedObjectNameExceptionText");
		}
		if ("Microsoft.SqlServer.Management.Smo.Table" == objectType.ToString())
		{
			Table.CheckTableName(base.Name);
		}
	}

	public override ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new SchemaObjectComparer(stringComparer);
	}
}
