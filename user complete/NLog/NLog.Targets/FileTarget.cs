using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NLog.Common;
using NLog.Internal;
using NLog.Internal.Fakeables;
using NLog.Layouts;
using NLog.Targets.FileAppenders;
using NLog.Targets.FileArchiveHandlers;
using NLog.Time;

namespace NLog.Targets;

/// <summary>
/// FileTarget for writing formatted messages to one or more log-files.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/File-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/File-target">Documentation on NLog Wiki</seealso>
[Target("File")]
public class FileTarget : TargetWithLayoutHeaderAndFooter
{
	private struct OpenFileAppender
	{
		public IFileAppender FileAppender { get; }

		public int SequenceNumber { get; }

		public OpenFileAppender(IFileAppender fileAppender, int sequenceNumber)
		{
			FileAppender = fileAppender;
			SequenceNumber = sequenceNumber;
		}
	}

	private Layout _fileName = NLog.Layouts.Layout.Empty;

	private string? _fixedFileName;

	private Encoding _encoding = System.Text.Encoding.UTF8;

	private bool? _writeBom;

	private string? _archiveDateFormat;

	private long _archiveAboveSize;

	private FileArchivePeriod _archiveEvery;

	private Layout? _archiveFileName;

	private static readonly string _legacyDateArchiveSuffixFormat = "_{1:yyyyMMdd}_{0:00}";

	private static readonly string _legacySequenceArchiveSuffixFormat = "_{0:00}";

	private int _maxArchiveFiles = -1;

	private int _maxArchiveDays;

	private string? _archiveNumbering;

	private string? _archiveSuffixFormat;

	private bool _archiveSuffixFormatLegacy;

	private IFileArchiveHandler? _fileArchiveHandler;

	private readonly Dictionary<string, OpenFileAppender> _openFileCache = new Dictionary<string, OpenFileAppender>(StringComparer.OrdinalIgnoreCase);

	private readonly ReusableStreamCreator _reusableFileWriteStream = new ReusableStreamCreator();

	private readonly ReusableStreamCreator _reusableBatchFileWriteStream = new ReusableStreamCreator(batchStream: true);

	private readonly ReusableBufferCreator _reusableEncodingBuffer = new ReusableBufferCreator(1024);

	private readonly SortHelpers.KeySelector<AsyncLogEventInfo, string> _getFileNameFromLayout;

	private Timer? _openFileMonitorTimer;

	private string _lastFileNameFromLayout = string.Empty;

	private static readonly char[] DirectorySeparatorChars = new char[2]
	{
		Path.DirectorySeparatorChar,
		Path.AltDirectorySeparatorChar
	};

