using System.Data;
using System.Data.SqlClient;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Interfaz para el servicio de base de datos
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// Obtiene un DataReader para consultas
        /// </summary>
        SqlDataReader GetDataReader(string sql);
        
        /// <summary>
        /// Obtiene una DataTable para consultas
        /// </summary>
        DataTable GetDataTable(string sql, string tableName);
        
        /// <summary>
        /// Actualiza una tabla usando DataAdapter
        /// </summary>
        bool UpdateTable(DataTable table);
        
        /// <summary>
        /// Adjunta una base de datos
        /// </summary>
        bool AttachDatabase(string dbName, string mdfPath, string ldfPath);
        
        /// <summary>
        /// Desadjunta una base de datos
        /// </summary>
        bool DetachDatabase(string dbName);
        
        /// <summary>
        /// Verifica si una base de datos está adjunta
        /// </summary>
        bool IsDatabaseAttached(string dbName);
        
        /// <summary>
        /// Cierra la conexión a la base de datos
        /// </summary>
        void CloseConnection();
    }
}

