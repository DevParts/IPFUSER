using System;
using System.Collections.Generic;
using NLog.LayoutRenderers;

namespace NLog.Config;

/// <summary>
/// Factory specialized for <see cref="T:NLog.LayoutRenderers.LayoutRenderer" />s.
/// </summary>
internal sealed class LayoutRendererFactory : Factory<LayoutRenderer, LayoutRendererAttribute>
{
	private readonly Dictionary<string, FuncLayoutRenderer> _funcRenderers = new Dictionary<string, FuncLayoutRenderer>(StringComparer.OrdinalIgnoreCase);

	public LayoutRendererFactory(ConfigurationItemFactory parentFactory)
		: base(parentFactory)
	{
	}

	/// <inheritdoc />
	public override void Clear()
	{
		_funcRenderers.Clear();
		base.Clear();
	}

	/// <summary>
	/// Register a layout renderer with a callback function.
	/// </summary>
	/// <param name="itemName">Name of the layoutrenderer, without ${}.</param>
	/// <param name="renderer">the renderer that renders the value.</param>
	public void RegisterFuncLayout(string itemName, FuncLayoutRenderer renderer)
	{
		itemName = FactoryExtensions.NormalizeName(itemName);
		lock (ConfigurationItemFactory.SyncRoot)
		{
			_funcRenderers[itemName] = renderer;
		}
	}

	/// <inheritdoc />
	public override bool TryCreateInstance(string typeAlias, out LayoutRenderer? result)
	{
		typeAlias = FactoryExtensions.NormalizeName(typeAlias);
		if (_funcRenderers.Count > 0)
		{
			lock (ConfigurationItemFactory.SyncRoot)
			{
				if (_funcRenderers.TryGetValue(typeAlias, out FuncLayoutRenderer value))
				{
					result = value;
					return true;
				}
			}
		}
		return base.TryCreateInstance(typeAlias, out result);
	}
}
