using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Servicio de base de datos adaptado de SQLSeverDBMS para .NET 8
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private SqlConnection? _connection;
        private SqlConnection? _codesConnection;
        private string _dbName = string.Empty;
        private string _codesDbName = string.Empty;
        private string _dataSource = "(local)\\SQLEXPRESS";
        private string _user = string.Empty;
        private string _password = string.Empty;
        private bool _useWindowsAuthentication = true;
        private int _connectionTimeout = 30;

        private string GetConnectionString(string? databaseName = null)
        {
            if (string.IsNullOrEmpty(_dataSource))
            {
                _dataSource = "(local)\\SQLEXPRESS";
            }

            string catalog = databaseName ?? _dbName;
            if (string.IsNullOrEmpty(catalog))
            {
                catalog = "master";
            }

            // Agregar opciones SSL para evitar errores de certificados no confiables
            string sslOptions = "Encrypt=True;TrustServerCertificate=True";
            
            if (_useWindowsAuthentication)
            {
                return $"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog={catalog};Data Source={_dataSource};Connect Timeout={_connectionTimeout};{sslOptions}";
            }
            else if (!string.IsNullOrEmpty(_password))
            {
                return $"Data Source={_dataSource};Initial Catalog={catalog};User ID={_user};Password={_password};Connect Timeout={_connectionTimeout};{sslOptions}";
            }
            else
            {
                return $"Data Source={_dataSource};Initial Catalog={catalog};User ID={_user};Connect Timeout={_connectionTimeout};{sslOptions}";
            }
        }

        public bool Connect(string dataSource, string catalog, bool useWindowsAuth, string? user = null, string? password = null)
        {
            try
            {
                _dataSource = dataSource;
                _dbName = catalog;
                _useWindowsAuthentication = useWindowsAuth;
                _user = user ?? string.Empty;
                _password = password ?? string.Empty;

                // Cerrar conexión anterior si existe
                CloseConnection();

                // Probar conexión
                using var testConnection = new SqlConnection(GetConnectionString());
                testConnection.Open();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al conectar a la base de datos: {ex.Message}", ex);
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

        public void UpdateTable(DataTable table)
        {
            try
            {
                if (table == null || table.Rows.Count == 0) return;

                using var connection = new SqlConnection(GetConnectionString());
                connection.Open();

                string tableName = table.TableName;
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new Exception("El DataTable debe tener un TableName");
                }

                string selectSql = $"SELECT * FROM {tableName}";
                var adapter = new SqlDataAdapter(selectSql, connection);

                var builder = new SqlCommandBuilder(adapter);
                adapter.UpdateCommand = builder.GetUpdateCommand();
                adapter.InsertCommand = builder.GetInsertCommand();
                adapter.DeleteCommand = builder.GetDeleteCommand();

                adapter.Update(table);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar tabla: {ex.Message}", ex);
            }
        }

        public bool ConnectCodesDatabase(string codesDbName)
        {
            try
            {
                _codesDbName = codesDbName;

                // Cerrar conexión anterior si existe
                if (_codesConnection != null && _codesConnection.State == ConnectionState.Open)
                {
                    _codesConnection.Close();
                    _codesConnection.Dispose();
                }

                // Probar conexión
                _codesConnection = new SqlConnection(GetConnectionString(codesDbName));
                _codesConnection.Open();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al conectar a la base de datos de códigos: {ex.Message}", ex);
            }
        }

        public DataTable GetCodes(string sql)
        {
            try
            {
                if (string.IsNullOrEmpty(_codesDbName))
                {
                    throw new InvalidOperationException("No se ha conectado a ninguna base de datos de códigos. Use ConnectCodesDatabase primero.");
                }

                using var connection = new SqlConnection(GetConnectionString(_codesDbName));
                connection.Open();

                var adapter = new SqlDataAdapter(sql, connection);
                var dataTable = new DataTable("Codes");
                adapter.Fill(dataTable);

                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener códigos: {ex.Message}", ex);
            }
        }

        public void MarkCodesAsSent(DataTable codesTable)
        {
            try
            {
                if (codesTable == null || codesTable.Rows.Count == 0) return;

                if (string.IsNullOrEmpty(_codesDbName))
                {
                    throw new InvalidOperationException("No se ha conectado a ninguna base de datos de códigos.");
                }

                using var connection = new SqlConnection(GetConnectionString(_codesDbName));
                connection.Open();

                string selectSql = "SELECT * FROM Codes";
                var adapter = new SqlDataAdapter(selectSql, connection);

                var builder = new SqlCommandBuilder(adapter);
                adapter.UpdateCommand = builder.GetUpdateCommand();

                adapter.Update(codesTable);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al marcar códigos como enviados: {ex.Message}", ex);
            }
        }

        public bool AttachDatabase(string dbName, string mdfPath, string ldfPath)
        {
            try
            {
                if (!File.Exists(mdfPath))
                {
                    throw new FileNotFoundException($"El archivo .mdf no existe: {mdfPath}");
                }

                if (IsDatabaseAttached(dbName))
                {
                    System.Diagnostics.Debug.WriteLine($"Base de datos '{dbName}' ya está adjunta");
                    return true;
                }

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
                    attachSql = $@"
                        CREATE DATABASE [{dbName.Replace("'", "''")}]
                        ON (FILENAME = '{mdfPath.Replace("'", "''")}')
                        FOR ATTACH";
                }

                using var connection = new SqlConnection(GetConnectionString("master"));
                connection.Open();

                var command = new SqlCommand(attachSql, connection);
                command.CommandTimeout = 60;
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
                System.Diagnostics.Debug.WriteLine($"Error al desadjuntar base de datos: {ex.Message}");
                return false;
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

        public DataTable GetAttachedDatabases()
        {
            try
            {
                string sql = "SELECT name, database_id, create_date FROM sys.databases WHERE database_id > 4 ORDER BY name";

                using var connection = new SqlConnection(GetConnectionString("master"));
                connection.Open();

                var adapter = new SqlDataAdapter(sql, connection);
                var dataTable = new DataTable("Databases");
                adapter.Fill(dataTable);

                return dataTable;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener bases de datos adjuntas: {ex.Message}", ex);
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

            if (_codesConnection != null && _codesConnection.State == ConnectionState.Open)
            {
                _codesConnection.Close();
                _codesConnection.Dispose();
                _codesConnection = null;
            }
        }

        public void UpdateConsumos(int jobId, int idInicial, int idFinal)
        {
            try
            {
                if (idInicial <= 0 || idFinal <= 0 || idFinal < idInicial)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateConsumos: IDs inválidos - IdInicial: {idInicial}, IdFinal: {idFinal}");
                    return;
                }

                // Obtener todos los archivos del job desde CodesIndex
                string selectSql = $"SELECT * FROM CodesIndex WHERE IdJob = {jobId} ORDER BY FromRecord";
                DataTable codesIndexTable = GetDataTable(selectSql, "CodesIndex");

                if (codesIndexTable.Rows.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateConsumos: No se encontraron archivos para JobId: {jobId}");
                    return;
                }

                int idActual = idInicial;

                // Iterar sobre cada archivo y actualizar Consumed si corresponde
                // Basado en la lógica del código original (frmMain.UpdateConsumos)
                for (int i = 0; i < codesIndexTable.Rows.Count; i++)
                {
                    DataRow row = codesIndexTable.Rows[i];
                    int fromRecord = Convert.ToInt32(row["FromRecord"]);
                    int toRecord = Convert.ToInt32(row["ToRecord"]);

                    // Verificar si el rango de IDs consumidos se solapa con este archivo
                    // idActual debe estar dentro del rango del archivo
                    if (idActual >= fromRecord && idActual <= toRecord)
                    {
                        int consumedActual = Convert.ToInt32(row["Consumed"]);

                        if (idFinal <= toRecord)
                        {
                            // Todos los códigos consumidos están en este archivo
                            int incremento = idFinal - idActual + 1;
                            row["Consumed"] = consumedActual + incremento;
                            System.Diagnostics.Debug.WriteLine($"UpdateConsumos: Archivo '{row["FileName"]}' - Incremento: {incremento}, Consumed: {consumedActual} -> {consumedActual + incremento}");
                            break; // Ya procesamos todos los códigos
                        }
                        else
                        {
                            // Parte de los códigos están en este archivo, parte en el siguiente
                            int incremento = toRecord - idActual + 1;
                            row["Consumed"] = consumedActual + incremento;
                            System.Diagnostics.Debug.WriteLine($"UpdateConsumos: Archivo '{row["FileName"]}' - Incremento parcial: {incremento}, Consumed: {consumedActual} -> {consumedActual + incremento}");
                            
                            // Continuar con el siguiente archivo
                            if (i < codesIndexTable.Rows.Count - 1)
                            {
                                idActual = Convert.ToInt32(codesIndexTable.Rows[i + 1]["FromRecord"]);
                            }
                            else
                            {
                                break; // No hay más archivos
                            }
                        }
                    }
                }

                // Actualizar la base de datos
                UpdateTable(codesIndexTable);
                System.Diagnostics.Debug.WriteLine($"UpdateConsumos: Actualizados archivos en CodesIndex para JobId: {jobId}, IdInicial: {idInicial}, IdFinal: {idFinal}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en UpdateConsumos: {ex.Message}");
                throw new Exception($"Error al actualizar consumos en CodesIndex: {ex.Message}", ex);
            }
        }
    }
}

