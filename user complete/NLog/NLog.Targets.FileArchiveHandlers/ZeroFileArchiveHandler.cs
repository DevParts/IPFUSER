using System;
using System.IO;
using System.Linq;
using NLog.Common;
using NLog.Internal;

namespace NLog.Targets.FileArchiveHandlers;

/// <summary>
/// Deletes/truncates the active logging-file when archive-roll-event is triggered
/// </summary>
internal sealed class ZeroFileArchiveHandler : BaseFileArchiveHandler, IFileArchiveHandler
{
	public ZeroFileArchiveHandler(FileTarget fileTarget)
		: base(fileTarget)
	{
	}

	public int ArchiveBeforeOpenFile(string newFileName, LogEventInfo firstLogEvent, DateTime? previousFileLastModified, int newSequenceNumber)
	{
		string newFilePath = FileTarget.CleanFullFilePath(newFileName);
		bool initialFileOpen = newSequenceNumber == 0;
		if (DeleteOldArchiveFiles(newFilePath, initialFileOpen))
		{
			FixWindowsFileSystemTunneling(newFilePath);
		}
		return 0;
	}

	private bool DeleteOldArchiveFiles(string newFilePath, bool initialFileOpen)
	{
		try
		{
			if (initialFileOpen && _fileTarget.DeleteOldFileOnStartup && Path.GetFileNameWithoutExtension(newFilePath).Any((char chr) => char.IsDigit(chr)))
			{
				return DeleteOldFilesBeforeArchive(newFilePath, initialFileOpen, parseArchiveSequenceNo: false);
			}
			if (File.Exists(newFilePath))
			{
				string archiveCleanupReason = ((_fileTarget.MaxArchiveFiles < 0 && _fileTarget.ArchiveOldFileOnStartup) ? "ArchiveOldFileOnStartup=true" : $"MaxArchiveFiles={_fileTarget.MaxArchiveFiles}");
				return DeleteOldArchiveFile(newFilePath, archiveCleanupReason);
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Debug(ex, "{0}: Failed to archive file: '{1}'", _fileTarget, newFilePath);
			if (ex.MustBeRethrown(_fileTarget))
			{
				throw;
			}
		}
		return false;
	}
}
