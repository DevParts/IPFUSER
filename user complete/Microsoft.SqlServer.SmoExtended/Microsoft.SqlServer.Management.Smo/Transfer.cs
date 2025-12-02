using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class Transfer : DataTransferBase, ITransferMetadataProvider
{
	private string tempDtsPackageFilesDir = string.Empty;

	public string TemporaryPackageDirectory
	{
		get
		{
			return tempDtsPackageFilesDir;
		}
		set
		{
			tempDtsPackageFilesDir = value;
		}
	}

	public int BatchSize { get; set; }

	public int BulkCopyTimeout { get; set; }

	public event DataTransferEventHandler DataTransferEvent;

	public Transfer()
	{
		BatchSize = 0;
		BulkCopyTimeout = 0;
	}

	public Transfer(Database database)
		: base(database)
	{
		BatchSize = 0;
		BulkCopyTimeout = 0;
	}

	protected static Urn[] ProcessDependencyChain(Server server, DependencyChainCollection dependencyChain, bool isDataOnly, bool isCreateOrder)
	{
		Dictionary<Dependency, bool[]> brokenLinks = new Dictionary<Dependency, bool[]>();
		FindCycles(dependencyChain, server, isDataOnly, isCreateOrder, brokenLinks);
		BreakCycles(brokenLinks);
		List<Urn> list = new List<Urn>(dependencyChain.Count);
		Dictionary<Urn, object> lookupTable = new Dictionary<Urn, object>(dependencyChain.Count);
		foreach (Dependency item in dependencyChain)
		{
			AddDependency(list, lookupTable, item);
		}
		return list.ToArray();
	}

	private static void AddDependency(List<Urn> objectsInOrder, Dictionary<Urn, object> lookupTable, Dependency dependency)
	{
		if (dependency == null || lookupTable.ContainsKey(dependency.Urn))
		{
			return;
		}
		foreach (Dependency link in dependency.Links)
		{
			AddDependency(objectsInOrder, lookupTable, link);
		}
		objectsInOrder.Add(dependency.Urn);
		lookupTable.Add(dependency.Urn, null);
	}

	private static void FindCycles(DependencyChainCollection dependencyChain, Server server, bool isDataOnly, bool isCreateOrder, Dictionary<Dependency, bool[]> BrokenLinks)
	{
		Dictionary<Urn, object> dictionary = new Dictionary<Urn, object>();
		foreach (Dependency item in dependencyChain)
		{
			if (!dictionary.ContainsKey(item.Urn))
			{
				List<Dependency> currentChain = new List<Dependency>();
				Visit(item, currentChain, dictionary, server, isDataOnly, isCreateOrder, BrokenLinks);
			}
		}
	}

	private static void BreakCycles(Dictionary<Dependency, bool[]> BrokenLinks)
	{
		foreach (KeyValuePair<Dependency, bool[]> BrokenLink in BrokenLinks)
		{
			int num = 0;
			for (int i = 0; i < BrokenLink.Value.Length; i++)
			{
				if (BrokenLink.Value[i])
				{
					BrokenLink.Key.Links.RemoveAt(i - num);
					num++;
				}
			}
		}
	}

	private static void MarkNodeForBreaking(Dependency[] cycle, Server server, bool isDataOnly, bool isCreateOrder, Dictionary<Dependency, bool[]> BrokenLinks)
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < cycle.Length; i++)
		{
			string text = cycle[i].Urn.Type.Trim();
			if (text.Equals("StoredProcedure", StringComparison.OrdinalIgnoreCase) || text.Equals("Synonym", StringComparison.OrdinalIgnoreCase))
			{
				num = GetNodeIdForWhichToBreakLink(i, cycle.Length, isCreateOrder);
				break;
			}
			if (text.Equals("UserDefinedFunction", StringComparison.OrdinalIgnoreCase))
			{
				SqlSmoObject smoObject = server.GetSmoObject(cycle[i].Urn);
				if (smoObject is UserDefinedFunction { FunctionType: UserDefinedFunctionType.Scalar, IsSchemaBound: false })
				{
					num = GetNodeIdForWhichToBreakLink(i, cycle.Length, isCreateOrder);
					break;
				}
			}
			if (text.Equals("Default", StringComparison.OrdinalIgnoreCase) && cycle[i].Urn.Parent.Type.Equals("Column", StringComparison.OrdinalIgnoreCase))
			{
				num = GetNodeIdForWhichToBreakLink(i, cycle.Length, isCreateOrder);
				break;
			}
			if (isDataOnly)
			{
				if (!text.Equals("Table", StringComparison.OrdinalIgnoreCase))
				{
					num = GetNodeIdForWhichToBreakLink(i, cycle.Length, isCreateOrder);
					break;
				}
				continue;
			}
			num2 = i + 1;
			if (num2 >= cycle.Length)
			{
				num2 = 0;
			}
			if (text.Equals("Table", StringComparison.OrdinalIgnoreCase) && cycle[num2].Urn.Type.Trim().Equals("Table", StringComparison.OrdinalIgnoreCase))
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			if (isDataOnly)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.CyclicalForeignKeys);
			}
			num = 0;
			TraceHelper.Assert(condition: false, "We could not find a node to break for the cycle");
			SqlSmoObject.Trace("We could not find a node to break for the cycle");
		}
		num2 = num + 1;
		if (num2 >= cycle.Length)
		{
			num2 = 0;
		}
		BrokenLinks[cycle[num]][cycle[num].Links.IndexOf(cycle[num2])] = true;
	}

	private static int GetNodeIdForWhichToBreakLink(int nodeId, int cycleLength, bool isCreateOrder)
	{
		if (isCreateOrder)
		{
			return nodeId;
		}
		if (nodeId == 0)
		{
			return cycleLength - 1;
		}
		return nodeId - 1;
	}

	private static void Visit(Dependency dependency, List<Dependency> currentChain, Dictionary<Urn, object> visitedUrns, Server server, bool isDataOnly, bool isCreateOrder, Dictionary<Dependency, bool[]> BrokenLinks)
	{
		if (currentChain.Contains(dependency))
		{
			List<Dependency> list = new List<Dependency>();
			int num = currentChain.IndexOf(dependency);
			for (int i = num; i < currentChain.Count; i++)
			{
				list.Add(currentChain[i]);
			}
			{
				foreach (Dependency item in list)
				{
					if (!visitedUrns.ContainsKey(item.Urn))
					{
						MarkNodeForBreaking(list.ToArray(), server, isDataOnly, isCreateOrder, BrokenLinks);
						break;
					}
				}
				return;
			}
		}
		if (dependency.Links.Count > 0 && !BrokenLinks.ContainsKey(dependency))
		{
			bool[] array = new bool[dependency.Links.Count];
			MarkFalseDependency(dependency, array, isDataOnly);
			BrokenLinks.Add(dependency, array);
		}
		currentChain.Add(dependency);
		int num2 = 0;
		foreach (Dependency link in dependency.Links)
		{
			if (link != null && link != dependency)
			{
				if (!BrokenLinks[dependency][num2])
				{
					Visit(link, currentChain, visitedUrns, server, isDataOnly, isCreateOrder, BrokenLinks);
				}
				num2++;
			}
		}
		currentChain.RemoveAt(currentChain.Count - 1);
		visitedUrns[dependency.Urn] = null;
	}

	private static void MarkFalseDependency(Dependency d1, bool[] b, bool isDataOnly)
	{
		int num = 0;
		string text = d1.Urn.Type.Trim();
		foreach (Dependency link in d1.Links)
		{
			if (isDataOnly)
			{
				if (!text.Equals("Table", StringComparison.OrdinalIgnoreCase))
				{
					b[num] = true;
				}
			}
			else if (text.Equals("Table", StringComparison.OrdinalIgnoreCase) && link.Urn.Type.Trim().Equals("Table", StringComparison.OrdinalIgnoreCase))
			{
				b[num] = true;
			}
			num++;
		}
	}

	public void TransferData()
	{
		TransferWriter transferWriter = null;
		try
		{
			transferWriter = GetScriptLoadedTransferWriter();
			UpdateWriter(transferWriter);
			SqlTransaction sqlTransaction = null;
			try
			{
				using SqlConnection sqlConnection = new SqlConnection(GetSourceConnectionString());
				sqlConnection.Open();
				using (SqlConnection sqlConnection2 = new SqlConnection(GetDestinationConnectionString()))
				{
					sqlConnection2.Open();
					ExecuteStatements(sqlConnection2, transferWriter.PreTransaction, null);
					try
					{
						if (base.UseDestinationTransaction)
						{
							sqlTransaction = sqlConnection2.BeginTransaction();
							DataTransferProgressEvent("BEGIN TRANSACTION");
						}
						ExecuteStatements(sqlConnection2, transferWriter.Prologue, sqlTransaction);
						SqlBulkCopyData(sqlConnection, sqlConnection2, transferWriter, sqlTransaction);
						ExecuteStatements(sqlConnection2, transferWriter.Epilogue, sqlTransaction);
					}
					catch (Exception)
					{
						if (base.UseDestinationTransaction && sqlTransaction != null)
						{
							sqlTransaction.Rollback();
							DataTransferProgressEvent("ROLLBACK TRANSACTION");
						}
						throw;
					}
					if (base.UseDestinationTransaction)
					{
						sqlTransaction.Commit();
						DataTransferProgressEvent("COMMIT TRANSACTION");
					}
					ExecuteStatements(sqlConnection2, transferWriter.PostTransaction, null);
					sqlConnection2.Close();
				}
				sqlConnection.Close();
			}
			catch (Exception innerException)
			{
				if (compensationScript != null && compensationScript.Count > 0)
				{
					using SqlConnection sqlConnection3 = new SqlConnection(GetDestinationConnectionString());
					sqlConnection3.Open();
					EnumerableContainer enumerableContainer = new EnumerableContainer();
					enumerableContainer.Add(compensationScript);
					ExecuteStatements(sqlConnection3, enumerableContainer, null);
					sqlConnection3.Close();
				}
				throw new TransferException(ExceptionTemplatesImpl.TransferDataException, innerException);
			}
		}
		finally
		{
			if (base.LogTransferDumps && transferWriter != null)
			{
				string fileName = string.Empty;
				using StreamWriter outfile = GetTempFile(GetTempDir(), "TransferDump{0}.sql", ref fileName, GetStreamWriter);
				using StreamWriter outfile2 = new StreamWriter(Console.OpenStandardOutput());
				DumpWriterContent("-- NON_TRANSACTABLE", transferWriter.PreTransaction, outfile, outfile2);
				DumpWriterContent("-- PROLOGUE SQL", transferWriter.Prologue, outfile, outfile2);
				DumpWriterContent("-- EPILOGUE SQL", transferWriter.Epilogue, outfile, outfile2);
				DumpWriterContent("-- POST_TRANSACTION SQL", transferWriter.PostTransaction, outfile, outfile2);
				if (compensationScript != null && compensationScript.Count > 0)
				{
					EnumerableContainer enumerableContainer2 = new EnumerableContainer();
					enumerableContainer2.Add(compensationScript);
					DumpWriterContent("-- COMPENSATION", enumerableContainer2, outfile, outfile2);
				}
			}
		}
	}

	private void UpdateWriter(TransferWriter writer)
	{
		if (writer.Tables.Count <= 0)
		{
			return;
		}
		writer.Prologue.Add("SET QUOTED_IDENTIFIER ON");
		writer.Prologue.Add("SET ANSI_NULLS ON");
		foreach (Urn table2 in writer.Tables)
		{
			SqlSmoObject smoObject = base.Database.Parent.GetSmoObject(table2);
			Table table = (Table)smoObject;
			if (table.IsSupportedProperty("IsFileTable") && table.GetPropValueOptional("IsFileTable", defaultValue: false))
			{
				string format = "ALTER TABLE {0} {1} FILETABLE_NAMESPACE";
				string text = table.FormatFullNameForScripting(base.Scripter.Options.GetScriptingPreferences());
				writer.Prologue.Add(string.Format(SmoApplication.DefaultCulture, format, new object[2]
				{
					text,
					Scripts.DISABLE
				}));
				writer.Epilogue.Add(string.Format(SmoApplication.DefaultCulture, format, new object[2]
				{
					text,
					Scripts.ENABLE
				}));
			}
		}
	}

	private void ExecuteStatements(SqlConnection destinationConnection, IEnumerable<string> statements, SqlTransaction transaction)
	{
		foreach (string statement in statements)
		{
			SqlCommand sqlCommand = new SqlCommand(statement, destinationConnection);
			sqlCommand.CommandTimeout = 120;
			SqlCommand sqlCommand2 = sqlCommand;
			if (base.UseDestinationTransaction && transaction != null)
			{
				sqlCommand2.Transaction = transaction;
			}
			sqlCommand2.ExecuteNonQuery();
			DataTransferProgressEvent(statement);
		}
	}

	private void SqlBulkCopyData(SqlConnection sourceConnection, SqlConnection destinationConnection, TransferWriter writer, SqlTransaction transaction)
	{
		ScriptingPreferences scriptingPreferences = base.Scripter.Options.GetScriptingPreferences();
		SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.FireTriggers;
		if (scriptingPreferences.Table.Identities)
		{
			sqlBulkCopyOptions |= SqlBulkCopyOptions.KeepIdentity;
		}
		foreach (Urn table2 in writer.Tables)
		{
			SqlSmoObject smoObject = base.Database.Parent.GetSmoObject(table2);
			Table table = (Table)smoObject;
			using SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(destinationConnection, sqlBulkCopyOptions, transaction);
			string text = string.Format(SmoApplication.DefaultCulture, "{0}.{1}", new object[2]
			{
				SqlSmoObject.MakeSqlBraket(table.Schema),
				SqlSmoObject.MakeSqlBraket(table.Name)
			});
			string text2 = table.FormatFullNameForScripting(base.Scripter.Options.GetScriptingPreferences());
			DataTransferInformationEvent(ExceptionTemplatesImpl.StartingDataTransfer(text2));
			string text3 = SetCoulmnNameAndMapping(table, sqlBulkCopy);
			string cmdText = ((!table.IsSupportedProperty("IsMemoryOptimized") || !table.IsMemoryOptimized) ? string.Format(SmoApplication.DefaultCulture, "SELECT {0} FROM {1}", new object[2] { text3, text }) : string.Format(SmoApplication.DefaultCulture, "SELECT {0} FROM {1} WITH (SNAPSHOT)", new object[2] { text3, text }));
			SqlCommand sqlCommand = new SqlCommand(cmdText, sourceConnection);
			using SqlDataReader reader = sqlCommand.ExecuteReader();
			sqlBulkCopy.DestinationTableName = text2;
			sqlBulkCopy.BulkCopyTimeout = BulkCopyTimeout;
			sqlBulkCopy.BatchSize = BatchSize;
			sqlBulkCopy.WriteToServer((IDataReader)reader);
			DataTransferInformationEvent(ExceptionTemplatesImpl.CompletedDataTransfer(text2));
		}
	}

	private string SetCoulmnNameAndMapping(Table table, SqlBulkCopy bulkCopy)
	{
		string text = string.Empty;
		bool flag = table.Columns.Cast<Column>().Any((Column col) => col.IsColumnSet);
		foreach (Column column in table.Columns)
		{
			if (!column.IsGraphInternalColumn() && !column.IsGraphComputedColumn() && !column.Computed && (!flag || !column.IsSparse))
			{
				string empty = string.Empty;
				text = string.Format(format: (column.DataType.SqlDataType != SqlDataType.UserDefinedType && column.DataType.SqlDataType != SqlDataType.HierarchyId && column.DataType.SqlDataType != SqlDataType.Geography && column.DataType.SqlDataType != SqlDataType.Geometry) ? "{0} {1}," : "{0} CAST({1} as varbinary(max)) AS {1},", provider: SmoApplication.DefaultCulture, args: new object[2]
				{
					text,
					SqlSmoObject.MakeSqlBraket(column.Name)
				});
				SqlBulkCopyColumnMapping bulkCopyColumnMapping = new SqlBulkCopyColumnMapping(column.Name, column.Name);
				bulkCopy.ColumnMappings.Add(bulkCopyColumnMapping);
			}
		}
		return text.Remove(text.Length - 1);
	}

	private string GetSourceConnectionString()
	{
		SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
		sqlConnectionStringBuilder.DataSource = base.Database.Parent.Name;
		if (!(sqlConnectionStringBuilder.IntegratedSecurity = base.Database.Parent.ConnectionContext.LoginSecure))
		{
			sqlConnectionStringBuilder.UserID = base.Database.Parent.ConnectionContext.Login;
			sqlConnectionStringBuilder.Password = base.Database.Parent.ConnectionContext.Password;
		}
		sqlConnectionStringBuilder.Pooling = false;
		sqlConnectionStringBuilder.InitialCatalog = base.Database.Name;
		return sqlConnectionStringBuilder.ConnectionString;
	}

	private string GetDestinationConnectionString()
	{
		SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
		sqlConnectionStringBuilder.DataSource = base.DestinationServer;
		sqlConnectionStringBuilder.IntegratedSecurity = base.DestinationLoginSecure;
		if (!base.DestinationLoginSecure)
		{
			sqlConnectionStringBuilder.UserID = base.DestinationLogin;
			sqlConnectionStringBuilder.Password = base.DestinationPassword;
		}
		sqlConnectionStringBuilder.Pooling = false;
		return sqlConnectionStringBuilder.ConnectionString;
	}

	public IDataTransferProvider GetTransferProvider()
	{
		throw new NotSupportedException();
	}

	private void OnDataTransferProgress(DataTransferEventType dataTransferEventType, string message)
	{
		if (this.DataTransferEvent != null)
		{
			this.DataTransferEvent(this, new DataTransferEventArgs(dataTransferEventType, message));
		}
	}

	private void DataTransferProgressEvent(string statement)
	{
		OnDataTransferProgress(DataTransferEventType.Progress, ExceptionTemplatesImpl.ExecutingScript(statement));
	}

	private void DataTransferInformationEvent(string message)
	{
		OnDataTransferProgress(DataTransferEventType.Information, message);
	}

	void ITransferMetadataProvider.SaveMetadata()
	{
		throw new NotSupportedException();
	}

	SortedList ITransferMetadataProvider.GetOptions()
	{
		throw new NotSupportedException();
	}

	private string GetTempDir()
	{
		string tempPath = Path.GetTempPath();
		if (string.IsNullOrEmpty(tempPath))
		{
			throw new SmoException(ExceptionTemplatesImpl.InexistentDir(tempPath));
		}
		string text = Path.Combine(tempPath, "\\Microsoft\\SQL Server\\Smo");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		if (!Directory.Exists(text))
		{
			throw new SmoException(ExceptionTemplatesImpl.InexistentDir(text));
		}
		return text;
	}

	private static StreamWriter GetStreamWriter(string fileName)
	{
		return new StreamWriter(new FileStream(fileName, FileMode.OpenOrCreate), Encoding.Unicode);
	}

	private static T GetTempFile<T>(string directory, string mask, ref string fileName, Func<string, T> createFile) where T : class
	{
		T val = null;
		int num = 0;
		fileName = string.Empty;
		do
		{
			fileName = Path.Combine(directory, string.Format(SmoApplication.DefaultCulture, mask, new object[1] { Guid.NewGuid().ToString("N", SmoApplication.DefaultCulture) }));
			if (File.Exists(fileName))
			{
				continue;
			}
			try
			{
				val = createFile(fileName);
			}
			catch (PathTooLongException)
			{
				throw;
			}
			catch (IOException innerException)
			{
				if (num++ < 10)
				{
					continue;
				}
				throw new SmoException(ExceptionTemplatesImpl.CantCreateTempFile(directory), innerException);
			}
		}
		while (val == null);
		return val;
	}

	private void DumpWriterContent(string label, IEnumerable<string> strings, StreamWriter outfile, StreamWriter outfile2)
	{
		if (label == null)
		{
			return;
		}
		outfile.WriteLine("-- =================================================");
		outfile2?.WriteLine("-- =================================================");
		outfile.WriteLine(label);
		outfile2?.WriteLine(label);
		outfile.WriteLine("");
		outfile2?.WriteLine("");
		if (strings == null)
		{
			outfile.WriteLine("-- *** empty ***");
			outfile2?.WriteLine("-- *** empty ***");
			return;
		}
		foreach (string @string in strings)
		{
			outfile.WriteLine(@string);
			outfile.WriteLine(Globals.Go);
			if (outfile2 != null)
			{
				outfile2.WriteLine(@string);
				outfile2.WriteLine(Globals.Go);
			}
		}
		outfile.WriteLine("");
		outfile2?.WriteLine("");
	}
}
