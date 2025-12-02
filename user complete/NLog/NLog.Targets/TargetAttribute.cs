using System;
using NLog.Config;

namespace NLog.Targets;

/// <summary>
/// Marks class as logging target and attaches a type-alias name for use in NLog configuration.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class TargetAttribute : NameBaseAttribute
{
	/// <summary>
	/// Gets or sets a value indicating whether to the target is a wrapper target (used to generate the target summary documentation page).
	/// </summary>
	public bool IsWrapper { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to the target is a compound target (used to generate the target summary documentation page).
	/// </summary>
	public bool IsCompound { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.TargetAttribute" /> class.
	/// </summary>
	/// <param name="name">The target type-alias for use in NLog configuration.</param>
	public TargetAttribute(string name)
		: base(name)
	{
	}
}
