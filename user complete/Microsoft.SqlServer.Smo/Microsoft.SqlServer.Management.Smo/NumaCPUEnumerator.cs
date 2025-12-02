using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class NumaCPUEnumerator : IEnumerator
{
	private int idx;

	private SortedList col;

	object IEnumerator.Current => col[idx];

	internal NumaCPUEnumerator(SortedList col)
	{
		idx = -1;
		this.col = col;
	}

	bool IEnumerator.MoveNext()
	{
		return ++idx < col.Count;
	}

	void IEnumerator.Reset()
	{
		idx = -1;
	}
}
