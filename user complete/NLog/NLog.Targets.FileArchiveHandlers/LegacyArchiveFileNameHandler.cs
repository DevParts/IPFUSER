using System;
using System.IO;
using System.Threading;
using NLog.Common;
using NLog.Internal;
using NLog.Layouts;
using NLog.Time;

namespace NLog.Targets.FileArchiveHandlers;

/// <summary>
/// Legacy archive logic with file-move of active-file to file-path specified by <see cref="P:NLog.Targets.FileTarget.ArchiveFileName" />
/// </summary>
/// <remarks>
/// Kept mostly for legacy reasons, because archive operation can fail because of file-locks by other applications (or by multi-process logging).
///
/// Avoid using <see cref="P:NLog.Targets.FileTarget.ArchiveFileName" /> when possible, and instead rely on only using <see cref="P:NLog.Targets.FileTarget.FileName" /> and <see cref="P:NLog.Targets.FileTarget.ArchiveSuffixFormat" />.
/// </remarks>
internal sealed class LegacyArchiveFileNameHandler : RollingArchiveFileHandler, IFileArchiveHandler
{
	public LegacyArchiveFileNameHandler(FileTarget fileTarget)
		: base(fileTarget)
	{
	}

	public override int ArchiveBeforeOpenFile(string newFileName, LogEventInfo firstLogEvent, DateTime? previousFileLastModified, int newSequenceNumber)
	{
		string text = _fileTarget.ArchiveFileName?.Render(firstLogEvent);
		if (text == null || StringHelpers.IsNullOrWhiteSpace(text))
		{
			return base.ArchiveBeforeOpenFile(newFileName, firstLogEvent, previousFileLastModified, newSequenceNumber);
		}
		string newFilePath = FileTarget.CleanFullFilePath(newFileName);
		bool initialFileOpen = newSequenceNumber == 0;
		if (ArchiveBeforeOpenFile(text, newFilePath, firstLogEvent, previousFileLastModified, initialFileOpen))
		{
			FixWindowsFileSystemTunneling(newFilePath);
		}
		return 0;
	}

	private bool ArchiveBeforeOpenFile(string archiveFileName, string newFilePath, LogEventInfo firstLogEvent, DateTime? previousFileLastModified, bool initialFileOpen)
	{
		bool result = false;
		if (_fileTarget.MaxArchiveFiles >= 0 || _fileTarget.MaxArchiveDays > 0 || (initialFileOpen && _fileTarget.DeleteOldFileOnStartup))
		{
			bool num = _fileTarget.ArchiveSuffixFormat.IndexOf("{0", StringComparison.Ordinal) >= 0 && _fileTarget.ArchiveSuffixFormat.IndexOf("{1", StringComparison.Ordinal) < 0 && (!(_fileTarget.ArchiveFileName is SimpleLayout simpleLayout) || (simpleLayout.OriginalText.IndexOf("${date", StringComparison.OrdinalIgnoreCase) < 0 && simpleLayout.OriginalText.IndexOf("${shortdate", StringComparison.OrdinalIgnoreCase) < 0));
			string fileName = Path.GetFileName(newFilePath);
			if (num)
			{
				string text = BuildArchiveFilePath(archiveFileName, int.MaxValue, DateTime.MinValue);
				string path = text.Replace(int.MaxValue.ToString(), "*");
				string directoryName = Path.GetDirectoryName(text);
				result = DeleteOldFilesBeforeArchive(directoryName, Path.GetFileName(path), initialFileOpen, parseArchiveSequenceNo: true, fileName);
			}
			else
			{
				string filePath = FileTarget.CleanFullFilePath(archiveFileName);
				result = DeleteOldFilesBeforeArchive(filePath, initialFileOpen, parseArchiveSequenceNo: false, fileName);
			}
		}
		if (initialFileOpen && !_fileTarget.ArchiveOldFileOnStartup)
		{
			return result;
		}
		if (!ArchiveOldFileWithRetry(archiveFileName, newFilePath, firstLogEvent, previousFileLastModified))
		{
			return result;
		}
		return true;
	}

