using System;

namespace NLog.Config;

/// <summary>
/// Marks the class or a member as advanced. Advanced classes and members are hidden by
/// default in generated documentation.
/// </summary>
[Obsolete("Attribute is no longer used. Obsolete with NLog 5.0")]
[AttributeUsage(AttributeTargets.Property)]
public sealed class AdvancedAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.AdvancedAttribute" /> class.
	/// </summary>
	public AdvancedAttribute()
	{
	}
}
