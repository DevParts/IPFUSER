using System;
using NLog.Config;

namespace NLog.LayoutRenderers;

/// <summary>
/// Designates a property of the class as an ambient property.
/// </summary>
/// <example>
/// non-ambient:  ${uppercase:${level}}
/// ambient    :  ${level:uppercase}
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class AmbientPropertyAttribute : NameBaseAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.AmbientPropertyAttribute" /> class.
	/// </summary>
	/// <param name="name">Ambient property name.</param>
	public AmbientPropertyAttribute(string name)
		: base(name)
	{
	}
}
