using System;
using JetBrains.Annotations;

namespace NLog.Config;

/// <summary>
/// Attaches a type-alias for an item (such as <see cref="T:NLog.Targets.Target" />,
/// <see cref="T:NLog.LayoutRenderers.LayoutRenderer" />, <see cref="T:NLog.Layouts.Layout" />, etc.).
/// </summary>
[MeansImplicitUse]
public abstract class NameBaseAttribute : Attribute
{
	/// <summary>
	/// Gets the name of the type-alias
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.NameBaseAttribute" /> class.
	/// </summary>
	/// <param name="name">The type-alias for use in NLog configuration.</param>
	protected NameBaseAttribute(string name)
	{
		Name = name;
	}
}
