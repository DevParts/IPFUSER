using System;

namespace NLog.Config;

/// <summary>
/// Indicates NLog should not scan this property during configuration.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NLogConfigurationIgnorePropertyAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.NLogConfigurationIgnorePropertyAttribute" /> class.
	/// </summary>
	public NLogConfigurationIgnorePropertyAttribute()
	{
	}
}
