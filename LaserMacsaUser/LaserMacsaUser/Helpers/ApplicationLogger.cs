using System;
using System.IO;
using System.Text;
using System.Threading;

namespace LaserMacsaUser.Helpers
{
    /// <summary>
    /// Logger global para toda la aplicación.
    /// Genera archivos de log en la carpeta Logs con el formato:
    /// IPAdmin-YYYY-MM-DD.log
    /// </summary>
    public static class ApplicationLogger
    {
        private static readonly object _lockObj = new object();
        private static string? _logsDirectory;

        /// <summary>
        /// Escribe un mensaje normal en el log (nivel INFO)
        /// </summary>
        public static void Log(string message)
        {
            WriteLog("INFO", message);
        }

        /// <summary>
        /// Escribe un mensaje de error en el log (nivel ERROR)
        /// </summary>
        public static void LogError(string message, Exception ex)
        {
            WriteLog("ERROR", $"{message}: {ex.Message}\n{ex.StackTrace}");
        }

        /// <summary>
        /// Maneja la escritura en el archivo de log 
        /// </summary>
        private static void WriteLog(string level, string message)
        {
            try
            {
                string logDirectory = GetLogsDirectory();
                string fileName = $"IPUSER-{DateTime.Now:yyyy-MM-dd}.log";
                string fullPath = Path.Combine(logDirectory, fileName);

                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

                // Uso de lock para que múltiples hilos no escriban al mismo tiempo
                lock (_lockObj)
                {
                    File.AppendAllText(fullPath, logEntry + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
                // Si falla, no lanzamos excepción. Evitamos romper la app.
            }
        }

        /// <summary>
        /// Obtiene o crea el directorio de logs junto al ejecutable
        /// </summary>
        private static string GetLogsDirectory()
        {
            if (_logsDirectory == null)
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                _logsDirectory = Path.Combine(appDirectory, "Logs");

                if (!Directory.Exists(_logsDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(_logsDirectory); // Intentar crear el directorio
                    }
                    catch
                    {
                        // Si falla por permisos, usar el directorio temporal del sistema
                        _logsDirectory = Path.GetTempPath();
                    }
                }
            }

            return _logsDirectory;
        }
    }
}
