using System;

namespace LaserMacsaUser.Exceptions
{
    public class DatabaseConnectionException : Exception
    {
        public string DataSource { get; }
        public string Database { get; }
        public string ErrorCode { get; }

        public DatabaseConnectionException(
            string dataSource, 
            string database, 
            string message, 
            Exception? innerException = null) 
            : base(FormatMessage(dataSource, database, message), innerException)
        {
            DataSource = dataSource;
            Database = database;
            ErrorCode = GenerateErrorCode();
        }

        private static string FormatMessage(string dataSource, string database, string message)
        {
            return $"Error de conexión a base de datos.\n" +
                   $"Servidor: {dataSource}\n" +
                   $"Base de datos: {database}\n" +
                   $"Detalle: {message}\n\n" +
                   $"Sugerencias:\n" +
                   $"1. Verificar que SQL Server esté ejecutándose\n" +
                   $"2. Verificar credenciales de acceso\n" +
                   $"3. Verificar que la base de datos exista\n" +
                   $"4. Verificar firewall y permisos de red";
        }

        private string GenerateErrorCode()
        {
            return $"DB_CONN_{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}