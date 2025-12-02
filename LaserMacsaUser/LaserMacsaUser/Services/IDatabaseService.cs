using System.Data;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Interfaz para el servicio de base de datos
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// Conecta a la base de datos principal (IPFEu)
        /// </summary>
        bool Connect(string dataSource, string catalog, bool useWindowsAuth, string? user = null, string? password = null);

        /// <summary>
        /// Obtiene una tabla de datos desde la base de datos principal
        /// </summary>
        DataTable GetDataTable(string sql, string tableName);

        /// <summary>
        /// Actualiza una tabla en la base de datos principal
        /// </summary>
        void UpdateTable(DataTable table);

        /// <summary>
        /// Conecta a la base de datos de códigos
        /// </summary>
        bool ConnectCodesDatabase(string codesDbName);

        /// <summary>
        /// Obtiene códigos de la base de datos de códigos
        /// </summary>
        DataTable GetCodes(string sql);

        /// <summary>
        /// Marca códigos como enviados
        /// </summary>
        void MarkCodesAsSent(DataTable codesTable);

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
        /// Cierra la conexión
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// Obtiene la lista de bases de datos adjuntas
        /// </summary>
        DataTable GetAttachedDatabases();

        /// <summary>
        /// Actualiza el campo Consumed en CodesIndex para los códigos consumidos
        /// </summary>
        /// <param name="jobId">ID del trabajo</param>
        /// <param name="idInicial">ID inicial de los códigos consumidos</param>
        /// <param name="idFinal">ID final de los códigos consumidos</param>
        void UpdateConsumos(int jobId, int idInicial, int idFinal);
    }
}

