using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace DataBases.SqlServer;

public class SQLSeverDBMS
{
	public enum ACCESS_TABLE_TYPE
	{
		READ_ONLY = 1,
		ADD_ONLY = 2,
		READ_WRITE = 4
	}

	private enum SQL_COMMAND_TYPE
	{
		SQL_SELECT,
		SQL_INSERT,
		SQL_UPDATE,
		SQL_DELETE
	}

	public enum DB_ACCESS_MODES
	{
		READ_WRITE,
		READ_ONLY
	}

	private SqlConnection _Cn;

	private string _DbName;

	private string _DataSource;

	private string _User;

	private string _Password;

	private Dictionary<string, SqlDataAdapter> _DataAdapters;

	private bool _PopUpErrors;

	private bool _UseWindowsAuthentication;

	private int _ConnetionTimeOut;

	public string DbName
	{
		get
		{
			return _DbName;
		}
		set
		{
			_DbName = value;
		}
	}

	public string DataSource
	{
		get
		{
			return _DataSource;
		}
		set
		{
			_DataSource = value;
		}
	}

	public string User
	{
		get
		{
			return _User;
		}
		set
		{
			_User = value;
		}
	}

	public string Password
	{
		get
		{
			return _Password;
		}
		set
		{
			_Password = value;
		}
	}

	public int ConnectionTimeOut
	{
		get
		{
			return _ConnetionTimeOut;
		}
		set
		{
			_ConnetionTimeOut = value;
		}
	}

	public SqlConnection GetConnection
	{
		get
		{
			SqlConnection sqlConnection = new SqlConnection();
			if (Operators.CompareString(_DataSource, "", TextCompare: false) == 0)
			{
				_DataSource = "(local)\\SQLEXPRESS";
			}
			if (_UseWindowsAuthentication)
			{
				sqlConnection.ConnectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=" + _DbName + ";Data Source=" + DataSource;
			}
			else if (Operators.CompareString(_Password, "", TextCompare: false) != 0)
			{
				sqlConnection.ConnectionString = "Data Source=" + DataSource + ";Initial Catalog=" + _DbName + ";User ID=" + _User + ";Password=" + Password;
			}
			else
			{
				sqlConnection.ConnectionString = "Data Source=" + DataSource + ";Initial Catalog=" + _DbName + ";User ID=" + _User;
			}
			SqlConnection sqlConnection2 = sqlConnection;
			sqlConnection2.ConnectionString = sqlConnection2.ConnectionString + ";Connect Timeout=" + ConnectionTimeOut;
			return sqlConnection;
		}
	}

	public bool PopupErrors
	{
		get
		{
			return _PopUpErrors;
		}
		set
		{
			_PopUpErrors = value;
		}
	}

	public bool UseWindowsAuthentication
	{
		get
		{
			return _UseWindowsAuthentication;
		}
		set
		{
			_UseWindowsAuthentication = value;
		}
	}

	public SQLSeverDBMS()
	{
		_Cn = new SqlConnection();
		_DataAdapters = new Dictionary<string, SqlDataAdapter>();
		_ConnetionTimeOut = 15;
	}

	public void BulkTable(DataTable SrcTable, string TableName)
	{
		SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(GetConnection.ConnectionString, SqlBulkCopyOptions.TableLock);
		sqlBulkCopy.DestinationTableName = TableName;
		sqlBulkCopy.WriteToServer(SrcTable);
	}

