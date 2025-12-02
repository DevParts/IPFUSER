using System;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// System special folder path (includes My Documents, My Music, Program Files, Desktop, and more).
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Special-Folder-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Special-Folder-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("specialfolder")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public class SpecialFolderLayoutRenderer : LayoutRenderer
{
	/// <summary>
	/// Gets or sets the system special folder to use.
	/// </summary>
	/// <remarks>
	/// Full list of options is available at <a href="https://docs.microsoft.com/en-us/dotnet/api/system.environment.specialfolder">MSDN</a>.
	/// The most common ones are:
	/// <ul>
	/// <li><b>CommonApplicationData</b> - application data for all users.</li>
	/// <li><b>ApplicationData</b> - roaming application data for current user.</li>
	/// <li><b>LocalApplicationData</b> - non roaming application data for current user</li>
	/// <li><b>UserProfile</b> - Profile folder for current user</li>
	/// <li><b>DesktopDirectory</b> - Desktop-directory for current user</li>
	/// <li><b>MyDocuments</b> - My Documents-directory for current user</li>
	/// <li><b>System</b> - System directory</li>
	/// </ul>
	/// </remarks>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public Environment.SpecialFolder Folder { get; set; } = Environment.SpecialFolder.CommonApplicationData;

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

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		string value = PathHelpers.CombinePaths(GetFolderPath(Folder), Dir, File);
		builder.Append(value);
	}

	internal static string GetFolderPath(Environment.SpecialFolder folder)
	{
		try
		{
			string folderPath = Environment.GetFolderPath(folder);
			if (!string.IsNullOrEmpty(folderPath))
			{
				return folderPath;
			}
		}
		catch
		{
			string folderPathFromEnvironment = GetFolderPathFromEnvironment(folder);
			if (!string.IsNullOrEmpty(folderPathFromEnvironment))
			{
				return folderPathFromEnvironment;
			}
			throw;
		}
		return GetFolderPathFromEnvironment(folder);
	}

	private static string GetFolderPathFromEnvironment(Environment.SpecialFolder folder)
	{
		try
		{
			string folderWindowsEnvironmentVariable = GetFolderWindowsEnvironmentVariable(folder);
			if (string.IsNullOrEmpty(folderWindowsEnvironmentVariable))
			{
				return string.Empty;
			}
			if (!PlatformDetector.IsWin32)
			{
				return string.Empty;
			}
			return Environment.GetEnvironmentVariable(folderWindowsEnvironmentVariable) ?? string.Empty;
		}
		catch
		{
			return string.Empty;
		}
	}

	private static string GetFolderWindowsEnvironmentVariable(Environment.SpecialFolder folder)
	{
		return folder switch
		{
			Environment.SpecialFolder.CommonApplicationData => "COMMONAPPDATA", 
			Environment.SpecialFolder.LocalApplicationData => "LOCALAPPDATA", 
			Environment.SpecialFolder.ApplicationData => "APPDATA", 
			Environment.SpecialFolder.UserProfile => "USERPROFILE", 
			_ => string.Empty, 
		};
	}
}
