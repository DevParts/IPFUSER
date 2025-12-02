using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace DataBases.Sybase;

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

	private OdbcConnection _Cn;

	private string _DbLocation;

	private string _DbName;

	private Dictionary<string, OdbcDataAdapter> _DataAdapters;

	private bool _PopUpErrors;

	public string ODBCName
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

	public OdbcConnection GetConnection
	{
		get
		{
			if (_Cn == null || _Cn.State == ConnectionState.Closed)
			{
				OdbcConnection odbcConnection = new OdbcConnection();
				odbcConnection.ConnectionString = "DSN=" + _DbName;
				_Cn = odbcConnection;
				_Cn.Open();
			}
			return _Cn;
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

	public DBMS()
	{
		_DataAdapters = new Dictionary<string, OdbcDataAdapter>();
	}

	public OdbcDataReader GetDataReader(string Sql)
	{
		OdbcCommand odbcCommand = new OdbcCommand();
		odbcCommand.Connection = GetConnection;
		odbcCommand.CommandText = Sql;
		OdbcDataReader result;
		try
		{
			OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader(CommandBehavior.Default);
			result = odbcDataReader;
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
		OdbcCommand odbcCommand = new OdbcCommand();
		odbcCommand.Connection = GetConnection;
		odbcCommand.CommandText = Sql;
		int result;
		try
		{
			int num = odbcCommand.ExecuteNonQuery();
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
			result = 0;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public DataTable GetTable(string TableName, ACCESS_TABLE_TYPE Mode)
	{
		DataTable dataTable = new DataTable();
		OdbcDataReader reader = ((Mode != ACCESS_TABLE_TYPE.ADD_ONLY) ? GetDataReader("Select * from " + TableName) : GetDataReader("Select top 1 * from " + TableName));
		dataTable.Load(reader);
		dataTable.TableName = TableName;
		if (Mode == ACCESS_TABLE_TYPE.READ_WRITE || Mode == ACCESS_TABLE_TYPE.ADD_ONLY)
		{
			try
			{
				OdbcDataAdapter odbcDataAdapter = _DataAdapters[TableName];
			}
			catch (KeyNotFoundException ex)
			{
				ProjectData.SetProjectError(ex);
				KeyNotFoundException ex2 = ex;
				DataTable schemaTable = GetSchemaTable(TableName);
				OdbcDataAdapter odbcDataAdapter = new OdbcDataAdapter("Select * from " + TableName, _Cn);
				odbcDataAdapter.InsertCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_INSERT, schemaTable);
				odbcDataAdapter.UpdateCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_UPDATE, schemaTable);
				odbcDataAdapter.DeleteCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_DELETE, schemaTable);
				_DataAdapters.Add(TableName, odbcDataAdapter);
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
			OdbcDataAdapter odbcDataAdapter = _DataAdapters[Table.TableName];
			DataTable changes = Table.GetChanges();
			if (changes != null)
			{
				odbcDataAdapter.Update(changes);
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
		OdbcDataReader reader = null;
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

	public OdbcDataAdapter CreateAdapter(string TableName)
	{
		DataTable dataTable = new DataTable();
		OdbcDataReader dataReader = GetDataReader("Select top 1 * from " + TableName);
		dataTable.Load(dataReader);
		dataTable.TableName = TableName;
		OdbcDataAdapter odbcDataAdapter;
		try
		{
			odbcDataAdapter = _DataAdapters[TableName];
		}
		catch (KeyNotFoundException ex)
		{
			ProjectData.SetProjectError(ex);
			KeyNotFoundException ex2 = ex;
			DataTable schemaTable = GetSchemaTable(TableName);
			odbcDataAdapter = new OdbcDataAdapter("Select * from " + TableName, _Cn);
			odbcDataAdapter.InsertCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_INSERT, schemaTable);
			odbcDataAdapter.UpdateCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_UPDATE, schemaTable);
			odbcDataAdapter.DeleteCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_DELETE, schemaTable);
			_DataAdapters.Add(TableName, odbcDataAdapter);
			ProjectData.ClearProjectError();
		}
		return odbcDataAdapter;
	}

	public DataTable GetSchemaDB()
	{
		OdbcConnection getConnection = GetConnection;
		return getConnection.GetSchema("Tables", new string[4] { null, null, null, "Table" });
	}

	public void CloseConnection()
	{
		if ((_Cn != null && _Cn.State != ConnectionState.Closed) ? true : false)
		{
			_Cn.Close();
		}
	}

	private DataTable GetSchemaTable(string TableName)
	{
		OdbcCommand odbcCommand = new OdbcCommand();
		odbcCommand.Connection = GetConnection;
		odbcCommand.CommandText = "Select * from " + TableName;
		DataTable result;
		try
		{
			OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader(CommandBehavior.KeyInfo);
			DataTable schemaTable = odbcDataReader.GetSchemaTable();
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

	private OdbcCommand DoSqlCommand(DataTable Table, SQL_COMMAND_TYPE SqlType, DataTable StructTable)
	{
		OdbcCommand odbcCommand = new OdbcCommand();
		odbcCommand.CommandType = CommandType.Text;
		odbcCommand.Connection = GetConnection;
		bool flag = true;
		checked
		{
			switch (unchecked((int)SqlType))
			{
			case 0:
				odbcCommand.CommandText = "Select * from " + Table.TableName;
				break;
			case 1:
			{
				odbcCommand.CommandText = "Insert into " + Table.TableName + "(";
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
							odbcCommand.CommandText += Table.Columns[num2].ColumnName;
						}
						else
						{
							OdbcCommand odbcCommand2 = odbcCommand;
							odbcCommand2.CommandText = odbcCommand2.CommandText + "," + Table.Columns[num2].ColumnName;
						}
						OdbcParameter odbcParameter2 = new OdbcParameter();
						odbcParameter2.ParameterName = Table.Columns[num2].ColumnName;
						odbcParameter2.SourceColumn = Table.Columns[num2].ColumnName;
						odbcCommand.Parameters.Add(odbcParameter2);
					}
					num2++;
				}
				odbcCommand.CommandText += ")values(";
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
							odbcCommand.CommandText += Table.Columns[num2].ColumnName;
						}
						else
						{
							odbcCommand.CommandText += Table.Columns[num2].ColumnName;
						}
					}
					num2++;
				}
				odbcCommand.CommandText += ")";
				break;
			}
			case 2:
			{
				odbcCommand.CommandText = "Update " + Table.TableName + " Set ";
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
							OdbcCommand odbcCommand2 = odbcCommand;
							odbcCommand2.CommandText = odbcCommand2.CommandText + Table.Columns[num2].ColumnName + "=" + Table.Columns[num2].ColumnName;
						}
						else
						{
							OdbcCommand odbcCommand2 = odbcCommand;
							odbcCommand2.CommandText = odbcCommand2.CommandText + "," + Table.Columns[num2].ColumnName + "=" + Table.Columns[num2].ColumnName;
						}
						OdbcParameter odbcParameter3 = new OdbcParameter();
						odbcParameter3.ParameterName = Table.Columns[num2].ColumnName;
						odbcParameter3.SourceColumn = Table.Columns[num2].ColumnName;
						odbcCommand.Parameters.Add(odbcParameter3);
					}
					num2++;
				}
				odbcCommand.CommandText += " where ";
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
							OdbcCommand odbcCommand2 = odbcCommand;
							odbcCommand2.CommandText = odbcCommand2.CommandText + Table.Columns[num2].ColumnName + "=" + Table.Columns[num2].ColumnName;
						}
						else
						{
							OdbcCommand odbcCommand2 = odbcCommand;
							odbcCommand2.CommandText = odbcCommand2.CommandText + " and " + Table.Columns[num2].ColumnName + "=" + Table.Columns[num2].ColumnName;
						}
						OdbcParameter odbcParameter4 = new OdbcParameter();
						odbcParameter4.ParameterName = Table.Columns[num2].ColumnName;
						odbcParameter4.SourceColumn = Table.Columns[num2].ColumnName;
						odbcCommand.Parameters.Add(odbcParameter4);
					}
					num2++;
				}
				break;
			}
			case 3:
			{
				odbcCommand.CommandText = "Delete from " + Table.TableName + " where ";
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
							OdbcCommand odbcCommand2 = odbcCommand;
							odbcCommand2.CommandText = odbcCommand2.CommandText + Table.Columns[num2].ColumnName + "=" + Table.Columns[num2].ColumnName;
						}
						else
						{
							OdbcCommand odbcCommand2 = odbcCommand;
							odbcCommand2.CommandText = odbcCommand2.CommandText + " and " + Table.Columns[num2].ColumnName + "=" + Table.Columns[num2].ColumnName;
						}
						OdbcParameter odbcParameter = new OdbcParameter();
						odbcParameter.ParameterName = Table.Columns[num2].ColumnName;
						odbcParameter.SourceColumn = Table.Columns[num2].ColumnName;
						odbcCommand.Parameters.Add(odbcParameter);
					}
					num2++;
				}
				break;
			}
			}
			return odbcCommand;
		}
	}
}