	public SqlDataReader GetDataReader(string Sql)
	{
		SqlCommand sqlCommand = new SqlCommand();
		sqlCommand.Connection = GetConnection;
		sqlCommand.CommandText = Sql;
		sqlCommand.Connection.Open();
		SqlDataReader result;
		try
		{
			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
			result = sqlDataReader;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			if (PopupErrors)
			{
				MessageBox.Show("[DB]Error en la ejecución del comando SQL: \r\n" + Sql + " " + ex2.Message);
			}
			result = null;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public int DoSQLAction(string Sql, int TimeOut = 30)
	{
		SqlCommand sqlCommand = new SqlCommand();
		sqlCommand.Connection = GetConnection;
		sqlCommand.CommandText = Sql;
		sqlCommand.CommandTimeout = TimeOut;
		sqlCommand.Connection.Open();
		int result;
		try
		{
			int num = sqlCommand.ExecuteNonQuery();
			sqlCommand.Connection.Close();
			result = num;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			if (PopupErrors)
			{
				MessageBox.Show("[DB]Error en la ejecución del comando SQL: \r\n" + Sql + " " + ex2.Message);
			}
			sqlCommand.Connection.Close();
			result = 0;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public DataTable GetTable(string TableName, ACCESS_TABLE_TYPE Mode)
	{
		DataTable dataTable = new DataTable();
		SqlDataReader reader = ((Mode != ACCESS_TABLE_TYPE.ADD_ONLY) ? GetDataReader("Select * from " + TableName) : GetDataReader("Select top 1 * from " + TableName));
		dataTable.Load(reader);
		dataTable.TableName = TableName;
		if (Mode == ACCESS_TABLE_TYPE.READ_WRITE || Mode == ACCESS_TABLE_TYPE.ADD_ONLY)
		{
			try
			{
				SqlDataAdapter sqlDataAdapter = _DataAdapters[TableName];
			}
			catch (KeyNotFoundException ex)
			{
				ProjectData.SetProjectError(ex);
				KeyNotFoundException ex2 = ex;
				DataTable schemaTable = GetSchemaTable(TableName);
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("Select * from " + TableName, _Cn.ConnectionString);
				sqlDataAdapter.InsertCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_INSERT, schemaTable);
				sqlDataAdapter.UpdateCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_UPDATE, schemaTable);
				sqlDataAdapter.DeleteCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_DELETE, schemaTable);
				_DataAdapters.Add(TableName, sqlDataAdapter);
				ProjectData.ClearProjectError();
			}
		}
		return dataTable;
	}

	public DataTable UpdateTable(DataTable Table)
	{
		DataTable result;
		try
		{
			SqlDataAdapter sqlDataAdapter = _DataAdapters[Table.TableName];
			DataTable changes = Table.GetChanges();
			if (changes != null)
			{
				sqlDataAdapter.Update(changes);
			}
			Table.AcceptChanges();
			result = Table;
		}
		catch (KeyNotFoundException ex)
		{
			ProjectData.SetProjectError(ex);
			KeyNotFoundException ex2 = ex;
			result = null;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public DataTable GetSqlTable(string SqlText, string TableName)
	{
		SqlDataReader reader = null;
		DataTable dataTable = new DataTable();
		try
		{
			reader = GetDataReader(SqlText);
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			if (PopupErrors)
			{
				MessageBox.Show("[DB]Error en la ejecución del comando SQL: " + SqlText + " \r\n" + ex2.Message);
			}
			ProjectData.ClearProjectError();
		}
		dataTable.Load(reader);
		dataTable.TableName = TableName;
		return dataTable;
	}

	public SqlDataAdapter CreateAdapter(string TableName)
	{
		DataTable dataTable = new DataTable();
		SqlDataReader dataReader = GetDataReader("Select top 1 * from " + TableName);
		dataTable.Load(dataReader);
		dataTable.TableName = TableName;
		dataReader.Close();
		SqlDataAdapter sqlDataAdapter;
		try
		{
			sqlDataAdapter = _DataAdapters[TableName];
		}
		catch (KeyNotFoundException ex)
		{
			ProjectData.SetProjectError(ex);
			KeyNotFoundException ex2 = ex;
			DataTable schemaTable = GetSchemaTable(TableName);
			sqlDataAdapter = new SqlDataAdapter("Select * from " + TableName, _Cn.ConnectionString);
			sqlDataAdapter.InsertCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_INSERT, schemaTable);
			sqlDataAdapter.UpdateCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_UPDATE, schemaTable);
			sqlDataAdapter.DeleteCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_DELETE, schemaTable);
			_DataAdapters.Add(TableName, sqlDataAdapter);
			ProjectData.ClearProjectError();
		}
		return sqlDataAdapter;
	}

