using System;
using System.IO;
using System.Security;
using NLog.Common;
using NLog.Internal;
using NLog.Time;

namespace NLog.Targets.FileAppenders;

[SecuritySafeCritical]
internal sealed class ExclusiveFileLockingAppender : IFileAppender, IDisposable
{
	private readonly FileTarget _fileTarget;

	private readonly string _filePath;

	private Stream _fileStream;

	private int _lastFileDeletedCheck;

	private long? _countedFileSize;

	private DateTime? _fileBirthTime;

	private DateTime _nextArchiveTime;

	private DateTime _lastFileBirthTime;

	public string FilePath => _filePath;

	public DateTime OpenStreamTime { get; }

	public DateTime LastWriteTime => FileLastModified;

	public DateTime FileLastModified { get; private set; }

	private DateTime FileBirthTime
	{
		get
		{
			return _fileBirthTime ?? OpenStreamTime;
		}
		set
		{
			_fileBirthTime = value;
		}
	}

	public DateTime NextArchiveTime
	{
		get
		{
			DateTime fileBirthTime = FileBirthTime;
			if (_lastFileBirthTime != fileBirthTime)
			{
				_nextArchiveTime = FileTarget.CalculateNextArchiveEventTime(_fileTarget.ArchiveEvery, fileBirthTime);
				_lastFileBirthTime = fileBirthTime;
			}
			return _nextArchiveTime;
		}
	}

	public long FileSize => _countedFileSize ?? _fileStream.Length;

	public ExclusiveFileLockingAppender(FileTarget fileTarget, string filePath)
	{
		_fileTarget = fileTarget;
		_filePath = filePath;
		OpenStreamTime = TimeSource.Current.Time;
		_lastFileDeletedCheck = Environment.TickCount;
		long fileInfoSize = RefreshFileBirthTimeUtc(forceRefresh: true);
		_fileStream = _fileTarget.CreateFileStreamWithRetry(this, fileTarget.BufferSize, initialFileOpen: true);
		_countedFileSize = RefreshCountedFileSize(_fileStream, fileInfoSize);
	}

	private long RefreshFileBirthTimeUtc(bool forceRefresh)
	{
		FileLastModified = TimeSource.Current.Time;
		if (_fileTarget.ArchiveEvery == FileArchivePeriod.None && _fileTarget.ArchiveAboveSize <= 0 && _fileTarget.ArchiveFileName == null)
		{
			return 0L;
		}
		try
		{
			FileInfo fileInfo = new FileInfo(_filePath);
			long num = (fileInfo.Exists ? fileInfo.Length : 0);
			if (num > 0)
			{
				DateTime systemTime = fileInfo.LookupValidFileCreationTimeUtc();
				DateTime fileBirthTime = ((systemTime.Year > 1980) ? TimeSource.Current.FromSystemTime(systemTime) : OpenStreamTime);
				if (!forceRefresh && fileBirthTime.Date < FileBirthTime.Date)
				{
					fileBirthTime = FileBirthTime;
				}
				FileBirthTime = fileBirthTime;
				FileLastModified = TimeSource.Current.FromSystemTime(fileInfo.LastWriteTimeUtc);
			}
			return num;
		}
		catch (Exception ex)
		{
			InternalLogger.Debug(ex, "{0}: Failed to refresh BirthTime for file: '{1}'", _fileTarget, _filePath);
			return 0L;
		}
	}

	public void Write(byte[] buffer, int offset, int count)
	{
		int num = Environment.TickCount - _lastFileDeletedCheck;
		if (num > 1000 || num < -1000)
		{
			MonitorFileHasBeenDeleted();
			_lastFileDeletedCheck = Environment.TickCount;
			FileLastModified = TimeSource.Current.Time;
		}
		_fileStream.Write(buffer, offset, count);
		if (_countedFileSize.HasValue)
		{
			_countedFileSize += count;
		}
	}

	public void Flush()
	{
		_fileStream.Flush();
	}

	public void Dispose()
	{
		SafeCloseFile(_filePath, _fileStream);
	}

	public bool VerifyFileExists()
	{
		return SafeFileExists(_filePath);
	}

	private void MonitorFileHasBeenDeleted()
	{
		if (!SafeFileExists(_filePath))
		{
			InternalLogger.Debug("{0}: Recreating FileStream because no longer File.Exists: '{1}'", _fileTarget, _filePath);
			SafeCloseFile(_filePath, _fileStream);
			_fileStream = _fileTarget.CreateFileStreamWithRetry(this, _fileTarget.BufferSize, initialFileOpen: false);
			long fileInfoSize = RefreshFileBirthTimeUtc(forceRefresh: false);
			_countedFileSize = RefreshCountedFileSize(_fileStream, fileInfoSize);
		}
	}

	private long? RefreshCountedFileSize(Stream fileStream, long fileInfoSize)
	{
		if (_fileTarget.ArchiveAboveSize > 0)
		{
			if (_fileTarget.GetType().Equals(typeof(FileTarget)))
			{
				return fileStream.Length;
			}
			if (fileStream.GetType().Equals(typeof(FileStream)))
			{
				return null;
			}
			return fileInfoSize;
		}
		return null;
	}

	private void SafeCloseFile(string filepath, Stream? fileStream)
	{
		try
		{
			fileStream?.Dispose();
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "{0}: Failed to close file: '{1}'", _fileTarget, filepath);
		}
	}

	private bool SafeFileExists(string filepath)
	{
		try
		{
			return File.Exists(filepath);
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "{0}: Failed to check if File.Exists: '{1}'", _fileTarget, filepath);
			return false;
		}
	}

	public override string ToString()
	{
		return _filePath;
	}
}
