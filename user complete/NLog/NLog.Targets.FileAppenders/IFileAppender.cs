using System;

namespace NLog.Targets.FileAppenders;

/// <summary>
/// Handles the actual file-operations on disk
/// </summary>
internal interface IFileAppender : IDisposable
{
	string FilePath { get; }

	long FileSize { get; }

	DateTime OpenStreamTime { get; }

	DateTime LastWriteTime { get; }

	DateTime FileLastModified { get; }

	DateTime NextArchiveTime { get; }

	/// <summary>
	/// Writes the specified bytes to a file.
	/// </summary>
	/// <param name="buffer">The bytes array.</param>
	/// <param name="offset">The bytes array offset.</param>
	/// <param name="count">The number of bytes.</param>
	void Write(byte[] buffer, int offset, int count);

	void Flush();

	bool VerifyFileExists();
}
