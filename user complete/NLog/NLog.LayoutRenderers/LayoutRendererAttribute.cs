using System;
using NLog.Config;

namespace NLog.LayoutRenderers;

/// <summary>
/// Marks class as layout-renderer and attaches a type-alias name for use in NLog configuration.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class LayoutRendererAttribute : NameBaseAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.LayoutRendererAttribute" /> class.
	/// </summary>
	/// <param name="name">The layout-renderer type-alias for use in NLog configuration - without '${ }'</param>
	public LayoutRendererAttribute(string name)
		: base(name)
	{
	}
}