	private static readonly HashSet<char> InvalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());

	/// <summary>
	/// Gets or sets the name of the file to write to.
	/// </summary>
	/// <remarks>
	/// <b>[Required]</b> Default: <see cref="F:NLog.Layouts.Layout.Empty" /> .
	/// When not absolute path then relative path will be resolved against <see cref="P:System.AppDomain.BaseDirectory" />.
	/// The FileName Layout supports layout-renderers, where a single FileTarget can write to multiple files.
	/// </remarks>
	/// <example>
	/// The following value makes NLog write logging events to files based on the log level in the directory where
	/// the application runs.
	/// <code>${basedir}/${level}.log</code>
	/// All <c>Debug</c> messages will go to <c>Debug.log</c>, all <c>Info</c> messages will go to <c>Info.log</c> and so on.
	/// You can combine as many of the layout renderers as you want to produce an arbitrary log file name.
	/// </example>
	/// <docgen category="General Options" order="2" />
	public Layout FileName
	{
		get
		{
			return _fileName;
		}
		set
		{
			_fileName = value;
			_fixedFileName = ((value is SimpleLayout { IsFixedText: not false } simpleLayout) ? simpleLayout.FixedText : null);
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether to create directories if they do not exist.
	/// </summary>
	/// <remarks>
	/// Default: <see langword="true" /> .
	/// Setting this to <see langword="false" /> may improve performance a bit, but will always fail
	/// when attempt writing to a non-existing directory.
	/// </remarks>
	/// <docgen category="Output Options" order="50" />
	public bool CreateDirs { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to delete old log file on startup.
	/// </summary>
	/// <remarks>
	/// Default: <see langword="false" /> .
	/// When current log-file exists, then it is deleted (and resetting sequence number)
	/// </remarks>
	/// <docgen category="Output Options" order="50" />
	public bool DeleteOldFileOnStartup { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to replace file contents on each write instead of appending log message at the end.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Output Options" order="100" />
	public bool ReplaceFileContentsOnEachWrite { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to keep log file open instead of opening and closing it on each logging event.
	/// </summary>
	/// <remarks>
	/// Default: <see langword="true" /> .
	/// KeepFileOpen = <see langword="true" /> gives the best performance, and ensure the file-lock is not lost to other applications.<br />
	/// KeepFileOpen = <see langword="false" /> gives the best compatibility, but slow performance and lead to file-locking issues with other applications.
	/// </remarks>
	/// <docgen category="Performance Tuning Options" order="10" />
	public bool KeepFileOpen { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to enable log file(s) to be deleted.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Output Options" order="50" />
	public bool EnableFileDelete { get; set; } = true;

	/// <summary>
	/// Gets or sets the line ending mode.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Targets.LineEndingMode.Default" /></remarks>
	/// <docgen category="Output Options" order="100" />
	public LineEndingMode LineEnding { get; set; } = LineEndingMode.Default;

	/// <summary>
	/// Gets or sets a value indicating whether to automatically flush the file buffers after each log message.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Performance Tuning Options" order="50" />
	public bool AutoFlush { get; set; } = true;

	/// <summary>
	/// Gets or sets the maximum number of files to be kept open.
	/// </summary>
	/// <remarks>
	/// Default: <see langword="5" /> . Higher number might improve performance when single FileTarget
	/// is writing to many files (such as splitting by loglevel or by logger-name).
	/// Files are closed in LRU (least recently used) ordering, so files unused
	/// for longest period are closed first. Careful with number higher than 10-15,
	/// because a large number of open files consumes system resources.
	/// </remarks>
	/// <docgen category="Performance Tuning Options" order="10" />
	public int OpenFileCacheSize { get; set; } = 5;

	/// <summary>
	/// Gets or sets the maximum number of seconds that files are kept open. Zero or negative means disabled.
	/// </summary>
	/// <remarks>Default: <see langword="0" /></remarks>
	/// <docgen category="Performance Tuning Options" order="50" />
	public int OpenFileCacheTimeout { get; set; }

	/// <summary>
	/// Gets or sets the maximum number of seconds before open files are flushed. Zero or negative means disabled.
	/// </summary>
	/// <remarks>Default: <see langword="0" /></remarks>
	/// <docgen category="Performance Tuning Options" order="50" />
	public int OpenFileFlushTimeout { get; set; }

	/// <summary>
	/// Gets or sets the log file buffer size in bytes.
	/// </summary>
	/// <remarks>Default: <see langword="32768" /></remarks>
	/// <docgen category="Performance Tuning Options" order="50" />
	public int BufferSize { get; set; } = 32768;

	/// <summary>
	/// Gets or sets the file encoding.
	/// </summary>
	/// <remarks>Default: <see cref="P:System.Text.Encoding.UTF8" /></remarks>
	/// <docgen category="Output Options" order="10" />
	public Encoding Encoding
	{
		get
		{
			return _encoding;
		}
		set
		{
			_encoding = value;
			if (!_writeBom.HasValue && InitialValueBom(value))
			{
				_writeBom = true;
			}
		}
	}

	/// <summary>
	/// Gets or sets whether or not this target should just discard all data that its asked to write.
	/// Mostly used for when testing NLog Stack except final write
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Output Options" order="100" />
	public bool DiscardAll { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to write BOM (byte order mark) in created files.
	/// </summary>
	/// <remarks>Default: <see langword="false" /> (Unless UTF16 / UTF32)</remarks>
	/// <docgen category="Output Options" order="50" />
	public bool WriteBom
	{
		get
		{
			return _writeBom == true;
		}
		set
		{
			_writeBom = value;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether any existing log-file should be archived on startup.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Archival Options" order="50" />
	public bool ArchiveOldFileOnStartup { get; set; }

	/// <summary>
	/// Gets or sets whether to write the Header on initial creation of file appender, even if the file is not empty.
	/// Default value is <see langword="false" />, which means only write header when initial file is empty (Ex. ensures valid CSV files)
	/// </summary>
	/// <remarks>
	/// Default: <see langword="false" /> .
	/// Alternative use <see cref="P:NLog.Targets.FileTarget.ArchiveOldFileOnStartup" /> to ensure each application session gets individual log-file.
	/// </remarks>
	/// <docgen category="Archival Options" order="50" />
	public bool WriteHeaderWhenInitialFileNotEmpty { get; set; }

	/// <summary>
	/// Gets or sets a value specifying the date format when using <see cref="P:NLog.Targets.FileTarget.ArchiveFileName" />.
	/// Obsolete and only here for Legacy reasons, instead use <see cref="P:NLog.Targets.FileTarget.ArchiveSuffixFormat" />.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Archival Options" order="50" />
	[Obsolete("Instead use ArchiveSuffixFormat. Marked obsolete with NLog 6.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public string? ArchiveDateFormat
	{
		get
		{
			return _archiveDateFormat;
		}
		set
		{
			if (!string.Equals(value, _archiveDateFormat))
			{
				if (!string.IsNullOrEmpty(value))
				{
					ArchiveSuffixFormat = "_{1:" + value + "}_{0:00}";
				}
				_archiveDateFormat = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the size in bytes above which log files will be automatically archived. Zero or negative means disabled.
	/// </summary>
	/// <remarks>Default: <see langword="0" /></remarks>
	/// <docgen category="Archival Options" order="50" />
	public long ArchiveAboveSize
	{
		get
		{
			return _archiveAboveSize;
		}
		set
		{
			_archiveAboveSize = ((value > 0) ? value : 0);
			_fileArchiveHandler = null;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether to trigger archive operation based on time-period, by moving active-file to file-path specified by <see cref="P:NLog.Targets.FileTarget.ArchiveFileName" />
	/// </summary>
	/// <remarks>
	/// Default: <see cref="F:NLog.Targets.FileArchivePeriod.None" /> .
	/// Archive move operation only works if <see cref="P:NLog.Targets.FileTarget.FileName" /> is static in nature, and not rolling automatically because of ${date} or ${shortdate}
	///
	/// NLog FileTarget probes the file-birthtime to recognize when time-period has passed, but file-birthtime is not supported by all filesystems.
	/// </remarks>
	/// <docgen category="Archival Options" order="50" />
	public FileArchivePeriod ArchiveEvery
	{
		get
		{
			return _archiveEvery;
		}
		set
		{
			_archiveEvery = value;
			_fileArchiveHandler = null;
		}
	}

	/// <summary>
	/// Legacy archive logic where file-archive-logic moves active file to path specified by <see cref="P:NLog.Targets.FileTarget.ArchiveFileName" />, and then recreates the active file.
	///
	/// Use <see cref="P:NLog.Targets.FileTarget.ArchiveSuffixFormat" /> to control suffix format, instead of now obsolete token {#}
	/// </summary>
	/// <remarks>
	/// Default: <see langword="null" /> .
	/// Archive file-move operation only works if <see cref="P:NLog.Targets.FileTarget.FileName" /> is static in nature, and not rolling automatically because of ${date} or ${shortdate} .
	///
	/// Legacy archive file-move operation can fail because of file-locks, so file-archiving can stop working because of environment issues (Other applications locking files).
	///
	/// Avoid using <see cref="P:NLog.Targets.FileTarget.ArchiveFileName" /> when possible, and instead rely on only using <see cref="P:NLog.Targets.FileTarget.FileName" /> and <see cref="P:NLog.Targets.FileTarget.ArchiveSuffixFormat" />.
	/// </remarks>
	/// <docgen category="Archival Options" order="50" />
	public Layout? ArchiveFileName
	{
		get
		{
			Layout layout = _archiveFileName;
			if (layout == null)
			{
				if (!_archiveSuffixFormatLegacy)
				{
					return null;
				}
				layout = FileName;
			}
			return layout;
		}
		set
		{
			string text = _archiveSuffixFormat;
			if (value is SimpleLayout simpleLayout)
			{
				if ((simpleLayout.OriginalText.IndexOf("${date", StringComparison.OrdinalIgnoreCase) >= 0 || simpleLayout.OriginalText.IndexOf("${shortdate", StringComparison.OrdinalIgnoreCase) >= 0) && (_archiveSuffixFormat == null || (object)_legacySequenceArchiveSuffixFormat == _archiveSuffixFormat || (object)_legacyDateArchiveSuffixFormat == _archiveSuffixFormat))
				{
					text = "_{0}";
				}
				if (simpleLayout.OriginalText.Contains('#'))
				{
					string txt = simpleLayout.OriginalText.Replace(".{#}", string.Empty).Replace("_{#}", "").Replace("-{#}", "")
						.Replace("{#}", "")
						.Replace(".{#", "")
						.Replace("_{#", "")
						.Replace("-{#", "")
						.Replace("{#", "")
						.Replace("#}", "")
						.Replace("#", "");
					text = _archiveSuffixFormat ?? _legacySequenceArchiveSuffixFormat;
					value = new SimpleLayout(txt);
				}
			}
			_archiveFileName = value;
			if ((object)_archiveSuffixFormat != text && text != null)
			{
				ArchiveSuffixFormat = text;
			}
			_fileArchiveHandler = null;
		}
	}

	/// <summary>
	/// Gets or sets the maximum number of archive files that should be kept. Negative means disabled.
	/// </summary>
	/// <remarks>Default: <see langword="-1" /></remarks>
	/// <docgen category="Archival Options" order="50" />
	public int MaxArchiveFiles
	{
		get
		{
			return _maxArchiveFiles;
		}
		set
		{
			_maxArchiveFiles = value;
			_fileArchiveHandler = null;
		}
	}

	/// <summary>
	/// Gets or sets the maximum days of archive files that should be kept. Zero or negative means disabled.
	/// </summary>
	/// <remarks>Default: <see langword="0" /></remarks>
	/// <docgen category="Archival Options" order="50" />
	public int MaxArchiveDays
	{
		get
		{
			return _maxArchiveDays;
		}
		set
		{
			_maxArchiveDays = ((value > 0) ? value : 0);
			_fileArchiveHandler = null;
		}
	}

	/// <summary>
	/// Gets or sets the way file archives are numbered.
	/// Obsolete and only here for Legacy reasons, instead use <see cref="P:NLog.Targets.FileTarget.ArchiveSuffixFormat" />.
	/// </summary>
	/// <remarks>Default: <c>Sequence</c></remarks>
	/// <docgen category="Archival Options" order="50" />
	[Obsolete("Instead use ArchiveSuffixFormat. Marked obsolete with NLog 6.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public string ArchiveNumbering
	{
		get
		{
			return _archiveNumbering ?? "Sequence";
		}
		set
		{
			if (!string.Equals(value, _archiveNumbering))
			{
				_archiveNumbering = (string.IsNullOrEmpty(value) ? null : value.Trim());
				if (_archiveNumbering != null && !string.IsNullOrEmpty(_archiveNumbering) && (_archiveSuffixFormat == null || (object)_archiveSuffixFormat == _legacyDateArchiveSuffixFormat || (object)_archiveSuffixFormat == _legacySequenceArchiveSuffixFormat))
				{
					ArchiveSuffixFormat = ((_archiveNumbering.IndexOf("date", StringComparison.OrdinalIgnoreCase) >= 0) ? _legacyDateArchiveSuffixFormat : _legacySequenceArchiveSuffixFormat);
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets the format-string to convert archive sequence-number by using string.Format
	/// </summary>
	/// <remarks>
	/// Default: <c>_{0:00}</c> .
	/// Ex. to prefix archive sequence number with leading zero's then one can use _{0:000} .
	///
	/// Legacy archive-logic with <see cref="P:NLog.Targets.FileTarget.ArchiveFileName" /> can use suffix _{1:yyyyMMdd}_{0:00} .
	/// </remarks>
	/// <docgen category="Archival Options" order="50" />
	public string ArchiveSuffixFormat
	{
		get
		{
			if (ArchiveEvery != FileArchivePeriod.None && (_archiveSuffixFormat == null || (object)_legacyDateArchiveSuffixFormat == _archiveSuffixFormat || (object)_legacySequenceArchiveSuffixFormat == _archiveSuffixFormat) && ArchiveFileName != null)
			{
				return ArchiveEvery switch
				{
					FileArchivePeriod.Year => "_{1:yyyy}_{0:00}", 
					FileArchivePeriod.Month => "_{1:yyyyMM}_{0:00}", 
					FileArchivePeriod.Hour => "_{1:yyyyMMddHH}_{0:00}", 
					FileArchivePeriod.Minute => "_{1:yyyyMMddHHmm}_{0:00}", 
					_ => _legacyDateArchiveSuffixFormat, 
				};
			}
			return _archiveSuffixFormat ?? _legacySequenceArchiveSuffixFormat;
		}
		set
		{
			if (!string.IsNullOrEmpty(value) && ArchiveFileName is SimpleLayout simpleLayout && StringHelpers.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(simpleLayout.OriginalText)) && value.IndexOf('_') == 0)
			{
				value = value.Substring(1);
			}
			_archiveSuffixFormat = value;
			string? archiveSuffixFormat = _archiveSuffixFormat;
			_archiveSuffixFormatLegacy = archiveSuffixFormat != null && archiveSuffixFormat.IndexOf("{1", StringComparison.Ordinal) >= 0;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether the footer should be written only when the file is archived.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Archival Options" order="50" />
	public bool WriteFooterOnArchivingOnly { get; set; }

	private int OpenFileMonitorTimerInterval
	{
		get
		{
			if (OpenFileFlushTimeout <= 0 || AutoFlush || !KeepFileOpen)
			{
				if (OpenFileCacheTimeout <= 500 || OpenFileCacheTimeout >= 3600)
				{
					return OpenFileCacheTimeout;
				}
				return 300;
			}
			if (OpenFileCacheTimeout <= 0)
			{
				return OpenFileFlushTimeout;
			}
			return Math.Min(OpenFileFlushTimeout, OpenFileCacheTimeout);
		}
	}

	private IFileArchiveHandler FileAchiveHandler => _fileArchiveHandler ?? (_fileArchiveHandler = CreateFileArchiveHandler());

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.FileTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	public FileTarget()
	{
		_getFileNameFromLayout = (AsyncLogEventInfo l) => GetFileNameFromLayout(l.LogEvent);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.FileTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	/// <param name="name">Name of the target.</param>
	public FileTarget(string name)
		: this()
	{
		base.Name = name;
	}

	/// <summary>
	/// Flushes all pending file operations.
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	protected override void FlushAsync(AsyncContinuation asyncContinuation)
	{
		try
		{
			InternalLogger.Trace("{0}: FlushAsync", this);
			if (_openFileCache.Count > 0)
			{
				foreach (KeyValuePair<string, OpenFileAppender> item in _openFileCache)
				{
					item.Value.FileAppender.Flush();
				}
			}
			asyncContinuation(null);
			InternalLogger.Trace("{0}: FlushAsync Done", this);
		}
		catch (Exception exception)
		{
			if (ExceptionMustBeRethrown(exception, "FlushAsync"))
			{
				throw;
			}
			asyncContinuation(exception);
		}
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		if (FileName == null || FileName == NLog.Layouts.Layout.Empty)
		{
			throw new NLogConfigurationException("FileTarget FileName-property must be assigned. FileName is needed for file writing.");
		}
		if (OpenFileMonitorTimerInterval > 0)
		{
			_openFileMonitorTimer = new Timer(OpenFileMonitorTimer);
		}
		base.InitializeTarget();
	}

	/// <inheritdoc />
	protected override void CloseTarget()
	{
		Timer? openFileMonitorTimer = _openFileMonitorTimer;
		_openFileMonitorTimer = null;
		openFileMonitorTimer?.Change(-1, -1);
		openFileMonitorTimer?.Dispose();
		if (_openFileCache.Count > 0)
		{
			foreach (KeyValuePair<string, OpenFileAppender> item in _openFileCache.ToList())
			{
				CloseFileWithFooter(item.Key, item.Value, archiveFile: false);
			}
			_openFileCache.Clear();
		}
		base.CloseTarget();
	}

	/// <inheritdoc />
	protected override void Write(LogEventInfo logEvent)
	{
		string fileNameFromLayout = GetFileNameFromLayout(logEvent);
		if (string.IsNullOrEmpty(fileNameFromLayout))
		{
			throw new ArgumentException("The path is not of a legal form.");
		}
		try
		{
			ReusableObjectCreator<MemoryStream>.LockOject lockOject = _reusableBatchFileWriteStream.Allocate();
			try
			{
				ReusableObjectCreator<StringBuilder>.LockOject lockOject2 = ReusableLayoutBuilder.Allocate();
				try
				{
					ReusableObjectCreator<char[]>.LockOject lockOject3 = _reusableEncodingBuffer.Allocate();
					try
					{
						RenderFormattedMessageToStream(logEvent, lockOject2.Result, lockOject3.Result, lockOject.Result);
					}
					finally
					{
						((IDisposable)lockOject3/*cast due to .constrained prefix*/).Dispose();
					}
				}
				finally
				{
					((IDisposable)lockOject2/*cast due to .constrained prefix*/).Dispose();
				}
				WriteBytesToFile(fileNameFromLayout, logEvent, lockOject.Result);
			}
			finally
			{
				((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "{0}: Failed writing to FileName: '{1}'", this, fileNameFromLayout);
			throw;
		}
	}

	/// <inheritdoc />
	protected override void Write(IList<AsyncLogEventInfo> logEvents)
	{
		foreach (KeyValuePair<string, IList<AsyncLogEventInfo>> item in logEvents.BucketSort(_getFileNameFromLayout))
		{
			if (string.IsNullOrEmpty(item.Key))
			{
				InternalLogger.Warn("{0}: FileName Layout returned empty string. The path is not of a legal form.", this);
				ArgumentException exception = new ArgumentException("The path is not of a legal form.");
				for (int i = 0; i < logEvents.Count; i++)
				{
					logEvents[i].Continuation(exception);
				}
				continue;
			}
			try
			{
				WriteLogEventsToFile(item.Key, item.Value);
			}
			catch (Exception ex)
			{
				InternalLogger.Error(ex, "{0}: Failed writing to FileName: '{1}'", this, item.Key);
				if (ExceptionMustBeRethrown(ex, "Write"))
				{
					throw;
				}
				for (int j = 0; j < item.Value.Count; j++)
				{
					item.Value[j].Continuation(ex);
				}
			}
		}
	}

	private void WriteLogEventsToFile(string filename, IList<AsyncLogEventInfo> logEvents)
	{
		ReusableObjectCreator<MemoryStream>.LockOject lockOject = _reusableBatchFileWriteStream.Allocate();
		try
		{
			MemoryStream memoryStream = lockOject.Result ?? new MemoryStream();
			int num = 0;
			while (num < logEvents.Count)
			{
				memoryStream.Position = 0L;
				memoryStream.SetLength(0L);
				int num2 = WriteToMemoryStream(logEvents, num, memoryStream);
				WriteBytesToFile(filename, logEvents[num].LogEvent, memoryStream);
				for (int i = 0; i < num2; i++)
				{
					logEvents[num++].Continuation(null);
				}
			}
		}
		finally
		{
			((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private string GetFileNameFromLayout(LogEventInfo logEvent)
	{
		if (_fixedFileName != null)
		{
			return _fixedFileName;
		}
		string text = _lastFileNameFromLayout;
		ReusableObjectCreator<StringBuilder>.LockOject lockOject = ReusableLayoutBuilder.Allocate();
		try
		{
			FileName.Render(logEvent, lockOject.Result);
			if (lockOject.Result.EqualTo(text))
			{
				return _lastFileNameFromLayout;
			}
			text = lockOject.Result.ToString();
		}
		finally
		{
			((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
		}
		_lastFileNameFromLayout = text;
		return text;
	}

	private int WriteToMemoryStream(IList<AsyncLogEventInfo> logEvents, int startIndex, MemoryStream ms)
	{
		long num = BufferSize * 100;
		ReusableObjectCreator<MemoryStream>.LockOject lockOject = _reusableFileWriteStream.Allocate();
		try
		{
			ReusableObjectCreator<StringBuilder>.LockOject lockOject2 = ReusableLayoutBuilder.Allocate();
			try
			{
				ReusableObjectCreator<char[]>.LockOject lockOject3 = _reusableEncodingBuffer.Allocate();
				try
				{
					StringBuilder result = lockOject2.Result;
					char[] result2 = lockOject3.Result;
					MemoryStream result3 = lockOject.Result;
					for (int i = startIndex; i < logEvents.Count; i++)
					{
						result3.Position = 0L;
						result3.SetLength(0L);
						result.ClearBuilder();
						RenderFormattedMessageToStream(logEvents[i].LogEvent, result, result2, result3);
						ms.Write(result3.GetBuffer(), 0, (int)result3.Length);
						if (ms.Length > num && !ReplaceFileContentsOnEachWrite)
						{
							return i - startIndex + 1;
						}
					}
				}
				finally
				{
					((IDisposable)lockOject3/*cast due to .constrained prefix*/).Dispose();
				}
			}
			finally
			{
				((IDisposable)lockOject2/*cast due to .constrained prefix*/).Dispose();
			}
		}
		finally
		{
			((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
		}
		return logEvents.Count - startIndex;
	}

	private void RenderFormattedMessageToStream(LogEventInfo logEvent, StringBuilder formatBuilder, char[] transformBuffer, MemoryStream streamTarget)
	{
		RenderFormattedMessage(logEvent, formatBuilder);
		formatBuilder.Append(LineEnding.NewLineCharacters);
		formatBuilder.CopyToStream(streamTarget, Encoding, transformBuffer);
	}

	/// <summary>
	/// Formats the log event for write.
	/// </summary>
	/// <param name="logEvent">The log event to be formatted.</param>
	/// <param name="target"><see cref="T:System.Text.StringBuilder" /> for the result.</param>
	protected virtual void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
	{
		Layout.Render(logEvent, target);
	}

	private void WriteBytesToFile(string filename, LogEventInfo firstLogEvent, MemoryStream ms)
	{
		bool hasWritten = true;
		if (!_openFileCache.TryGetValue(filename, out var value))
		{
			hasWritten = false;
			value = OpenFile(filename, firstLogEvent, null);
		}
		try
		{
			if (ArchiveAboveSize > 0 || ArchiveEvery != FileArchivePeriod.None)
			{
				value = RollArchiveFile(filename, value, firstLogEvent, hasWritten);
			}
			value.FileAppender.Write(ms.GetBuffer(), 0, (int)ms.Length);
			if (AutoFlush)
			{
				value.FileAppender.Flush();
			}
		}
		catch
		{
			_openFileCache.Remove(filename);
			value.FileAppender.Dispose();
			throw;
		}
	}

	private OpenFileAppender RollArchiveFile(string filename, OpenFileAppender openFile, LogEventInfo firstLogEvent, bool hasWritten)
	{
		int num = -1;
		bool flag = ArchiveFileName == null;
		while (num != openFile.SequenceNumber && MustArchiveFile(openFile.FileAppender, firstLogEvent))
		{
			num = openFile.SequenceNumber;
			DateTime? dateTime = (flag ? ((DateTime?)null) : new DateTime?(openFile.FileAppender.FileLastModified));
			if (dateTime > openFile.FileAppender.LastWriteTime)
			{
				if (!(dateTime == openFile.FileAppender.OpenStreamTime))
				{
					DateTime date = firstLogEvent.TimeStamp.Date;
					DateTime? obj = dateTime?.Date;
					if (!(date == obj))
					{
						goto IL_00ff;
					}
				}
				dateTime = openFile.FileAppender.LastWriteTime;
			}
			goto IL_00ff;
			IL_00ff:
			if (hasWritten)
			{
				CloseFileWithFooter(filename, openFile, archiveFile: true);
			}
			else
			{
				CloseFile(filename, openFile);
			}
			hasWritten = false;
			openFile = OpenFile(filename, firstLogEvent, dateTime, openFile.SequenceNumber + 1);
		}
		return openFile;
	}

	private bool MustArchiveFile(IFileAppender fileAppender, LogEventInfo firstLogEvent)
	{
		if (ArchiveAboveSize > 0 && MustArchiveBySize(fileAppender))
		{
			return true;
		}
		if (ArchiveEvery != FileArchivePeriod.None && MustArchiveEveryTimePeriod(fileAppender, firstLogEvent))
		{
			return true;
		}
		return false;
	}

	private bool MustArchiveBySize(IFileAppender fileAppender)
	{
		long fileSize = fileAppender.FileSize;
		if (fileSize == 0L || fileSize + 1 < ArchiveAboveSize)
		{
			return false;
		}
		InternalLogger.Debug("{0}: Archive because of filesize={1} of file: {2}", this, fileSize, fileAppender.FilePath);
		return true;
	}

	private bool MustArchiveEveryTimePeriod(IFileAppender fileAppender, LogEventInfo firstLogEvent)
	{
		if (fileAppender.NextArchiveTime >= firstLogEvent.TimeStamp)
		{
			return false;
		}
		InternalLogger.Debug("{0}: Archive because of filetime of file: {1}", this, fileAppender.FilePath);
		return true;
	}

	internal static DateTime CalculateNextArchiveEventTime(FileArchivePeriod archiveEvery, DateTime fileBirthTime)
	{
		return archiveEvery switch
		{
			FileArchivePeriod.Year => new DateTime(fileBirthTime.Year, 1, 1, 0, 0, 0, fileBirthTime.Kind).AddYears(1), 
			FileArchivePeriod.Month => new DateTime(fileBirthTime.Year, fileBirthTime.Month, 1, 0, 0, 0, fileBirthTime.Kind).AddMonths(1), 
			FileArchivePeriod.Day => new DateTime(fileBirthTime.Year, fileBirthTime.Month, fileBirthTime.Day, 0, 0, 0, fileBirthTime.Kind).AddDays(1.0), 
			FileArchivePeriod.Hour => new DateTime(fileBirthTime.Year, fileBirthTime.Month, fileBirthTime.Day, fileBirthTime.Hour, 0, 0, fileBirthTime.Kind).AddHours(1.0), 
			FileArchivePeriod.Minute => new DateTime(fileBirthTime.Year, fileBirthTime.Month, fileBirthTime.Day, fileBirthTime.Hour, fileBirthTime.Minute, 0, fileBirthTime.Kind).AddMinutes(1.0), 
			FileArchivePeriod.Monday => CalculateNextWeekday(fileBirthTime, DayOfWeek.Monday), 
			FileArchivePeriod.Tuesday => CalculateNextWeekday(fileBirthTime, DayOfWeek.Tuesday), 
			FileArchivePeriod.Wednesday => CalculateNextWeekday(fileBirthTime, DayOfWeek.Wednesday), 
			FileArchivePeriod.Thursday => CalculateNextWeekday(fileBirthTime, DayOfWeek.Thursday), 
			FileArchivePeriod.Friday => CalculateNextWeekday(fileBirthTime, DayOfWeek.Friday), 
			FileArchivePeriod.Saturday => CalculateNextWeekday(fileBirthTime, DayOfWeek.Saturday), 
			FileArchivePeriod.Sunday => CalculateNextWeekday(fileBirthTime, DayOfWeek.Sunday), 
			_ => DateTime.MaxValue, 
		};
	}

	/// <summary>
	/// Calculate the DateTime of the requested day of the week.
	/// </summary>
	/// <param name="previousLogEventTimestamp">The DateTime of the previous log event.</param>
	/// <param name="dayOfWeek">The next occurring day of the week to return a DateTime for.</param>
	/// <returns>The DateTime of the next occurring dayOfWeek.</returns>
	/// <remarks>For example: if previousLogEventTimestamp is Thursday 2017-03-02 and dayOfWeek is Sunday, this will return
	///  Sunday 2017-03-05. If dayOfWeek is Thursday, this will return *next* Thursday 2017-03-09.</remarks>
	public static DateTime CalculateNextWeekday(DateTime previousLogEventTimestamp, DayOfWeek dayOfWeek)
	{
		int dayOfWeek2 = (int)previousLogEventTimestamp.DayOfWeek;
		int num = (int)dayOfWeek;
		if (num <= dayOfWeek2)
		{
			num += 7;
		}
		return previousLogEventTimestamp.Date.AddDays(num - dayOfWeek2);
	}

	private OpenFileAppender OpenFile(string filename, LogEventInfo firstLogEvent, DateTime? previousFileLastModified, int sequenceNumber = 0)
	{
		bool num = sequenceNumber == 0 && CreateDirs && _openFileCache.Count == 0;
		PruneOpenFileCache();
		sequenceNumber = FileAchiveHandler.ArchiveBeforeOpenFile(filename, firstLogEvent, previousFileLastModified, sequenceNumber);
		string text = BuildFullFilePath(filename, sequenceNumber);
		if (num)
		{
			InternalLogger.Debug("{0}: Verify directory and creating writer to file: {1}", this, text);
			string directoryName = Path.GetDirectoryName(text);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
		}
		else
		{
			InternalLogger.Debug("{0}: Creating writer to file: {1}", this, text);
		}
		IFileAppender fileAppender = CreateFileAppender(text);
		OpenFileAppender openFileAppender = new OpenFileAppender(fileAppender, sequenceNumber);
		_openFileCache[filename] = openFileAppender;
		if (_openFileCache.Count == 1)
		{
			_openFileMonitorTimer?.Change(OpenFileMonitorTimerInterval * 1000, -1);
		}
		return openFileAppender;
	}

	private void PruneOpenFileCache()
	{
		while (_openFileCache.Count > 0)
		{
			KeyValuePair<string, OpenFileAppender> keyValuePair = default(KeyValuePair<string, OpenFileAppender>);
			foreach (KeyValuePair<string, OpenFileAppender> item in _openFileCache)
			{
				if (!item.Value.FileAppender.VerifyFileExists())
				{
					keyValuePair = item;
					break;
				}
			}
			if (string.IsNullOrEmpty(keyValuePair.Key))
			{
				break;
			}
			CloseFile(keyValuePair.Key, keyValuePair.Value);
		}
		while (_openFileCache.Count >= OpenFileCacheSize)
		{
			DateTime maxValue = DateTime.MaxValue;
			KeyValuePair<string, OpenFileAppender> keyValuePair2 = default(KeyValuePair<string, OpenFileAppender>);
			foreach (KeyValuePair<string, OpenFileAppender> item2 in _openFileCache)
			{
				if (item2.Value.FileAppender.LastWriteTime < maxValue)
				{
					keyValuePair2 = item2;
				}
			}
			if (!string.IsNullOrEmpty(keyValuePair2.Key))
			{
				CloseFileWithFooter(keyValuePair2.Key, keyValuePair2.Value, archiveFile: false);
				continue;
			}
			break;
		}
	}

	internal void CloseOpenFileBeforeArchiveCleanup(string filepath)
	{
		string a = ((_openFileCache.Count > 0) ? Path.GetFileName(filepath) : string.Empty);
		KeyValuePair<string, OpenFileAppender> keyValuePair;
		do
		{
			keyValuePair = default(KeyValuePair<string, OpenFileAppender>);
			foreach (KeyValuePair<string, OpenFileAppender> item in _openFileCache)
			{
				string fileName = Path.GetFileName(item.Value.FileAppender.FilePath);
				if (string.Equals(a, fileName, StringComparison.OrdinalIgnoreCase))
				{
					keyValuePair = item;
					break;
				}
			}
			if (keyValuePair.Key != null)
			{
				InternalLogger.Debug("{0}: Archive cleanup closing file: {1}", this, filepath);
				CloseFile(keyValuePair.Key, keyValuePair.Value);
			}
		}
		while (keyValuePair.Key != null);
	}

	private void CloseFile(string filename, OpenFileAppender openFile)
	{
		try
		{
			_openFileCache.Remove(filename);
			if (_openFileCache.Count == 0)
			{
				_openFileMonitorTimer?.Change(-1, -1);
			}
		}
		finally
		{
			openFile.FileAppender.Dispose();
		}
	}

	private void CloseFileWithFooter(string filename, OpenFileAppender openFile, bool archiveFile)
	{
		try
		{
			if (!ReplaceFileContentsOnEachWrite && (!WriteFooterOnArchivingOnly || archiveFile))
			{
				byte[] footerLayoutBytes = GetFooterLayoutBytes();
				if (footerLayoutBytes != null && footerLayoutBytes.Length != 0)
				{
					openFile.FileAppender.Write(footerLayoutBytes, 0, footerLayoutBytes.Length);
				}
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "{0}: Failed closing file: '{1}'", this, filename);
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
		}
		finally
		{
			CloseFile(filename, openFile);
		}
	}

	private void OpenFileMonitorTimer(object state)
	{
		bool flag = _openFileMonitorTimer != null;
		try
		{
			lock (base.SyncRoot)
			{
				flag = _openFileCache.Count != 0 && _openFileMonitorTimer != null;
				if (OpenFileCacheTimeout > 0)
				{
					PruneOpenFileCacheUsingTimeout();
				}
				if (OpenFileFlushTimeout > 0 && !AutoFlush)
				{
					DateTime dateTime = TimeSource.Current.Time.AddSeconds((double)(-(OpenFileFlushTimeout + 1)) * 1.5);
					foreach (KeyValuePair<string, OpenFileAppender> item in _openFileCache)
					{
						if (item.Value.FileAppender.LastWriteTime > dateTime)
						{
							item.Value.FileAppender.Flush();
						}
					}
				}
				flag = flag && _openFileCache.Count != 0;
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "{0}: Exception in OpenFileMonitorTimer", this);
		}
		finally
		{
			if (flag)
			{
				_openFileMonitorTimer?.Change(OpenFileMonitorTimerInterval * 1000, -1);
			}
		}
	}

	private void PruneOpenFileCacheUsingTimeout()
	{
		DateTime dateTime = TimeSource.Current.Time.AddSeconds(-OpenFileCacheTimeout);
		bool flag = false;
		foreach (KeyValuePair<string, OpenFileAppender> item in _openFileCache)
		{
			if (item.Value.FileAppender.LastWriteTime < dateTime)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		foreach (KeyValuePair<string, OpenFileAppender> item2 in _openFileCache.ToList())
		{
			if (item2.Value.FileAppender.LastWriteTime < dateTime)
			{
				CloseFile(item2.Key, item2.Value);
			}
		}
	}

	internal string BuildFullFilePath(string newFileName, int sequenceNumber, DateTime fileLastModified = default(DateTime))
	{
		if (sequenceNumber > 0 || fileLastModified != default(DateTime))
		{
			string text = Path.GetFileName(newFileName) ?? string.Empty;
			string text2 = Path.GetExtension(text) ?? string.Empty;
			newFileName = newFileName.Substring(0, newFileName.Length - text.Length);
			if (!string.IsNullOrEmpty(text2))
			{
				text = text.Substring(0, text.Length - text2.Length);
			}
			object arg = ((fileLastModified == default(DateTime)) ? string.Empty : ((object)fileLastModified));
			try
			{
				newFileName = newFileName + text + string.Format(ArchiveSuffixFormat, sequenceNumber, arg) + text2;
			}
			catch (Exception ex)
			{
				InternalLogger.Error(ex, "{0}: Failed to apply ArchiveSuffixFormat={1} using SequenceNumber={2} for file: '{3}'", this, ArchiveSuffixFormat, sequenceNumber, newFileName);
				if (ExceptionMustBeRethrown(ex, "BuildFullFilePath"))
				{
					throw;
				}
				newFileName = newFileName + text + string.Format(_legacySequenceArchiveSuffixFormat, sequenceNumber) + text2;
			}
		}
		return CleanFullFilePath(newFileName);
	}

	internal static string CleanFullFilePath(string filename)
	{
		int num = filename.LastIndexOfAny(DirectorySeparatorChars);
		char[] array = null;
		for (int i = num + 1; i < filename.Length; i++)
		{
			if (InvalidFileNameChars.Contains(filename[i]))
			{
				if (array == null)
				{
					array = filename.Substring(num + 1).ToCharArray();
				}
				array[i - (num + 1)] = '_';
			}
		}
		if (array != null)
		{
			filename = Path.Combine((num > 0) ? filename.Substring(0, num + 1) : string.Empty, new string(array));
		}
		return Path.GetFullPath(FileInfoHelper.IsRelativeFilePath(filename) ? Path.Combine(AppEnvironmentWrapper.FixFilePathWithLongUNC(LogManager.LogFactory.CurrentAppEnvironment.AppDomainBaseDirectory), filename) : filename);
	}

	private static bool InitialValueBom(Encoding encoding)
	{
		int num = encoding?.CodePage ?? 0;
		if (num != 1200 && num != 1201 && num != 12000)
		{
			return num == 12001;
		}
		return true;
	}

	/// <summary>
	/// The sequence of <see langword="byte" /> to be written in a file after applying any formatting and any
	/// transformations required from the <see cref="T:NLog.Layouts.Layout" />.
	/// </summary>
	/// <param name="layout">The layout used to render output message.</param>
	/// <returns>Sequence of <see langword="byte" /> to be written.</returns>
	/// <remarks>Usually it is used to render the header and hooter of the files.</remarks>
	private byte[] GetLayoutBytes(Layout layout)
	{
		if (layout == null)
		{
			return ArrayHelper.Empty<byte>();
		}
		ReusableObjectCreator<StringBuilder>.LockOject lockOject = ReusableLayoutBuilder.Allocate();
		try
		{
			ReusableObjectCreator<char[]>.LockOject lockOject2 = _reusableEncodingBuffer.Allocate();
			try
			{
				layout.Render(LogEventInfo.CreateNullEvent(), lockOject.Result);
				lockOject.Result.Append(LineEnding.NewLineCharacters);
				using MemoryStream memoryStream = new MemoryStream(lockOject.Result.Length);
				lockOject.Result.CopyToStream(memoryStream, Encoding, lockOject2.Result);
				return memoryStream.ToArray();
			}
			finally
			{
				((IDisposable)lockOject2/*cast due to .constrained prefix*/).Dispose();
			}
		}
		finally
		{
			((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
		}
	}

	internal byte[] GetFooterLayoutBytes()
	{
		if (base.Footer != null)
		{
			InternalLogger.Trace("{0}: Write footer", this);
			return GetLayoutBytes(base.Footer);
		}
		return ArrayHelper.Empty<byte>();
	}

	internal Stream CreateFileStreamWithRetry(IFileAppender fileAppender, int bufferSize, bool initialFileOpen)
	{
		int num = 1;
		int num2 = ((!KeepFileOpen) ? 5 : 0);
		string filePath = fileAppender.FilePath;
		for (int i = 0; i <= num2; i++)
		{
			try
			{
				return CreateFileStreamWithDirectory(filePath, bufferSize, initialFileOpen);
			}
			catch (DirectoryNotFoundException)
			{
				throw;
			}
			catch (IOException ex2)
			{
				if (i >= num2)
				{
					throw;
				}
				int num3 = ((num > 4) ? new Random().Next(4, num) : num);
				InternalLogger.Warn("{0}: Attempt #{1} to open {2} failed - {3} {4}. Sleeping for {5}ms", this, i, filePath, ex2.GetType(), ex2.Message, num3);
				num *= 4;
				Thread.Sleep(num3);
			}
		}
		throw new InvalidOperationException("Should not be reached.");
	}

	private Stream CreateFileStreamWithDirectory(string filePath, int bufferSize, bool initialFileOpen)
	{
		try
		{
			return OpenNewFileStream(filePath, bufferSize, initialFileOpen);
		}
		catch (DirectoryNotFoundException)
		{
			if (!CreateDirs)
			{
				throw;
			}
			InternalLogger.Debug("{0}: DirectoryNotFoundException - Attempting to create directory for file: {1}", this, filePath);
			string directoryName = Path.GetDirectoryName(filePath);
			try
			{
				Directory.CreateDirectory(directoryName);
			}
			catch (Exception innerException)
			{
				throw new NLogRuntimeException("Could not create directory " + directoryName, innerException);
			}
			return OpenNewFileStream(filePath, bufferSize, initialFileOpen);
		}
	}

	private Stream OpenNewFileStream(string filePath, int bufferSize, bool initialFileOpen)
	{
		Stream stream = CreateFileStream(filePath, bufferSize);
		try
		{
			bool? flag = null;
			if (WriteBom)
			{
				flag = ReplaceFileContentsOnEachWrite || stream.Length == 0;
				if (flag == true)
				{
					InternalLogger.Trace("{0}: Write byte order mark from encoding={1}", this, Encoding);
					byte[] preamble = Encoding.GetPreamble();
					if (preamble.Length != 0)
					{
						stream.Write(preamble, 0, preamble.Length);
					}
				}
			}
			if (base.Header != null && ((initialFileOpen && WriteHeaderWhenInitialFileNotEmpty) || ReplaceFileContentsOnEachWrite || (flag ?? (stream.Length == 0))))
			{
				InternalLogger.Trace("{0}: Write header", this);
				byte[] layoutBytes = GetLayoutBytes(base.Header);
				stream.Write(layoutBytes, 0, layoutBytes.Length);
			}
			return stream;
		}
		catch
		{
			stream?.Dispose();
			throw;
		}
	}

	/// <summary>
	/// Creates stream for appending to the specified <paramref name="filePath" />
	/// </summary>
	/// <param name="filePath">Path of the file to be written</param>
	/// <param name="bufferSize">Wanted internal buffer size for the stream</param>
	/// <returns>Stream for appending to the file</returns>
	protected virtual Stream CreateFileStream(string filePath, int bufferSize)
	{
		FileShare fileShare = FileShare.Read;
		if (EnableFileDelete)
		{
			fileShare |= FileShare.Delete;
		}
		FileMode mode = FileMode.Append;
		if (ReplaceFileContentsOnEachWrite)
		{
			mode = FileMode.Create;
		}
		return new FileStream(filePath, mode, FileAccess.Write, fileShare, bufferSize);
	}

	private IFileAppender CreateFileAppender(string filePath)
	{
		if (DiscardAll)
		{
			return new DiscardAllFileAppender(filePath);
		}
		if (ReplaceFileContentsOnEachWrite)
		{
			return new MinimalFileLockingAppender(this, filePath);
		}
		if (KeepFileOpen)
		{
			return new ExclusiveFileLockingAppender(this, filePath);
		}
		return new MinimalFileLockingAppender(this, filePath);
	}

	private IFileArchiveHandler CreateFileArchiveHandler()
	{
		if (MaxArchiveFiles < 0 && MaxArchiveDays == 0 && ArchiveAboveSize == 0L && ArchiveEvery == FileArchivePeriod.None)
		{
			if (!DeleteOldFileOnStartup && !ArchiveOldFileOnStartup)
			{
				return DisabledFileArchiveHandler.Default;
			}
			if (!ArchiveOldFileOnStartup)
			{
				return new ZeroFileArchiveHandler(this);
			}
		}
		if (MaxArchiveFiles == 0)
		{
			return new ZeroFileArchiveHandler(this);
		}
		if (ArchiveFileName == null)
		{
			return new RollingArchiveFileHandler(this);
		}
		return new LegacyArchiveFileNameHandler(this);
	}
}
