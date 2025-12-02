using System;
using System.Text;

namespace NLog.Internal;

/// <summary>
/// Allocates new builder and appends to the provided target builder on dispose
/// </summary>
internal struct AppendBuilderCreator : IDisposable
{
	private static readonly StringBuilderPool _builderPool = new StringBuilderPool(Environment.ProcessorCount * 2);

	private StringBuilderPool.ItemHolder _builder;

	/// <summary>
	/// Access the new builder allocated
	/// </summary>
	public StringBuilder Builder => _builder.Item;

	public AppendBuilderCreator(bool mustBeEmpty)
	{
		_builder = _builderPool.Acquire();
	}

	public void Dispose()
	{
		_builder.Dispose();
	}
}
