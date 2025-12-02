using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NLog.Common;
using NLog.Internal;
using NLog.Time;

namespace NLog.Targets.FileArchiveHandlers;

internal class BaseFileArchiveHandler
{
	private struct FileInfoDateTime : IComparer<FileInfoDateTime>
	{
		public FileInfo FileInfo { get; }

		public DateTime FileCreatedTimeUtc { get; }

		public int? ArchiveSequenceNumber { get; }

		public FileInfoDateTime(FileInfo fileInfo, DateTime fileCreatedTimeUtc, int? archiveSequenceNumber = null)
		{
			FileInfo = fileInfo;
			ArchiveSequenceNumber = archiveSequenceNumber;
			FileCreatedTimeUtc = fileCreatedTimeUtc;
		}

		public int Compare(FileInfoDateTime x, FileInfoDateTime y)
		{
			if (x.ArchiveSequenceNumber.HasValue && y.ArchiveSequenceNumber.HasValue)
			{
				return x.ArchiveSequenceNumber.Value.CompareTo(y.ArchiveSequenceNumber.Value);
			}
			if (x.FileCreatedTimeUtc.Date == y.FileCreatedTimeUtc.Date)
			{
				return StringComparer.OrdinalIgnoreCase.Compare(x.FileInfo.Name, y.FileInfo.Name);
			}
			return x.FileCreatedTimeUtc.CompareTo(y.FileCreatedTimeUtc);
		}

		public override string ToString()
		{
			return FileInfo.Name;
		}

		public static int? ScanFileNamesForMaxSequenceNo(FileInfo[] fileInfos, int fileWildcardStartIndex, int fileWildcardEndIndex)
		{
			int? result = null;
			if (fileWildcardStartIndex < 0)
			{
				return null;
			}
			for (int i = 0; i < fileInfos.Length; i++)
			{
				string name = fileInfos[i].Name;
				if (!ExcludeFileName(name, fileWildcardStartIndex, fileWildcardEndIndex, null) && TryParseStartSequenceNumber(name, fileWildcardStartIndex, fileWildcardEndIndex, out var archiveSequenceNo) && (!result.HasValue || archiveSequenceNo > result.Value))
				{
					result = archiveSequenceNo;
				}
			}
			return result;
		}

		/// <summary>
		/// - Only strict scan for sequence-number (GetTodaysArchiveFiles) when having input "fileLastWriteTime"
		///     - Expect optional DateTime-part to be "sortable" (when missing birthtime)
		///         - Trim away sequencer-number, so not part of sorting
		///     - Use DateTime part from FileSystem for ordering by Date-only, and sort by FileName
		/// </summary>
		public static IEnumerable<FileInfo> CleanupFiles(FileInfo[] fileInfos, int maxArchiveFiles, int maxArchiveDays, int fileWildcardStartIndex, int fileWildcardEndIndex, bool parseArchiveSequenceNo, string? excludeFileName = null)
		{
			if (fileInfos.Length <= 1)
			{
				if (maxArchiveFiles == 0 && fileInfos.Length == 1 && !ExcludeFileName(fileInfos[0].Name, fileWildcardStartIndex, fileWildcardEndIndex, excludeFileName))
				{
					yield return fileInfos[0];
				}
			}
			else
			{
				if (maxArchiveFiles >= fileInfos.Length && maxArchiveDays <= 0)
				{
					yield break;
				}
				List<FileInfoDateTime> fileInfoDates = new List<FileInfoDateTime>(fileInfos.Length);
				foreach (FileInfo fileInfo in fileInfos)
				{
					string name = fileInfo.Name;
					if (!ExcludeFileName(name, fileWildcardStartIndex, fileWildcardEndIndex, excludeFileName))
					{
						DateTime fileCreatedTimeUtc = fileInfo.LookupValidFileCreationTimeUtc();
						if (parseArchiveSequenceNo && TryParseStartSequenceNumber(name, fileWildcardStartIndex, fileWildcardEndIndex, out var archiveSequenceNo))
						{
							fileInfoDates.Add(new FileInfoDateTime(fileInfo, fileCreatedTimeUtc, archiveSequenceNo));
						}
						else
						{
							fileInfoDates.Add(new FileInfoDateTime(fileInfo, fileCreatedTimeUtc));
						}
					}
				}
				fileInfoDates.Sort((FileInfoDateTime x, FileInfoDateTime y) => x.Compare(x, y));
				for (int i2 = 0; i2 < fileInfoDates.Count && ShouldDeleteFile(fileInfoDates[i2], fileInfoDates.Count - i2, maxArchiveFiles, maxArchiveDays); i2++)
				{
					yield return fileInfoDates[i2].FileInfo;
				}
			}
		}