	private bool ArchiveOldFileWithRetry(string archiveFileName, string newFilePath, LogEventInfo firstLogEvent, DateTime? previousFileLastModified)
	{
		DateTime? dateTime = null;
		long? num = null;
		bool result = false;
		for (int i = 1; i <= 3; i++)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(newFilePath);
				if (!fileInfo.Exists)
				{
					return result;
				}
				result = true;
				if (HasFileInfoChanged(fileInfo, dateTime, num))
				{
					return false;
				}
				dateTime = dateTime ?? fileInfo.LastWriteTimeUtc;
				num = num ?? fileInfo.Length;
				if (ArchiveOldFile(archiveFileName, fileInfo, firstLogEvent, previousFileLastModified))
				{
					return true;
				}
			}
			catch (IOException ex)
			{
				InternalLogger.Debug(ex, "{0}: Failed to archive file, maybe file is locked: '{1}'", _fileTarget, newFilePath);
				if (!File.Exists(newFilePath))
				{
					return result;
				}
				if (i >= 3 && LogManager.ThrowExceptions)
				{
					throw;
				}
			}
			catch (Exception ex2)
			{
				InternalLogger.Debug(ex2, "{0}: Failed to archive file: '{1}'", _fileTarget, newFilePath);
				if (LogManager.ThrowExceptions)
				{
					throw;
				}
				Thread.Sleep(i * 10);
			}
		}
		return result;
	}

	private static bool HasFileInfoChanged(FileInfo newFileInfo, DateTime? lastWriteTimeUtc, long? lastFileLength)
	{
		if (lastWriteTimeUtc.HasValue && lastWriteTimeUtc.Value != newFileInfo.LastWriteTimeUtc)
		{
			return true;
		}
		if (lastFileLength.HasValue && lastFileLength.Value != newFileInfo.Length)
		{
			return true;
		}
		return false;
	}

	private bool ArchiveOldFile(string archiveFileName, FileInfo newFileInfo, LogEventInfo firstLogEvent, DateTime? previousFileLastModified)
	{
		DateTime dateTime = TimeSource.Current.FromSystemTime(newFileInfo.LastWriteTimeUtc);
		if (previousFileLastModified.HasValue && (previousFileLastModified > dateTime || dateTime >= firstLogEvent.TimeStamp))
		{
			dateTime = previousFileLastModified.Value;
		}
		int num = ResolveNextArchiveSequenceNo(archiveFileName, newFileInfo, dateTime);
		string text = BuildArchiveFilePath(archiveFileName, num, dateTime);
		if (!File.Exists(text))
		{
			InternalLogger.Info("{0}: Move file from '{1}' to '{2}'", _fileTarget, newFileInfo.FullName, text);
			File.Move(newFileInfo.FullName, text);
			return true;
		}
		if (!newFileInfo.Exists)
		{
			return true;
		}
		if (num == 0)
		{
			ArchiveFileAppendExisting(newFileInfo.FullName, text);
			return true;
		}
		return false;
	}

	private string BuildArchiveFilePath(string archiveFileName, int archiveNextSequenceNo, DateTime fileLastWriteTime)
	{
		return _fileTarget.BuildFullFilePath(archiveFileName, archiveNextSequenceNo, fileLastWriteTime);
	}

	private void ArchiveFileAppendExisting(string newFilePath, string archiveFilePath)
	{
		InternalLogger.Info("{0}: Already exists, append to {1}", _fileTarget, archiveFilePath);
		FileShare fileShare = FileShare.Read | FileShare.Delete;
		using (FileStream fileStream = File.Open(newFilePath, FileMode.Open, FileAccess.ReadWrite, fileShare))
		{
			using FileStream fileStream2 = File.Open(archiveFilePath, FileMode.Append);
			if (_fileTarget.WriteBom)
			{
				byte[] preamble = _fileTarget.Encoding.GetPreamble();
				if (preamble.Length != 0)
				{
					fileStream.Seek(preamble.Length, SeekOrigin.Begin);
				}
			}
			byte[] array = new byte[4096];
			int count;
			while ((count = fileStream.Read(array, 0, array.Length)) > 0)
			{
				fileStream2.Write(array, 0, count);
			}
			fileStream.SetLength(0L);
			if (!DeleteOldArchiveFile(newFilePath, "Truncate Active File"))
			{
				fileShare &= ~FileShare.Delete;
			}
			fileStream.Close();
		}
		if ((fileShare & FileShare.Delete) == 0)
		{
			DeleteOldArchiveFile(newFilePath, "Truncate Active File");
		}
	}

	private int ResolveNextArchiveSequenceNo(string archiveFileName, FileInfo newFileInfo, DateTime fileLastWriteTime)
	{
		string path = BuildArchiveFilePath(archiveFileName, int.MaxValue, fileLastWriteTime);
		string directoryName = Path.GetDirectoryName(path);
		if (string.IsNullOrEmpty(directoryName))
		{
			return 0;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
		if (!directoryInfo.Exists && _fileTarget.CreateDirs)
		{
			InternalLogger.Debug("{0}: Creating archive directory: {1}", _fileTarget, directoryName);
			directoryInfo.Create();
		}
		string text = Path.GetFileName(path).Replace(int.MaxValue.ToString(), "*");
		int num = text.IndexOf('*');
		if (num < 0)
		{
			return 0;
		}
		int fileWildcardEndIndex = text.Length - num;
		if (num > 0 && !char.IsLetterOrDigit(text[num - 1]))
		{
			text = text.Substring(0, num - 1) + text.Substring(num);
		}
		FileInfo[] files = directoryInfo.GetFiles(text);
		InternalLogger.Debug("{0}: Archive Sequence Rolling found {1} files matching wildcard {2} in directory: {3}", _fileTarget, files.Length, text, directoryName);
		if (files.Length == 0)
		{
			return 0;
		}
		int? maxArchiveSequenceNo = BaseFileArchiveHandler.GetMaxArchiveSequenceNo(files, num, fileWildcardEndIndex);
		if (maxArchiveSequenceNo.HasValue)
		{
			return maxArchiveSequenceNo.Value + 1;
		}
		if (string.Equals(directoryInfo.FullName, newFileInfo.DirectoryName, StringComparison.OrdinalIgnoreCase) && string.Equals(Path.GetFileName(archiveFileName), newFileInfo.Name, StringComparison.OrdinalIgnoreCase))
		{
			return 1;
		}
		return 0;
	}
}
