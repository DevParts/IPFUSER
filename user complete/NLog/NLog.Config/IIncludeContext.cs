namespace NLog.Config;

/// <summary>
/// Include context properties
/// </summary>
internal interface IIncludeContext
{
	/// <summary>
	/// Gets or sets the option to include all properties from the log events
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	bool IncludeEventProperties { get; set; }

	/// <summary>
	/// Gets or sets whether to include the contents of the <see cref="T:NLog.ScopeContext" /> properties-dictionary.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	bool IncludeScopeProperties { get; set; }

	/// <summary>
	/// Gets or sets whether to include the contents of the <see cref="T:NLog.ScopeContext" /> nested-state-stack.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	bool IncludeScopeNested { get; set; }
}
