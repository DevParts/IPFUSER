using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class SfcChildrenEnumerator : IEnumerator<ISfcSimpleNode>, IDisposable, IEnumerator
{
	private readonly IEnumerator children;

	private readonly SimpleNodeAdapter adapter;

	private ISfcSimpleNode current;

	public ISfcSimpleNode Current
	{
		get
		{
			if (current == null)
			{
				current = new SfcSimpleNode(children.Current, adapter);
			}
			return current;
		}
	}

	object IEnumerator.Current => Current;

	public SfcChildrenEnumerator(IEnumerator children, SimpleNodeAdapter adapter)
	{
		this.children = children;
		this.adapter = adapter;
	}

	public bool MoveNext()
	{
		current = null;
		while (children.MoveNext())
		{
			object reference = children.Current;
			if (adapter.CheckedIsCriteriaMatched(reference))
			{
				return true;
			}
		}
		return false;
	}

	public void Reset()
	{
		current = null;
		children.Reset();
	}

	public void Dispose()
	{
		if (children is IDisposable disposable)
		{
			disposable.Dispose();
		}
		current = null;
	}
}
