using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class SfcChildren : ISfcSimpleList, IEnumerable<ISfcSimpleNode>, IEnumerable
{
	private readonly IEnumerable listReference;

	private readonly SimpleNodeAdapter adapter;

	public IEnumerable ListReference => listReference;

	public SfcChildren(object reference, string name, SimpleNodeAdapter adapter)
	{
		if (reference == null)
		{
			throw new ArgumentNullException("reference");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (adapter == null)
		{
			throw new ArgumentNullException("adapter");
		}
		listReference = adapter.CheckedGetEnumerable(reference, name);
		this.adapter = adapter;
	}

	public IEnumerator<ISfcSimpleNode> GetEnumerator()
	{
		return GetChildren();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	protected IEnumerator<ISfcSimpleNode> GetChildren()
	{
		if (listReference != null)
		{
			return new SfcChildrenEnumerator(listReference.GetEnumerator(), adapter);
		}
		return new List<ISfcSimpleNode>().GetEnumerator();
	}
}
