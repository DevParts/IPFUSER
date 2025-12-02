using System;
using System.Reflection;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// Renders the assembly version information for the entry assembly or a named assembly.
/// </summary>
/// <remarks>
/// As this layout renderer uses reflection and version information is unlikely to change during application execution,
/// it is recommended to use it in conjunction with the <see cref="T:NLog.LayoutRenderers.Wrappers.CachedLayoutRendererWrapper" />.
/// </remarks>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/AssemblyVersion-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/AssemblyVersion-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("assembly-version")]
[ThreadAgnostic]
public class AssemblyVersionLayoutRenderer : LayoutRenderer
{
	private string? _default;

	private string _format = "major.minor.build.revision";

	private const string DefaultFormat = "major.minor.build.revision";

	private string? _assemblyVersion;

	/// <summary>
	/// The (full) name of the assembly. If <c>null</c>, using the entry assembly.
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the type of assembly version to retrieve.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.LayoutRenderers.AssemblyVersionType.Assembly" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public AssemblyVersionType Type { get; set; }

	/// <summary>
	///  The default value to render if the Version is not available
	/// </summary>
	///  <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	///  <docgen category="Layout Options" order="10" />
	public string Default
	{
		get
		{
			return _default ?? GenerateDefaultValue();
		}
		set
		{
			_default = value;
		}
	}

	/// <summary>
	/// Gets or sets the custom format of the assembly version output.
	/// </summary>
	/// <remarks>
	/// Default: <c>major.minor.build.revision</c> .
	/// Supported placeholders are 'major', 'minor', 'build' and 'revision'. For more details <see href="https://learn.microsoft.com/dotnet/api/system.version" />
	/// </remarks>
	/// <docgen category="Layout Options" order="10" />
	public string Format
	{
		get
		{
			return _format;
		}
		set
		{
			_format = value?.ToLowerInvariant() ?? string.Empty;
		}
	}

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		_assemblyVersion = null;
		base.InitializeLayoutRenderer();
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		string text = _assemblyVersion ?? (_assemblyVersion = ApplyFormatToVersion(GetVersion()));
		if (text == null)
		{
			text = GenerateDefaultValue();
		}
		builder.Append(text);
	}

	private string ApplyFormatToVersion(string version)
	{
		if (version == null)
		{
			return Default;
		}
		if (StringHelpers.IsNullOrWhiteSpace(version))
		{
			return Default;
		}
		if (version == "0.0.0.0" && _default != null)
		{
			return Default;
		}
		if (Format.Equals("major.minor.build.revision", StringComparison.OrdinalIgnoreCase))
		{
			return version;
		}
		string[] array = version.SplitAndTrimTokens('.');
		version = Format.Replace("major", array[0]).Replace("minor", (array.Length > 1) ? array[1] : "0").Replace("build", (array.Length > 2) ? array[2] : "0")
			.Replace("revision", (array.Length > 3) ? array[3] : "0");
		return version;
	}

	private string GenerateDefaultValue()
	{
		return string.Format("Could not find value for {0} assembly and version type {1}", string.IsNullOrEmpty(Name) ? "entry" : Name, Type);
	}

	private string GetVersion()
	{
		try
		{
			Assembly assembly = GetAssembly();
			return Type switch
			{
				AssemblyVersionType.File => assembly?.GetFirstCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? string.Empty, 
				AssemblyVersionType.Informational => assembly?.GetFirstCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty, 
				_ => assembly?.GetName()?.Version?.ToString() ?? string.Empty, 
			};
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "${assembly-version} - Failed to load assembly {0}", Name);
			if (ex.MustBeRethrown())
			{
				throw;
			}
			return string.Empty;
		}
	}

	/// <summary>
	/// Gets the assembly specified by <see cref="P:NLog.LayoutRenderers.AssemblyVersionLayoutRenderer.Name" />, or entry assembly otherwise
	/// </summary>
	protected virtual Assembly GetAssembly()
	{
		if (string.IsNullOrEmpty(Name))
		{
			return Assembly.GetEntryAssembly();
		}
		return Assembly.Load(new AssemblyName(Name));
	}
}
