namespace Microsoft.SqlServer.Management.Sdk.Differencing.Impl;

internal class Pair<T> : IPair<T>
{
	private T source;

	private T target;

	public T Source => source;

	public T Target => target;

	public Pair(T source, T target)
	{
		this.source = source;
		this.target = target;
	}
}
