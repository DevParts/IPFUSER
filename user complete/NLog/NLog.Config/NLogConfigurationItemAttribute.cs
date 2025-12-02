using System;
using JetBrains.Annotations;

namespace NLog.Config;

/// <summary>
/// Marks the object as configuration item for NLog.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[MeansImplicitUse]
public sealed class NLogConfigurationItemAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.NLogConfigurationItemAttribute" /> class.
	/// </summary>
	public NLogConfigurationItemAttribute()
	{
	}
}
