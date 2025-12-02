using System;
using System.IO;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.Internal.Fakeables;

namespace NLog.LayoutRenderers;

/// <summary>
/// The current application domain's base directory.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Basedir-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Basedir-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("basedir")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public class BaseDirLayoutRenderer : LayoutRenderer
{
	private readonly string _appDomainDirectory;

	private string _baseDir;

	private readonly IAppEnvironment _appEnvironment;

	/// <summary>
	/// Use base dir of current process. Alternative one can just use ${processdir}
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool ProcessDir { get; set; }

	/// <summary>
	/// Fallback to the base dir of current process, when AppDomain.BaseDirectory is Temp-Path (.NET Core 3 - Single File Publish)
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public bool FixTempDir { get; set; }

	/// <summary>
	/// Gets or sets the name of the file to be Path.Combine()'d with the directory name.
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Advanced Options" order="50" />
	public string File { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the name of the directory to be Path.Combine()'d with the directory name.
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Advanced Options" order="50" />
	public string Dir { get; set; } = string.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.BaseDirLayoutRenderer" /> class.
	/// </summary>
	public BaseDirLayoutRenderer()
		: this(LogFactory.DefaultAppEnvironment)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.BaseDirLayoutRenderer" /> class.
	/// </summary>
	internal BaseDirLayoutRenderer(IAppEnvironment appEnvironment)
	{
		_baseDir = (_appDomainDirectory = appEnvironment.AppDomainBaseDirectory);
		_appEnvironment = appEnvironment;
	}

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		_baseDir = _appDomainDirectory;
		if (ProcessDir)
		{
			string processDir = GetProcessDir();
			if (!string.IsNullOrEmpty(processDir))
			{
				_baseDir = processDir;
			}
		}
		else if (FixTempDir)
		{
			string fixedTempBaseDir = GetFixedTempBaseDir(_appDomainDirectory);
			if (!string.IsNullOrEmpty(fixedTempBaseDir))
			{
				_baseDir = fixedTempBaseDir;
			}
		}
		_baseDir = AppEnvironmentWrapper.FixFilePathWithLongUNC(_baseDir);
		_baseDir = PathHelpers.CombinePaths(_baseDir, Dir, File);
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.Append(_baseDir);
	}

	private string GetFixedTempBaseDir(string baseDir)
	{
		try
		{
			string userTempFilePath = _appEnvironment.UserTempFilePath;
			if (PathHelpers.IsTempDir(baseDir, userTempFilePath))
			{
				string processDir = GetProcessDir();
				if (!string.IsNullOrEmpty(processDir) && !PathHelpers.IsTempDir(processDir, userTempFilePath))
				{
					return processDir;
				}
			}
			return baseDir;
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "BaseDir LayoutRenderer unexpected exception");
			return baseDir;
		}
	}

	private string GetProcessDir()
	{
		return Path.GetDirectoryName(_appEnvironment.CurrentProcessFilePath);
	}
}
