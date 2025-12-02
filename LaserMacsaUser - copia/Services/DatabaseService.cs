using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Servicio de base de datos adaptado de SQLSeverDBMS para .NET 8
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private SqlConnection? _connection;
        private string _dbName = string.Empty;
        private string _dataSource = "(local)\\SQLEXPRESS";
        private string _user = string.Empty;
        private string _password = string.Empty;
        private bool _useWindowsAuthentication = true;
        private int _connectionTimeout = 30;
        
        public string DbName
        {
            get => _dbName;
            set => _dbName = value;
        }
        
        public string DataSource
        {
            get => _dataSource;
            set => _dataSource = value;
        }
        
        public string User
        {
            get => _user;
            set => _user = value;
        }
        
        public string Password
        {
            get => _password;
            set => _password = value;
        }
        
        public bool UseWindowsAuthentication
        {
            get => _useWindowsAuthentication;
            set => _useWindowsAuthentication = value;
        }
        
        public int ConnectionTimeout
        {
            get => _connectionTimeout;
            set => _connectionTimeout = value;
        }
        
        private string GetConnectionString(string? databaseName = null)
        {
            if (string.IsNullOrEmpty(_dataSource))
            {
                _dataSource = "(local)\\SQLEXPRESS";
            }
            
            string catalog = databaseName ?? _dbName;
            if (string.IsNullOrEmpty(catalog))
            {
                catalog = "master"; // Default to master if no database specified
            }
            
            if (_useWindowsAuthentication)
            {
                return $"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog={catalog};Data Source={_dataSource};Connect Timeout={_connectionTimeout}";
            }
            else if (!string.IsNullOrEmpty(_password))
            {
                return $"Data Source={_dataSource};Initial Catalog={catalog};User ID={_user};Password={_password};Connect Timeout={_connectionTimeout}";
            }
            else
            {
                return $"Data Source={_dataSource};Initial Catalog={catalog};User ID={_user};Connect Timeout={_connectionTimeout}";
            }
        }
        
        public SqlDataReader GetDataReader(string sql)
        {
            try
            {
                if (_connection == null || _connection.State != ConnectionState.Open)
                {
                    _connection = new SqlConnection(GetConnectionString());
                    _connection.Open();
                }
                
                var command = new SqlCommand(sql, _connection);
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar query: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Obtiene un DataReader conectándose a una base de datos específica
        /// </summary>
        public SqlDataReader GetDataReader(string sql, string databaseName)
        {
            try
            {
                using var connection = new SqlConnection(GetConnectionString(databaseName));
                connection.Open();
                var command = new SqlCommand(sql, connection);
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar query: {ex.Message}", ex);
            }
        }
        
        public DataTable GetDataTable(string sql, string tableName)
        {
            try
            {
                using var connection = new SqlConnection(GetConnectionString());
                connection.Open();
                
                var adapter = new SqlDataAdapter(sql, connection);
                var dataTable = new DataTable(tableName);
                adapter.Fill(dataTable);
                
                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener tabla: {ex.Message}", ex);
            }
        }
        
        public bool UpdateTable(DataTable table)
        {
            try
            {
                if (table == null || table.Rows.Count == 0) return true;
                
                using var connection = new SqlConnection(GetConnectionString());
                connection.Open();
                
                // Obtener el nombre de la tabla
                string tableName = table.TableName;
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new Exception("El DataTable debe tener un TableName");
                }
                
                // Crear adapter con SELECT original
                string selectSql = $"SELECT * FROM {tableName}";
                var adapter = new SqlDataAdapter(selectSql, connection);
                
                // Configurar comandos de actualización
                var builder = new SqlCommandBuilder(adapter);
                adapter.UpdateCommand = builder.GetUpdateCommand();
                adapter.InsertCommand = builder.GetInsertCommand();
                adapter.DeleteCommand = builder.GetDeleteCommand();
                
                // Actualizar
                adapter.Update(table);
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar tabla: {ex.Message}", ex);
            }
        }
        
        public bool AttachDatabase(string dbName, string mdfPath, string ldfPath)
        {
            try
            {
                // Verificar si el archivo existe
                if (!File.Exists(mdfPath))
                {
                    throw new FileNotFoundException($"El archivo .mdf no existe: {mdfPath}");
                }

                // Verificar si ya está adjunta
                if (IsDatabaseAttached(dbName))
                {
                    System.Diagnostics.Debug.WriteLine($"Base de datos '{dbName}' ya está adjunta");
                    return true;
                }

                // Construir comando ATTACH DATABASE usando CREATE DATABASE ... FOR ATTACH
                string attachSql;
                
                if (File.Exists(ldfPath))
                {
                    attachSql = $@"
                        CREATE DATABASE [{dbName.Replace("'", "''")}]
                        ON (FILENAME = '{mdfPath.Replace("'", "''")}')
                        LOG ON (FILENAME = '{ldfPath.Replace("'", "''")}')
                        FOR ATTACH";
                }
                else
                {
                    // Si no existe el .ldf, SQL Server lo recreará automáticamente
                    attachSql = $@"
                        CREATE DATABASE [{dbName.Replace("'", "''")}]
                        ON (FILENAME = '{mdfPath.Replace("'", "''")}')
                        FOR ATTACH";
                }
                
                using var connection = new SqlConnection(GetConnectionString("master"));
                connection.Open();
                
                var command = new SqlCommand(attachSql, connection);
                command.CommandTimeout = 60; // Aumentar timeout para operaciones de attach
                command.ExecuteNonQuery();
                
                System.Diagnostics.Debug.WriteLine($"Base de datos '{dbName}' adjuntada exitosamente desde: {mdfPath}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al adjuntar base de datos '{dbName}': {ex.Message}");
                throw new Exception($"Error al adjuntar base de datos: {ex.Message}", ex);
            }
        }
        
        public bool DetachDatabase(string dbName)
        {
            try
            {
                string detachSql = $@"
                    IF EXISTS (SELECT 1 FROM sys.databases WHERE name = '{dbName.Replace("'", "''")}')
                    BEGIN
                        ALTER DATABASE [{dbName.Replace("'", "''")}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        EXEC sp_detach_db @dbname = '{dbName.Replace("'", "''")}', @skipchecks = 'true';
                    END";
                
                using var connection = new SqlConnection(GetConnectionString("master"));
                connection.Open();
                
                var command = new SqlCommand(detachSql, connection);
                command.ExecuteNonQuery();
                
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desadjuntar base de datos: {ex.Message}", ex);
            }
        }
        
        public bool IsDatabaseAttached(string dbName)
        {
            try
            {
                string checkSql = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{dbName.Replace("'", "''")}'";
                
                using var connection = new SqlConnection(GetConnectionString("master"));
                connection.Open();
                
                var command = new SqlCommand(checkSql, connection);
                int count = Convert.ToInt32(command.ExecuteScalar());
                
                return count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verificando si base de datos está adjunta: {ex.Message}");
                return false;
            }
        }
        
        public void CloseConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}

