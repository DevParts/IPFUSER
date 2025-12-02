using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace DataBases.Access;

public class DBMS
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

	private OleDbConnection _Cn;

	private string _DbLocation;

	private string _DbName;

	private Dictionary<string, OleDbDataAdapter> _DataAdapters;

	private bool _PopUpErrors;

	private bool _DBFFiles;

	private string _OleDBProvider;

	public string DbLocation
	{
		get
		{
			return _DbLocation;
		}
		set
		{
			_DbLocation = value;
		}
	}

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

	public string OleDBProvider
	{
		get
		{
			return _OleDBProvider;
		}
		set
		{
			_OleDBProvider = value;
		}
	}

	public OleDbConnection GetConnection
	{
		get
		{
			string text = "Microsoft.Jet.OLEDB.4.0";
			if (Operators.CompareString(OleDBProvider, "", TextCompare: false) != 0)
			{
				text = OleDBProvider;
			}
			if (!_DBFFiles)
			{
				OleDbConnection oleDbConnection = new OleDbConnection();
				oleDbConnection.ConnectionString = "Provider=" + text + "; Data Source=" + _DbLocation + "\\" + _DbName + "; User Id=admin; Password=";
				return oleDbConnection;
			}
			OleDbConnection oleDbConnection2 = new OleDbConnection();
			oleDbConnection2.ConnectionString = "Provider=" + text + "; Data Source=" + _DbLocation + ";Extended Properties=dBASE IV; User Id=admin; Password=";
			return oleDbConnection2;
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

	public bool ExtendsToDBF
	{
		get
		{
			return _DBFFiles;
		}
		set
		{
			_DBFFiles = value;
		}
	}

	public DBMS()
	{
		_Cn = new OleDbConnection();
		_DataAdapters = new Dictionary<string, OleDbDataAdapter>();
	}

	public OleDbDataReader GetDataReader(string Sql)
	{
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = GetConnection;
		oleDbCommand.CommandText = Sql;
		oleDbCommand.Connection.Open();
		OleDbDataReader result;
		try
		{
			OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader(CommandBehavior.CloseConnection);
			result = oleDbDataReader;
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

	public int DoSQLAction(string Sql)
	{
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = GetConnection;
		oleDbCommand.CommandText = Sql;
		oleDbCommand.Connection.Open();
		int result;
		try
		{
			int num = oleDbCommand.ExecuteNonQuery();
			oleDbCommand.Connection.Close();
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
			oleDbCommand.Connection.Close();
			result = 0;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public DataTable GetTable(string TableName, ACCESS_TABLE_TYPE Mode)
	{
		DataTable dataTable = new DataTable();
		OleDbDataReader reader = ((Mode != ACCESS_TABLE_TYPE.ADD_ONLY) ? GetDataReader("Select * from " + TableName) : GetDataReader("Select top 1 * from " + TableName));
		dataTable.Load(reader);
		dataTable.TableName = TableName;
		if (Mode == ACCESS_TABLE_TYPE.READ_WRITE || Mode == ACCESS_TABLE_TYPE.ADD_ONLY)
		{
			try
			{
				OleDbDataAdapter oleDbDataAdapter = _DataAdapters[TableName];
			}
			catch (KeyNotFoundException ex)
			{
				ProjectData.SetProjectError(ex);
				KeyNotFoundException ex2 = ex;
				DataTable schemaTable = GetSchemaTable(TableName);
				OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter("Select * from " + TableName, _Cn.ConnectionString);
				oleDbDataAdapter.InsertCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_INSERT, schemaTable);
				oleDbDataAdapter.UpdateCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_UPDATE, schemaTable);
				oleDbDataAdapter.DeleteCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_DELETE, schemaTable);
				_DataAdapters.Add(TableName, oleDbDataAdapter);
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
			OleDbDataAdapter oleDbDataAdapter = _DataAdapters[Table.TableName];
			DataTable changes = Table.GetChanges();
			if (changes != null)
			{
				oleDbDataAdapter.Update(changes);
			}
			Table.AcceptChanges();
			result = Table;
		}
		catch (KeyNotFoundException ex)
		{
			ProjectData.SetProjectError(ex);
			KeyNotFoundException ex2 = ex;
			if (PopupErrors)
			{
				MessageBox.Show("[DB]Error Actualizando tabla: " + ex2.Message);
			}
			result = null;
			ProjectData.ClearProjectError();
		}
		catch (Exception ex3)
		{
			ProjectData.SetProjectError(ex3);
			Exception ex4 = ex3;
			if (PopupErrors)
			{
				MessageBox.Show("[DB]Error Actualizando tabla: " + ex4.Message);
			}
			result = null;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public DataTable GetSqlTable(string SqlText, string TableName)
	{
		OleDbDataReader reader = null;
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
				MessageBox.Show("[DB]Error: " + ex2.Message);
			}
			ProjectData.ClearProjectError();
		}
		dataTable.Load(reader);
		dataTable.TableName = TableName;
		return dataTable;
	}

	public OleDbDataAdapter CreateAdapter(string TableName)
	{
		DataTable dataTable = new DataTable();
		OleDbDataReader dataReader = GetDataReader("Select top 1 * from " + TableName);
		dataTable.Load(dataReader);
		dataTable.TableName = TableName;
		OleDbDataAdapter oleDbDataAdapter;
		try
		{
			oleDbDataAdapter = _DataAdapters[TableName];
		}
		catch (KeyNotFoundException ex)
		{
			ProjectData.SetProjectError(ex);
			KeyNotFoundException ex2 = ex;
			DataTable schemaTable = GetSchemaTable(TableName);
			oleDbDataAdapter = new OleDbDataAdapter("Select * from " + TableName, _Cn.ConnectionString);
			oleDbDataAdapter.InsertCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_INSERT, schemaTable);
			oleDbDataAdapter.UpdateCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_UPDATE, schemaTable);
			oleDbDataAdapter.DeleteCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_DELETE, schemaTable);
			_DataAdapters.Add(TableName, oleDbDataAdapter);
			ProjectData.ClearProjectError();
		}
		return oleDbDataAdapter;
	}

	public DataTable GetSchemaDB()
	{
		OleDbConnection getConnection = GetConnection;
		getConnection.Open();
		DataTable schema = getConnection.GetSchema("Tables", new string[4] { null, null, null, "Table" });
		getConnection.Close();
		return schema;
	}

	private DataTable GetSchemaTable(string TableName)
	{
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.Connection = GetConnection;
		oleDbCommand.CommandText = "Select * from " + TableName;
		oleDbCommand.Connection.Open();
		DataTable result;
		try
		{
			OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader(CommandBehavior.KeyInfo);
			DataTable schemaTable = oleDbDataReader.GetSchemaTable();
			oleDbCommand.Connection.Close();
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

	private OleDbCommand DoSqlCommand(DataTable Table, SQL_COMMAND_TYPE SqlType, DataTable StructTable)
	{
		OleDbCommand oleDbCommand = new OleDbCommand();
		oleDbCommand.CommandType = CommandType.Text;
		oleDbCommand.Connection = GetConnection;
		bool flag = true;
		checked
		{
			switch (unchecked((int)SqlType))
			{
			case 0:
				oleDbCommand.CommandText = "Select * from " + Table.TableName;
				break;
			case 1:
			{
				oleDbCommand.CommandText = "Insert into " + Table.TableName + "(";
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
							OleDbCommand oleDbCommand2 = oleDbCommand;
							oleDbCommand2.CommandText = oleDbCommand2.CommandText + "[" + Table.Columns[num2].ColumnName + "]";
						}
						else
						{
							OleDbCommand oleDbCommand2 = oleDbCommand;
							oleDbCommand2.CommandText = oleDbCommand2.CommandText + ",[" + Table.Columns[num2].ColumnName + "]";
						}
						OleDbParameter oleDbParameter2 = new OleDbParameter();
						oleDbParameter2.ParameterName = "p" + Table.Columns[num2].ColumnName;
						oleDbParameter2.SourceColumn = Table.Columns[num2].ColumnName;
						oleDbCommand.Parameters.Add(oleDbParameter2);
					}
					num2++;
				}
				oleDbCommand.CommandText += ")values(";
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
							OleDbCommand oleDbCommand2 = oleDbCommand;
							oleDbCommand2.CommandText = oleDbCommand2.CommandText + "[p" + Table.Columns[num2].ColumnName + "]";
						}
						else
						{
							OleDbCommand oleDbCommand2 = oleDbCommand;
							oleDbCommand2.CommandText = oleDbCommand2.CommandText + ",[p" + Table.Columns[num2].ColumnName + "]";
						}
					}
					num2++;
				}
				oleDbCommand.CommandText += ")";
				break;
			}
			case 2:
			{
				oleDbCommand.CommandText = "Update " + Table.TableName + " Set ";
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
							OleDbCommand oleDbCommand2 = oleDbCommand;
							oleDbCommand2.CommandText = oleDbCommand2.CommandText + "[" + Table.Columns[num2].ColumnName + "]=[p" + Table.Columns[num2].ColumnName + "]";
						}
						else
						{
							OleDbCommand oleDbCommand2 = oleDbCommand;
							oleDbCommand2.CommandText = oleDbCommand2.CommandText + ",[" + Table.Columns[num2].ColumnName + "]=[p" + Table.Columns[num2].ColumnName + "]";
						}
						OleDbParameter oleDbParameter3 = new OleDbParameter();
						oleDbParameter3.ParameterName = "p" + Table.Columns[num2].ColumnName;
						oleDbParameter3.SourceColumn = Table.Columns[num2].ColumnName;
						oleDbCommand.Parameters.Add(oleDbParameter3);
					}
					num2++;
				}
				oleDbCommand.CommandText += " where ";
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
							OleDbCommand oleDbCommand2 = oleDbCommand;
							oleDbCommand2.CommandText = oleDbCommand2.CommandText + Table.Columns[num2].ColumnName + "=[p" + Table.Columns[num2].ColumnName + "]";
						}
						else
						{
							OleDbCommand oleDbCommand2 = oleDbCommand;
							oleDbCommand2.CommandText = oleDbCommand2.CommandText + " and " + Table.Columns[num2].ColumnName + "=[p" + Table.Columns[num2].ColumnName + "]";
						}
						OleDbParameter oleDbParameter4 = new OleDbParameter();
						oleDbParameter4.ParameterName = "p" + Table.Columns[num2].ColumnName;
						oleDbParameter4.SourceColumn = Table.Columns[num2].ColumnName;
						oleDbCommand.Parameters.Add(oleDbParameter4);
					}
					num2++;
				}
				break;
			}
			case 3:
			{
				oleDbCommand.CommandText = "Delete * from " + Table.TableName + " where ";
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
							OleDbCommand oleDbCommand2 = oleDbCommand;
							oleDbCommand2.CommandText = oleDbCommand2.CommandText + "[" + Table.Columns[num2].ColumnName + "]=[p" + Table.Columns[num2].ColumnName + "]";
						}
						else
						{
							OleDbCommand oleDbCommand2 = oleDbCommand;
							oleDbCommand2.CommandText = oleDbCommand2.CommandText + " and [" + Table.Columns[num2].ColumnName + "]=[p" + Table.Columns[num2].ColumnName + "]";
						}
						OleDbParameter oleDbParameter = new OleDbParameter();
						oleDbParameter.ParameterName = "p" + Table.Columns[num2].ColumnName;
						oleDbParameter.SourceColumn = Table.Columns[num2].ColumnName;
						oleDbCommand.Parameters.Add(oleDbParameter);
					}
					num2++;
				}
				break;
			}
			}
			return oleDbCommand;
		}
	}
}
