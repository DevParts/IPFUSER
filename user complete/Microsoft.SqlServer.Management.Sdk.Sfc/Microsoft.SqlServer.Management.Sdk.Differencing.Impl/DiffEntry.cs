using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.Impl;

internal class DiffEntry : IDiffEntry
{
	private DiffType changeType;

	private Urn source;

	private Urn target;

	private IDictionary<string, IPair<object>> properties;

	public DiffType ChangeType
	{
		get
		{
			return changeType;
		}
		set
		{
			changeType = value;
		}
	}

	public Urn Source
	{
		get
		{
			return source;
		}
		set
		{
			source = value;
		}
	}

	public Urn Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
		}
	}

	public IDictionary<string, IPair<object>> Properties
	{
		get
		{
			if (properties == null)
			{
				properties = new Dictionary<string, IPair<object>>();
			}
			return properties;
		}
		set
		{
			properties = value;
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(ChangeType.ToString());
		stringBuilder.Append("{");
		stringBuilder.Append(string.Concat(source, ", ", target));
		if (ChangeType == DiffType.Updated)
		{
			stringBuilder.Append("- (" + properties.Count + ")");
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}
}