		private static bool ExcludeFileName(string archiveFileName, int fileWildcardStartIndex, int fileWildcardEndIndex, string? excludeFileName)
		{
			if (fileWildcardStartIndex >= 0 && fileWildcardEndIndex > 0)
			{
				for (int i = fileWildcardStartIndex; i <= archiveFileName.Length - fileWildcardEndIndex; i++)
				{
					if (char.IsLetter(archiveFileName[i]))
					{
						return true;
					}
				}
			}
			if (excludeFileName == null)
			{
				return false;
			}
			return string.Equals(archiveFileName, excludeFileName, StringComparison.OrdinalIgnoreCase);
		}

		private static bool ShouldDeleteFile(FileInfoDateTime existingArchiveFile, int remainingFileCount, int maxArchiveFiles, int maxArchiveDays)
		{
			if (maxArchiveFiles >= 0 && remainingFileCount > maxArchiveFiles)
			{
				return true;
			}
			if (maxArchiveDays > 0)
			{
				DateTime date = TimeSource.Current.Time.Date;
				double totalDays = (date - TimeSource.Current.FromSystemTime(existingArchiveFile.FileCreatedTimeUtc).Date).TotalDays;
				if (totalDays > (double)maxArchiveDays)
				{
					InternalLogger.Debug("FileTarget: Detected old file in archive. FileName={0}, FileDateUtc={1:u}, CurrentDateUtc={2:u}, Age={3} days", existingArchiveFile.FileInfo.FullName, existingArchiveFile.FileCreatedTimeUtc, date, Math.Round(totalDays, 1));
					return true;
				}
			}
			return false;
		}

		private static bool TryParseStartSequenceNumber(string archiveFileName, int fileWildcardStartIndex, int fileWildcardEndIndex, out int archiveSequenceNo)
		{
			int? num = null;
			bool flag = false;
			if (fileWildcardStartIndex < 0 || fileWildcardEndIndex <= 0)
			{
				archiveSequenceNo = 0;
				return false;
			}
			for (int i = fileWildcardStartIndex; i <= archiveFileName.Length - fileWildcardEndIndex; i++)
			{
				char c = archiveFileName[i];
				if (!char.IsDigit(c))
				{
					flag = num.HasValue;
					continue;
				}
				if (flag)
				{
					archiveSequenceNo = 0;
					return false;
				}
				num = ((!(num > 0)) ? new int?(0) : (num * 10)) + (c - 48);
			}
			archiveSequenceNo = num.GetValueOrDefault();
			return num.HasValue;
		}
	}

	protected readonly FileTarget _fileTarget;

	public BaseFileArchiveHandler(FileTarget fileTarget)
	{
		_fileTarget = fileTarget;
	}

	protected bool DeleteOldFilesBeforeArchive(string filePath, bool initialFileOpen, bool parseArchiveSequenceNo, string? excludeFileName = null)
	{
		string directoryName = Path.GetDirectoryName(filePath);
		string deleteOldFileNameWildcard = GetDeleteOldFileNameWildcard(filePath);
		return DeleteOldFilesBeforeArchive(directoryName, deleteOldFileNameWildcard, initialFileOpen, parseArchiveSequenceNo, excludeFileName);
	}

	protected bool DeleteOldFilesBeforeArchive(string fileDirectory, string fileWildcard, bool initialFileOpen, bool parseArchiveSequenceNo, string? excludeFileName = null)
	{
		try
		{
			if (string.IsNullOrEmpty(fileWildcard))
			{
				return false;
			}
			if (string.IsNullOrEmpty(fileDirectory))
			{
				return false;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(fileDirectory);
			if (!directoryInfo.Exists)
			{
				InternalLogger.Debug("{0}: Archive Cleanup found no files matching wildcard {1} in directory: {2}", _fileTarget, fileWildcard, fileDirectory);
				return false;
			}
			FileInfo[] files = directoryInfo.GetFiles(fileWildcard);
			InternalLogger.Debug("{0}: Archive Cleanup found {1} files matching wildcard {2} in directory: {3}", _fileTarget, files.Length, fileWildcard, fileDirectory);
			if (files.Length == 0)
			{
				return false;
			}
			int num = _fileTarget.MaxArchiveFiles;
			if (initialFileOpen && (!_fileTarget.ArchiveOldFileOnStartup || _fileTarget.DeleteOldFileOnStartup))
			{
				num = ((!_fileTarget.DeleteOldFileOnStartup) ? num : 0);
			}
			else if (num > 0)
			{
				num--;
			}
			string archiveCleanupReason = ((_fileTarget.MaxArchiveFiles < 0 && _fileTarget.MaxArchiveDays > 0) ? $"MaxArchiveDays={_fileTarget.MaxArchiveDays}" : $"MaxArchiveFiles={_fileTarget.MaxArchiveFiles}");
			if (initialFileOpen && _fileTarget.DeleteOldFileOnStartup)
			{
				archiveCleanupReason = "DeleteOldFileOnStartup=true";
			}
			int num2 = fileWildcard.IndexOf('*');
			int fileWildcardEndIndex = ((num2 >= 0 && num2 == fileWildcard.LastIndexOf('*')) ? (fileWildcard.Length - num2) : (-1));
			bool result = false;
			foreach (FileInfo item in FileInfoDateTime.CleanupFiles(files, num, _fileTarget.MaxArchiveDays, num2, fileWildcardEndIndex, parseArchiveSequenceNo, excludeFileName))
			{
				result = true;
				DeleteOldArchiveFile(item.FullName, archiveCleanupReason);
			}
			return result;
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "{0}: Failed to cleanup archive folder: {1}  {2}", _fileTarget, fileDirectory, fileWildcard ?? "");
			if (ex.MustBeRethrown(_fileTarget))
			{
				throw;
			}
		}
		return false;
	}

