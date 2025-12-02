using System;
using JetBrains.Annotations;

namespace NLog.Config;

/// <summary>
/// Attribute used to mark the default parameters for layout renderers.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
[MeansImplicitUse]
public sealed class DefaultParameterAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.DefaultParameterAttribute" /> class.
	/// </summary>
	public DefaultParameterAttribute()
	{
	}
}