	public bool AttachDB(string DbName, string FileData, string FileLog)
	{
		SqlCommand sqlCommand = new SqlCommand();
		string dbName = _DbName;
		_DbName = "Master";
		sqlCommand.Connection = GetConnection;
		sqlCommand.CommandText = "EXEC sp_attach_db @dbname = '" + DbName + "', @filename1 = '" + FileData + "', @filename2 = '" + FileLog + "'";
		sqlCommand.CommandType = CommandType.Text;
		sqlCommand.Connection.Open();
		bool result;
		try
		{
			int num = sqlCommand.ExecuteNonQuery();
			sqlCommand.Connection.Close();
			_DbName = dbName;
			result = true;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			if (PopupErrors)
			{
				MessageBox.Show("[DB]Error enlazando la base de datos " + ex2.Message);
			}
			sqlCommand.Connection.Close();
			_DbName = dbName;
			result = false;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public bool DetachDB(string DbName)
	{
		SqlCommand sqlCommand = new SqlCommand();
		string dbName = _DbName;
		_DbName = "Master";
		sqlCommand.Connection = GetConnection;
		sqlCommand.CommandText = "ALTER DATABASE [" + DbName + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
		sqlCommand.Connection.Open();
		bool result;
		try
		{
			int num = sqlCommand.ExecuteNonQuery();
			sqlCommand.CommandText = "sp_detach_db '" + DbName + "';";
			num = sqlCommand.ExecuteNonQuery();
			sqlCommand.Connection.Close();
			_DbName = dbName;
			result = true;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			if (PopupErrors)
			{
				MessageBox.Show("[DB]Error separando la base de datos " + ex2.Message);
			}
			sqlCommand.Connection.Close();
			_DbName = dbName;
			result = false;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public void CloseConnection()
	{
		foreach (KeyValuePair<string, SqlDataAdapter> dataAdapter in _DataAdapters)
		{
			dataAdapter.Value.Dispose();
		}
	}

	public DataTable GetListAttachedDb()
	{
		string dbName = _DbName;
		_DbName = "Master";
		SqlDataReader reader = null;
		DataTable dataTable = new DataTable();
		try
		{
			reader = GetDataReader("Select name from sysdatabases");
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			if (PopupErrors)
			{
				MessageBox.Show("[DB]Error en la ejecución del comando SQL: Select name from sysdatabases \r\n" + ex2.Message);
			}
			ProjectData.ClearProjectError();
		}
		finally
		{
			_DbName = dbName;
		}
		dataTable.Load(reader);
		dataTable.TableName = "DataBases";
		return dataTable;
	}

	public void ShrinkDb(string DbName, int TimeOut)
	{
		DoSQLAction("DBCC SHRINKDATABASE (" + DbName + ")", TimeOut);
	}

	public void ReIndexTable(string TableName, int TimeOut)
	{
		DoSQLAction("DBCC DBREINDEX (" + TableName + ", '', 0)", TimeOut);
	}

	public void SetAccess(DB_ACCESS_MODES Mode)
	{
		if (Mode == DB_ACCESS_MODES.READ_ONLY)
		{
			DoSQLAction("ALTER DATABASE [" + DbName + "] SET READ_ONLY");
		}
		else
		{
			DoSQLAction("ALTER DATABASE [" + DbName + "] SET READ_WRITE");
		}
	}

	private DataTable GetSchemaTable(string TableName)
	{
		SqlCommand sqlCommand = new SqlCommand();
		sqlCommand.Connection = GetConnection;
		sqlCommand.CommandText = "Select top 1 * from " + TableName;
		sqlCommand.Connection.Open();
		DataTable result;
		try
		{
			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.KeyInfo);
			DataTable schemaTable = sqlDataReader.GetSchemaTable();
			sqlCommand.Connection.Close();
			result = schemaTable;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			result = null;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	private SqlCommand DoSqlCommand(DataTable Table, SQL_COMMAND_TYPE SqlType, DataTable StructTable)
	{
		SqlCommand sqlCommand = new SqlCommand();
		sqlCommand.CommandType = CommandType.Text;
		sqlCommand.Connection = GetConnection;
		bool flag = true;
		checked
		{
			switch (unchecked((int)SqlType))
			{
			case 0:
				sqlCommand.CommandText = "Select * from " + Table.TableName;
				break;
			case 1:
			{
				sqlCommand.CommandText = "Insert into " + Table.TableName + "(";
				int num5 = Table.Columns.Count - 1;
				int num2 = 0;
				while (true)
				{
					int num6 = num2;
					int num4 = num5;
					if (num6 > num4)
					{
						break;
					}
					if (!Conversions.ToBoolean(StructTable.Rows[num2]["IsAutoIncrement"]))
					{
						if (flag)
						{
							flag = false;
							sqlCommand.CommandText += Table.Columns[num2].ColumnName;
						}
						else
						{
							SqlCommand sqlCommand2 = sqlCommand;
							sqlCommand2.CommandText = sqlCommand2.CommandText + "," + Table.Columns[num2].ColumnName;
						}
						SqlParameter sqlParameter2 = new SqlParameter();
						sqlParameter2.ParameterName = "@" + Table.Columns[num2].ColumnName;
						sqlParameter2.SourceColumn = Table.Columns[num2].ColumnName;
						sqlCommand.Parameters.Add(sqlParameter2);
					}
					num2++;
				}
				sqlCommand.CommandText += ")values(";
				flag = true;
				int num7 = Table.Columns.Count - 1;
				num2 = 0;
				while (true)
				{
					int num8 = num2;
					int num4 = num7;
					if (num8 > num4)
					{
						break;
					}
					if (!Conversions.ToBoolean(StructTable.Rows[num2]["IsAutoIncrement"]))
					{
						if (flag)
						{
							flag = false;
							SqlCommand sqlCommand2 = sqlCommand;
							sqlCommand2.CommandText = sqlCommand2.CommandText + "@" + Table.Columns[num2].ColumnName;
						}
						else
						{
							SqlCommand sqlCommand2 = sqlCommand;
							sqlCommand2.CommandText = sqlCommand2.CommandText + ",@" + Table.Columns[num2].ColumnName;
						}
					}
					num2++;
				}
				sqlCommand.CommandText += ")";
				break;
			}
			case 2:
			{
				sqlCommand.CommandText = "Update " + Table.TableName + " Set ";
				int num9 = Table.Columns.Count - 1;
				int num2 = 0;
				while (true)
				{
					int num10 = num2;
					int num4 = num9;
					if (num10 > num4)
					{
						break;
					}
					if (!Conversions.ToBoolean(StructTable.Rows[num2]["Iskey"]) & !Conversions.ToBoolean(StructTable.Rows[num2]["IsAutoIncrement"]))
					{
						if (flag)
						{
							flag = false;
							SqlCommand sqlCommand2 = sqlCommand;
							sqlCommand2.CommandText = sqlCommand2.CommandText + Table.Columns[num2].ColumnName + "=@" + Table.Columns[num2].ColumnName;
						}
						else
						{
							SqlCommand sqlCommand2 = sqlCommand;
							sqlCommand2.CommandText = sqlCommand2.CommandText + "," + Table.Columns[num2].ColumnName + "=@" + Table.Columns[num2].ColumnName;
						}
						SqlParameter sqlParameter3 = new SqlParameter();
						sqlParameter3.ParameterName = "@" + Table.Columns[num2].ColumnName;
						sqlParameter3.SourceColumn = Table.Columns[num2].ColumnName;
						sqlCommand.Parameters.Add(sqlParameter3);
					}
					num2++;
				}
				sqlCommand.CommandText += " where ";
				flag = true;
				int num11 = Table.Columns.Count - 1;
				num2 = 0;
				while (true)
				{
					int num12 = num2;
					int num4 = num11;
					if (num12 > num4)
					{
						break;
					}
					if (Conversions.ToBoolean(StructTable.Rows[num2]["IsKey"]))
					{
						if (flag)
						{
							flag = false;
							SqlCommand sqlCommand2 = sqlCommand;
							sqlCommand2.CommandText = sqlCommand2.CommandText + Table.Columns[num2].ColumnName + "=@" + Table.Columns[num2].ColumnName;
						}
						else
						{
							SqlCommand sqlCommand2 = sqlCommand;
							sqlCommand2.CommandText = sqlCommand2.CommandText + " and " + Table.Columns[num2].ColumnName + "=@" + Table.Columns[num2].ColumnName;
						}
						SqlParameter sqlParameter4 = new SqlParameter();
						sqlParameter4.ParameterName = "@" + Table.Columns[num2].ColumnName;
						sqlParameter4.SourceColumn = Table.Columns[num2].ColumnName;
						sqlCommand.Parameters.Add(sqlParameter4);
					}
					num2++;
				}
				break;
			}
			case 3:
			{
				sqlCommand.CommandText = "Delete from " + Table.TableName + " where ";
				int num = Table.Columns.Count - 1;
				int num2 = 0;
				while (true)
				{
					int num3 = num2;
					int num4 = num;
					if (num3 > num4)
					{
						break;
					}
					if (Conversions.ToBoolean(StructTable.Rows[num2]["IsKey"]))
					{
						if (flag)
						{
							flag = false;
							SqlCommand sqlCommand2 = sqlCommand;
							sqlCommand2.CommandText = sqlCommand2.CommandText + Table.Columns[num2].ColumnName + "=@" + Table.Columns[num2].ColumnName;
						}
						else
						{
							SqlCommand sqlCommand2 = sqlCommand;
							sqlCommand2.CommandText = sqlCommand2.CommandText + " and " + Table.Columns[num2].ColumnName + "=@" + Table.Columns[num2].ColumnName;
						}
						SqlParameter sqlParameter = new SqlParameter();
						sqlParameter.ParameterName = "@" + Table.Columns[num2].ColumnName;
						sqlParameter.SourceColumn = Table.Columns[num2].ColumnName;
						sqlCommand.Parameters.Add(sqlParameter);
					}
					num2++;
				}
				break;
			}
			}
			return sqlCommand;
		}
	}
}
