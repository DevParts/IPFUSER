using System;
using System.IO;
using System.Linq;
using NLog.Common;
using NLog.Internal;

namespace NLog.Targets.FileArchiveHandlers;

/// <summary>
/// Rolls the active-file to the next sequence-number
/// </summary>
internal class RollingArchiveFileHandler : BaseFileArchiveHandler, IFileArchiveHandler
{
	public RollingArchiveFileHandler(FileTarget fileTarget)
		: base(fileTarget)
	{
	}

	public virtual int ArchiveBeforeOpenFile(string newFileName, LogEventInfo firstLogEvent, DateTime? previousFileLastModified, int newSequenceNumber)
	{
		bool flag = newSequenceNumber == 0;
		if (_fileTarget.MaxArchiveFiles >= 0 || _fileTarget.MaxArchiveDays > 0 || (flag && _fileTarget.DeleteOldFileOnStartup))
		{
			string text = FileTarget.CleanFullFilePath(newFileName);
			bool parseArchiveSequenceNo = !Path.GetFileNameWithoutExtension(text).Any((char c) => char.IsDigit(c));
			bool flag2 = DeleteOldFilesBeforeArchive(text, flag, parseArchiveSequenceNo);
			if (_fileTarget.MaxArchiveFiles == 0 || _fileTarget.MaxArchiveFiles == 1 || (flag && _fileTarget.DeleteOldFileOnStartup))
			{
				if (flag2)
				{
					FixWindowsFileSystemTunneling(text);
				}
				return 0;
			}
		}
		if (flag && (_fileTarget.ArchiveOldFileOnStartup || _fileTarget.ArchiveAboveSize > 0 || _fileTarget.ArchiveEvery != FileArchivePeriod.None))
		{
			string newFilePath = FileTarget.CleanFullFilePath(newFileName);
			return RollToInitialSequenceNumber(newFilePath);
		}
		return newSequenceNumber;
	}

	private int RollToInitialSequenceNumber(string newFilePath)
	{
		int num = 0;
		try
		{
			if (AllowOptimizedRollingForArchiveAboveSize())
			{
				FileInfo fileInfo = new FileInfo(newFilePath);
				long num2 = (fileInfo.Exists ? fileInfo.Length : 0);
				if (num2 > 0 && num2 < _fileTarget.ArchiveAboveSize)
				{
					InternalLogger.Debug("{0}: Archive rolling skipped because file-size={1} < ArchiveAboveSize for file: {2}", _fileTarget, num2, newFilePath);
					return num;
				}
			}
			string directoryName = Path.GetDirectoryName(newFilePath);
			if (string.IsNullOrEmpty(directoryName))
			{
				return 0;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
			if (!directoryInfo.Exists)
			{
				InternalLogger.Debug("{0}: Archive Sequence Rolling found no files in directory: {1}", _fileTarget, directoryName);
				return 0;
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFilePath);
			string extension = Path.GetExtension(newFilePath);
			string text = fileNameWithoutExtension + "*" + extension;
			FileInfo[] files = directoryInfo.GetFiles(text);
			InternalLogger.Debug("{0}: Archive Sequence Rolling found {1} files matching wildcard {2} in directory: {3}", _fileTarget, files.Length, text, directoryName);
			if (files.Length == 0)
			{
				return 0;
			}
			if (_fileTarget.DeleteOldFileOnStartup)
			{
				FileInfo[] array = files;
				foreach (FileInfo fileInfo2 in array)
				{
					DeleteOldArchiveFile(fileInfo2.FullName, "DeleteOldFileOnStartup=true");
				}
				return 0;
			}
			string fileName = Path.GetFileName(_fileTarget.BuildFullFilePath(newFilePath, int.MaxValue, DateTime.MinValue).Replace(int.MaxValue.ToString(), "*"));
			int num3 = fileName.IndexOf('*');
			int fileWildcardEndIndex = ((num3 >= 0) ? (fileName.Length - num3) : (-1));
			num = BaseFileArchiveHandler.GetMaxArchiveSequenceNo(files, num3, fileWildcardEndIndex).GetValueOrDefault();
			if (_fileTarget.ArchiveOldFileOnStartup)
			{
				num++;
			}
			return num;
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "{0}: Failed to resolve initial archive sequence number for file: {1}", _fileTarget, newFilePath);
			if (ex.MustBeRethrown(_fileTarget))
			{
				throw;
			}
			return num;
		}
	}

	private bool AllowOptimizedRollingForArchiveAboveSize()
	{
		if (_fileTarget.ArchiveAboveSize > 0 && _fileTarget.ArchiveEvery == FileArchivePeriod.None && !_fileTarget.ArchiveOldFileOnStartup && !_fileTarget.DeleteOldFileOnStartup)
		{
			return _fileTarget.GetType().Equals(typeof(FileTarget));
		}
		return false;
	}
}
