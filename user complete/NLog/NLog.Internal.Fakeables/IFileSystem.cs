using System.IO;

namespace NLog.Internal.Fakeables;

/// <summary>
/// Abstract calls to FileSystem
/// </summary>
internal interface IFileSystem
{
	/// <summary>Determines whether the specified file exists.</summary>
	/// <param name="path">The file to check.</param>
	bool FileExists(string path);

	/// <summary>Returns the content of the specified text-file</summary>
	/// <param name="path">The file to load.</param>
	TextReader LoadTextFile(string path);
}