	protected static int? GetMaxArchiveSequenceNo(FileInfo[] fileInfos, int fileWildcardStartIndex, int fileWildcardEndIndex)
	{
		return FileInfoDateTime.ScanFileNamesForMaxSequenceNo(fileInfos, fileWildcardStartIndex, fileWildcardEndIndex);
	}

	private static string GetDeleteOldFileNameWildcard(string filepath)
	{
		string text = Path.GetFileNameWithoutExtension(filepath) ?? string.Empty;
		string text2 = Path.GetExtension(filepath) ?? string.Empty;
		if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
		{
			return string.Empty;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		bool flag = false;
		for (int i = 0; i < text.Length; i++)
		{
			if (!char.IsLetter(text[i]))
			{
				flag = flag || char.IsDigit(text[i]);
				if (flag)
				{
					if (num3 == 0)
					{
						num4 = i;
					}
					num3++;
				}
				continue;
			}
			if (num3 != 0)
			{
				if (num2 <= num3)
				{
					num = num4;
					num2 = num3;
				}
				num3 = 0;
			}
			flag = false;
		}
		if (num2 < num3)
		{
			num = num4;
			num2 = num3;
		}
		if (num2 > 0)
		{
			string text3 = text.Substring(0, num);
			string text4 = text.Substring(num + num2);
			if (!string.IsNullOrEmpty(text4))
			{
				return text3 + "*" + text4 + "*" + text2;
			}
			return text3 + "*" + text2;
		}
		return text + "*" + text2;
	}

	protected bool DeleteOldArchiveFile(string filepath, string archiveCleanupReason)
	{
		for (int i = 1; i <= 3; i++)
		{
			try
			{
				InternalLogger.Info("{0}: Cleanup file archive {1}. Delete file: '{2}'.", _fileTarget, archiveCleanupReason, filepath);
				_fileTarget.CloseOpenFileBeforeArchiveCleanup(filepath);
				File.Delete(filepath);
				return true;
			}
			catch (DirectoryNotFoundException ex)
			{
				InternalLogger.Debug(ex, "{0}: Failed to delete old file as directory not found: '{1}'", _fileTarget, filepath);
				return true;
			}
			catch (FileNotFoundException ex2)
			{
				InternalLogger.Debug(ex2, "{0}: Failed to delete old file as file not found: '{1}'", _fileTarget, filepath);
				return true;
			}
			catch (IOException ex3)
			{
				InternalLogger.Debug(ex3, "{0}: Failed to delete old file, maybe file is locked: '{1}'", _fileTarget, filepath);
				if (!File.Exists(filepath))
				{
					return true;
				}
				if (i >= 3 && ex3.MustBeRethrown(_fileTarget))
				{
					throw;
				}
			}
			catch (Exception ex4)
			{
				InternalLogger.Warn(ex4, "{0}: Failed to delete old archive file: '{1}'.", _fileTarget, filepath);
				if (ex4.MustBeRethrown(_fileTarget))
				{
					throw;
				}
				return false;
			}
			Thread.Sleep(i * 10);
		}
		return false;
	}

	protected void FixWindowsFileSystemTunneling(string newFilePath)
	{
		try
		{
			if (PlatformDetector.IsWin32 && !File.Exists(newFilePath))
			{
				File.Create(newFilePath).Dispose();
				File.SetCreationTimeUtc(newFilePath, DateTime.UtcNow);
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Debug(ex, "{0}: Failed to refresh CreationTimeUtc for FileName: {1}", _fileTarget, newFilePath);
		}
	}
}
