using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SimpleObjectKey : ObjectKeyBase
{
	protected string name;

	internal static readonly StringCollection fields;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public override string UrnFilter => string.Format(SmoApplication.DefaultCulture, "@Name='{0}'", new object[1] { Urn.EscapeString(name) });

	public override bool IsNull => null == name;

	public SimpleObjectKey(string name)
	{
		this.name = name;
	}

	static SimpleObjectKey()
	{
		fields = new StringCollection();
		fields.Add("Name");
	}

	public override string ToString()
	{
		return string.Format(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(name) });
	}

	public override string GetExceptionName()
	{
		return name;
	}

	public override StringCollection GetFieldNames()
	{
		return fields;
	}

	public override ObjectKeyBase Clone()
	{
		return new SimpleObjectKey(Name);
	}

	internal override void Validate(Type objectType)
	{
		bool flag = objectType.Equals(typeof(UserDefinedAggregateParameter)) || objectType.Equals(typeof(UserDefinedFunctionParameter));
		if (Name == null || (Name.Length == 0 && !flag))
		{
			throw new UnsupportedObjectNameException(ExceptionTemplatesImpl.UnsupportedObjectNameExceptionText(objectType.ToString())).SetHelpContext("UnsupportedObjectNameExceptionText");
		}
	}

	public override ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new SimpleObjectComparer(stringComparer);
	}
}
