using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class FilePerObjectWriter : ISmoScriptWriter, IDisposable
{
	private const string FILE_EXTENSION = ".sql";

	private const char INVALID_CHARACTER_REPLACEMENT = ' ';

	private Dictionary<Urn, SingleFileWriter> SingleFileWriters;

	private HashSet<string> fileNames;

	private string folderPath;

	private string _header;

	public Encoding Encoding { get; set; }

	public bool AppendToFile { get; set; }

	public string BatchTerminator { get; set; }

	public bool ScriptBatchTerminator { get; set; }

	public int InsertBatchSize { get; set; }

	public string Header
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				_header = value;
			}
		}
	}

	private void Init()
	{
		BatchTerminator = Globals.Go;
		InsertBatchSize = 100;
	}

	public FilePerObjectWriter(string folderPath)
	{
		VerfiyFolderPath(folderPath);
		SingleFileWriters = new Dictionary<Urn, SingleFileWriter>();
		this.folderPath = folderPath;
		Init();
		fileNames = new HashSet<string>();
	}

	private void VerfiyFolderPath(string folderPath)
	{
		if (string.IsNullOrEmpty(folderPath))
		{
			throw new ScriptWriterException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("folderPath"));
		}
		if (!Directory.Exists(folderPath))
		{
			throw new ScriptWriterException(ExceptionTemplatesImpl.FolderPathNotFound);
		}
	}

	public void ScriptObject(IEnumerable<string> script, Urn obj)
	{
		SingleFileWriter fileWriter = GetFileWriter(obj);
		fileWriter.ScriptObject(script, obj);
	}

	public void ScriptData(IEnumerable<string> dataScript, Urn table)
	{
		SingleFileWriter fileWriter = GetFileWriter(table);
		fileWriter.ScriptData(dataScript, table);
	}

	public void ScriptContext(string databaseContext, Urn obj)
	{
		SingleFileWriter fileWriter = GetFileWriter(obj);
		fileWriter.ScriptContext(databaseContext, obj);
	}

	public void Close()
	{
		Dispose();
	}

	public void Dispose()
	{
		foreach (KeyValuePair<Urn, SingleFileWriter> singleFileWriter in SingleFileWriters)
		{
			if (singleFileWriter.Value != null)
			{
				singleFileWriter.Value.Dispose();
			}
		}
	}

	protected virtual string CompleteFileName(string fileName)
	{
		return Path.Combine(folderPath, fileName + ".sql");
	}

	private SingleFileWriter GetSingleFileWriter(string uniqueFileName)
	{
		SingleFileWriter singleFileWriter = ((Encoding == null) ? new SingleFileWriter(CompleteFileName(uniqueFileName), AppendToFile) : new SingleFileWriter(CompleteFileName(uniqueFileName), AppendToFile, Encoding));
		singleFileWriter.ScriptBatchTerminator = ScriptBatchTerminator;
		singleFileWriter.InsertBatchSize = InsertBatchSize;
		singleFileWriter.BatchTerminator = BatchTerminator;
		if (!string.IsNullOrEmpty(_header))
		{
			singleFileWriter.Header = _header;
		}
		return singleFileWriter;
	}

	protected virtual SingleFileWriter GetFileWriter(Urn obj)
	{
		if (obj.Type == "UnresolvedEntity")
		{
			return GetUnresolveEntityWriter();
		}
		if (SingleFileWriters.ContainsKey(obj))
		{
			return SingleFileWriters[obj];
		}
		Urn urn = ObjectKey(obj);
		if (SingleFileWriters.ContainsKey(urn))
		{
			return SingleFileWriters[urn];
		}
		string validFileName = GetValidFileName(urn);
		string text = validFileName;
		int num = 0;
		while (fileNames.Contains(text))
		{
			num++;
			text = string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", new object[2] { validFileName, num });
		}
		SingleFileWriter singleFileWriter = GetSingleFileWriter(text);
		fileNames.Add(text);
		SingleFileWriters.Add(urn, singleFileWriter);
		return singleFileWriter;
	}

	private static Urn ObjectKey(Urn obj)
	{
		if (obj.Type.Equals(DefaultConstraint.UrnSuffix))
		{
			if (obj.Parent.Type.Equals(Column.UrnSuffix))
			{
				return obj.Parent.Parent;
			}
			return obj;
		}
		return ObjectKeyRec(obj);
	}

	private static Urn ObjectKeyRec(Urn key)
	{
		switch (key.Type)
		{
		case "ForeignKey":
		case "Check":
		case "FullTextIndex":
		case "Index":
		case "Trigger":
		case "Column":
		case "Param":
		case "Statistic":
			key = key.Parent;
			break;
		case "ExtendedProperty":
			key = ObjectKey(key.Parent);
			break;
		}
		return key;
	}

	private SingleFileWriter GetUnresolveEntityWriter()
	{
		string text = "UnresolvedEntity";
		Urn key = new Urn(text);
		if (SingleFileWriters.ContainsKey(key))
		{
			return SingleFileWriters[key];
		}
		string text2 = text;
		SingleFileWriter singleFileWriter = GetSingleFileWriter(text2);
		fileNames.Add(text2);
		SingleFileWriters.Add(key, singleFileWriter);
		return singleFileWriter;
	}

	protected virtual string GetValidFileName(Urn urn)
	{
		string text = GetFileName(urn);
		char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
		char[] array = invalidFileNameChars;
		foreach (char oldChar in array)
		{
			text = text.Replace(oldChar, ' ');
		}
		return text;
	}

	protected virtual string GetFileName(Urn urn)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string attribute = urn.GetAttribute("Schema");
		if (attribute != null && attribute.Length > 0)
		{
			stringBuilder.AppendFormat("{0}.", attribute);
		}
		string attribute2 = urn.GetAttribute("Name");
		if (attribute2 == null)
		{
			throw new ScriptWriterException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.FilePerObjectUrnMissingName, new object[1] { urn }));
		}
		stringBuilder.AppendFormat("{0}.{1}", attribute2, urn.Type);
		return stringBuilder.ToString();
	}
}
