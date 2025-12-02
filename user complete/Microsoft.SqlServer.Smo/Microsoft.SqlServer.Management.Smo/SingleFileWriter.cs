using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SingleFileWriter : ISmoScriptWriter, IDisposable
{
	private StreamWriter streamWriter;

	private string currentContext;

	private bool _wroteHeader;

	public string BatchTerminator { get; set; }

	public bool ScriptBatchTerminator { get; set; }

	public int InsertBatchSize { get; set; }

	public string Header
	{
		set
		{
			if (!_wroteHeader && !string.IsNullOrEmpty(value))
			{
				streamWriter.WriteLine(value);
				_wroteHeader = true;
			}
		}
	}

	private void Init(string path, bool appendToFile, Encoding encoding)
	{
		CheckValidFileName(path);
		try
		{
			if (encoding != null)
			{
				streamWriter = NetCoreHelpers.CreateStreamWriter(path, appendToFile, encoding);
			}
			else
			{
				streamWriter = NetCoreHelpers.CreateStreamWriter(path, appendToFile);
			}
		}
		catch (IOException innerException)
		{
			throw new ScriptWriterException(ExceptionTemplatesImpl.FileWritingException, innerException);
		}
		catch (UnauthorizedAccessException innerException2)
		{
			throw new ScriptWriterException(ExceptionTemplatesImpl.FileWritingException, innerException2);
		}
		catch (ArgumentException innerException3)
		{
			throw new ScriptWriterException(ExceptionTemplatesImpl.FileWritingException, innerException3);
		}
		catch (SecurityException innerException4)
		{
			throw new ScriptWriterException(ExceptionTemplatesImpl.FileWritingException, innerException4);
		}
		BatchTerminator = Globals.Go;
		currentContext = string.Empty;
		InsertBatchSize = 100;
		ScriptBatchTerminator = true;
	}

	private void CheckValidFileName(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			throw new ScriptWriterException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("path"));
		}
		string fileName;
		try
		{
			if (!Directory.Exists(Path.GetDirectoryName(Path.GetFullPath(path))))
			{
				throw new ScriptWriterException(ExceptionTemplatesImpl.FolderPathNotFound);
			}
			fileName = Path.GetFileName(path);
		}
		catch (ArgumentException innerException)
		{
			throw new ScriptWriterException(ExceptionTemplatesImpl.FileWritingException, innerException);
		}
		if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
		{
			throw new ScriptWriterException(ExceptionTemplatesImpl.InvalideFileName);
		}
	}

	public SingleFileWriter(string path)
	{
		Init(path, appendToFile: false, null);
	}

	public SingleFileWriter(string path, bool appendToFile)
	{
		Init(path, appendToFile, null);
	}

	public SingleFileWriter(string path, bool appendToFile, Encoding encoding)
	{
		if (encoding == null)
		{
			throw new ScriptWriterException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("encoding"));
		}
		Init(path, appendToFile, encoding);
	}

	public SingleFileWriter(string path, Encoding encoding)
	{
		if (encoding == null)
		{
			throw new ScriptWriterException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("encoding"));
		}
		Init(path, appendToFile: false, encoding);
	}

	public void ScriptObject(IEnumerable<string> script, Urn obj)
	{
		if (ScriptBatchTerminator)
		{
			foreach (string item in script)
			{
				streamWriter.Write(item);
				if (!item.EndsWith(System.Environment.NewLine))
				{
					streamWriter.WriteLine();
				}
				streamWriter.WriteLine(BatchTerminator);
			}
			return;
		}
		foreach (string item2 in script)
		{
			streamWriter.WriteLine(item2);
		}
	}

	public void ScriptData(IEnumerable<string> dataScript, Urn table)
	{
		if (ScriptBatchTerminator && InsertBatchSize > 0)
		{
			int num = 0;
			{
				foreach (string item in dataScript)
				{
					streamWriter.WriteLine(item);
					num++;
					if (num % InsertBatchSize == 0)
					{
						streamWriter.WriteLine(BatchTerminator);
						num = 0;
					}
				}
				return;
			}
		}
		foreach (string item2 in dataScript)
		{
			streamWriter.WriteLine(item2);
		}
	}

	public void ScriptContext(string databaseContext, Urn obj)
	{
		if (!databaseContext.Equals(currentContext, StringComparison.Ordinal))
		{
			streamWriter.WriteLine(databaseContext);
			if (ScriptBatchTerminator)
			{
				streamWriter.WriteLine(BatchTerminator);
			}
			currentContext = databaseContext;
		}
	}

	public void Close()
	{
		Dispose();
	}

	public void Dispose()
	{
		streamWriter.Dispose();
	}
}
