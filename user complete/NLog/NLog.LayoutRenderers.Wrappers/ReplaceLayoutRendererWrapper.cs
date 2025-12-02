using System;
using NLog.Config;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Replaces a string in the output of another layout with another string.
/// </summary>
/// <example>
/// ${replace:searchFor=foo:replaceWith=bar:inner=${message}}
/// </example>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Replace-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Replace-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("replace")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class ReplaceLayoutRendererWrapper : WrapperLayoutRendererBase
{
	private string _searchFor = string.Empty;

	private string _searchForOriginal = string.Empty;

	private string _replaceWith = string.Empty;

	private string _replaceWithOriginal = string.Empty;

	/// <summary>
	/// Gets or sets the text to search for.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string SearchFor
	{
		get
		{
			return _searchForOriginal ?? _searchFor;
		}
		set
		{
			_searchForOriginal = value;
			_searchFor = SimpleLayout.Evaluate(value, base.LoggingConfiguration, null, false);
		}
	}

	/// <summary>
	/// Gets or sets the replacement string.
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string ReplaceWith
	{
		get
		{
			return _replaceWithOriginal ?? _replaceWith;
		}
		set
		{
			_replaceWithOriginal = value;
			_replaceWith = SimpleLayout.Evaluate(value, base.LoggingConfiguration, null, false);
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether to ignore case.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Condition Options" order="10" />
	public bool IgnoreCase { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to search for whole words
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Condition Options" order="10" />
	public bool WholeWords { get; set; }

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		if (string.IsNullOrEmpty(SearchFor))
		{
			throw new NLogConfigurationException("Replace-LayoutRenderer SearchFor-property must be assigned. Searching for blank value not supported.");
		}
		if (_searchForOriginal != null)
		{
			_searchFor = SimpleLayout.Evaluate(_searchForOriginal, base.LoggingConfiguration);
		}
		if (_replaceWithOriginal != null)
		{
			_replaceWith = SimpleLayout.Evaluate(_replaceWithOriginal, base.LoggingConfiguration);
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		if (IgnoreCase || WholeWords)
		{
			StringComparison comparison = (IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
			return StringHelpers.Replace(text, _searchFor, _replaceWith, comparison, WholeWords);
		}
		return text.Replace(_searchFor, _replaceWith);
	}
}
