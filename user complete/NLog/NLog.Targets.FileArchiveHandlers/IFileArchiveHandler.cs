using System;

namespace NLog.Targets.FileArchiveHandlers;

/// <summary>
/// Handles the actual file-operations on disk
/// </summary>
internal interface IFileArchiveHandler
{
	/// <summary>
	/// Called just before opening a new log-file
	/// </summary>
	/// <param name="newFileName">File-name of the new log-file</param>
	/// <param name="firstLogEvent">The first LogEvent for the new log-file</param>
	/// <param name="previousFileLastModified">Previous file-write-time</param>
	/// <param name="newSequenceNumber">File-path-suffix for the new log-file</param>
	/// <returns>Updated <paramref name="newSequenceNumber" /> for the new file.</returns>
	int ArchiveBeforeOpenFile(string newFileName, LogEventInfo firstLogEvent, DateTime? previousFileLastModified, int newSequenceNumber);
}
