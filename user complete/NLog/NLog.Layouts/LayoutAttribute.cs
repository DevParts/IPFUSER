using System;
using NLog.Config;

namespace NLog.Layouts;

/// <summary>
/// Marks class as Layout and attaches a type-alias name for use in NLog configuration.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class LayoutAttribute : NameBaseAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.LayoutAttribute" /> class.
	/// </summary>
	/// <param name="name">The Layout type-alias for use in NLog configuration.</param>
	public LayoutAttribute(string name)
		: base(name)
	{
	}
}
