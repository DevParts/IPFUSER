using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.Impl;

internal abstract class Diffgram : IDiffgram, IEnumerable<IDiffEntry>, IEnumerable
{
	private object source;

	private object target;

	public object SourceRoot => source;

	public object TargetRoot => target;

	public Diffgram(object source, object target)
	{
		this.source = source;
		this.target = target;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public abstract IEnumerator<IDiffEntry> GetEnumerator();
}
