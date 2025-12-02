using System;
using System.IO;
using System.Security;
using NLog.Common;
using NLog.Internal;
using NLog.Time;

namespace NLog.Targets.FileAppenders;

[SecuritySafeCritical]
internal sealed class MinimalFileLockingAppender : IFileAppender, IDisposable
{
	private readonly FileTarget _fileTarget;

	private readonly string _filePath;

	private bool _initialFileOpen;

	private DateTime _nextArchiveTime;

	private DateTime _lastFileBirthTimeUtc;

	public string FilePath => _filePath;

	public DateTime OpenStreamTime { get; }

	public DateTime LastWriteTime { get; private set; }

	public DateTime FileLastModified
	{
		get
		{
			try
			{
				FileInfo fileInfo = new FileInfo(_filePath);
				if (fileInfo.Exists && fileInfo.Length != 0L)
				{
					return TimeSource.Current.FromSystemTime(fileInfo.LastWriteTimeUtc);
				}
				return OpenStreamTime;
			}
			catch (Exception ex)
			{
				InternalLogger.Error(ex, "{0}: Failed to lookup FileInfo.LastWriteTimeUtc for file: {1}", _fileTarget, _filePath);
				if (ex.MustBeRethrown())
				{
					throw;
				}
				return OpenStreamTime;
			}
		}
	}

	public DateTime NextArchiveTime
	{
		get
		{
			if (_nextArchiveTime < TimeSource.Current.Time.AddMinutes(1.0) || _lastFileBirthTimeUtc == DateTime.MinValue)
			{
				FileInfo fileInfo = new FileInfo(_filePath);
				DateTime dateTime = ((fileInfo.Exists && fileInfo.Length != 0L) ? fileInfo.LookupValidFileCreationTimeUtc() : DateTime.MinValue);
				if (dateTime == DateTime.MinValue || _lastFileBirthTimeUtc < dateTime)
				{
					DateTime fileBirthTime = ((dateTime.Year > 1980) ? TimeSource.Current.FromSystemTime(dateTime) : OpenStreamTime);
					_nextArchiveTime = FileTarget.CalculateNextArchiveEventTime(_fileTarget.ArchiveEvery, fileBirthTime);
					_lastFileBirthTimeUtc = dateTime;
				}
			}
			return _nextArchiveTime;
		}
	}

	public long FileSize
	{
		get
		{
			try
			{
				FileInfo fileInfo = new FileInfo(_filePath);
				return fileInfo.Exists ? fileInfo.Length : 0;
			}
			catch (Exception ex)
			{
				InternalLogger.Error(ex, "{0}: Failed to lookup FileInfo.Length for file: {1}", _fileTarget, _filePath);
				if (ex.MustBeRethrown())
				{
					throw;
				}
				return 0L;
			}
		}
	}

	public MinimalFileLockingAppender(FileTarget fileTarget, string filePath)
	{
		_fileTarget = fileTarget;
		_filePath = filePath;
		_initialFileOpen = true;
		OpenStreamTime = (LastWriteTime = TimeSource.Current.Time);
	}

	public void Write(byte[] buffer, int offset, int count)
	{
		int bufferSize = Math.Min((count / 4096 + 1) * 4096, _fileTarget.BufferSize);
		bool initialFileOpen = _initialFileOpen;
		_initialFileOpen = false;
		using (Stream stream = _fileTarget.CreateFileStreamWithRetry(this, bufferSize, initialFileOpen))
		{
			stream.Write(buffer, offset, count);
			if (_fileTarget.ReplaceFileContentsOnEachWrite)
			{
				byte[] footerLayoutBytes = _fileTarget.GetFooterLayoutBytes();
				if (footerLayoutBytes != null && footerLayoutBytes.Length != 0)
				{
					stream.Write(footerLayoutBytes, 0, footerLayoutBytes.Length);
				}
			}
		}
		LastWriteTime = TimeSource.Current.Time;
	}

	public void Dispose()
	{
	}

	public void Flush()
	{
	}

	public bool VerifyFileExists()
	{
		return FileSize != 0;
	}

	public override string ToString()
	{
		return _filePath;
	}
}
