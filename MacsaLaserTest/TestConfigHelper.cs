using System;
using System.Configuration;
using System.IO;
using System.Xml;

namespace MacsaLaserTest
{
    /// <summary>
    /// Helper para leer la configuración de IP desde LaserMacsaUser
    /// </summary>
    public static class TestConfigHelper
    {
        private static string _cachedIp = null;
        private static readonly object _lock = new object();

        /// <summary>
        /// Obtiene la IP configurada desde LaserMacsaUser
        /// Busca primero en user.config (donde se guardan los settings del usuario)
        /// Si no se encuentra, busca en App.config (valores por defecto)
        /// </summary>
        public static string GetLaserIP()
        {
            lock (_lock)
            {
                if (_cachedIp != null)
                    return _cachedIp;

                try
                {
                    // 1. Buscar en user.config (donde se guardan los settings del usuario)
                    string userConfigPath = FindUserConfigFile();
                    if (!string.IsNullOrEmpty(userConfigPath) && File.Exists(userConfigPath))
                    {
                        string ip = ReadIPFromConfig(userConfigPath);
                        if (!string.IsNullOrEmpty(ip))
                        {
                            _cachedIp = ip;
                            return _cachedIp;
                        }
                    }

                    // 2. Buscar en App.config (valores por defecto)
                    string[] posiblesRutas = new string[]
                    {
                        // Desde bin/Debug del proyecto actual
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "LaserMacsaUser", "bin", "Debug", "net8.0-windows", "LaserMacsaUser.dll.config"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "LaserMacsaUser", "bin", "Release", "net8.0-windows", "LaserMacsaUser.dll.config"),
                        // Desde la raíz del proyecto
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "LaserMacsaUser", "App.config"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "LaserMacsaUser", "App.config"),
                        // Rutas absolutas desde la raíz del workspace
                        Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "", "..", "..", "..", "..", "LaserMacsaUser", "App.config"),
                        // Desde el directorio actual
                        Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "LaserMacsaUser", "App.config"),
                    };

                    foreach (string ruta in posiblesRutas)
                    {
                        try
                        {
                            string rutaNormalizada = Path.GetFullPath(ruta);
                            if (File.Exists(rutaNormalizada))
                            {
                                string ip = ReadIPFromConfig(rutaNormalizada);
                                if (!string.IsNullOrEmpty(ip))
                                {
                                    _cachedIp = ip;
                                    return _cachedIp;
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al leer configuración: {ex.Message}");
                }

                // Valor por defecto si no se encuentra
                _cachedIp = "192.168.0.180";
                return _cachedIp;
            }
        }

        /// <summary>
        /// Busca el archivo user.config donde se guardan los settings del usuario
        /// </summary>
        private static string FindUserConfigFile()
        {
            try
            {
                // En .NET, los UserScopedSettings se guardan en:
                // %LOCALAPPDATA%\[CompanyName]\[ApplicationName]_[hash]\[version]\user.config
                // O en %APPDATA% en algunos casos
                
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                
                // Buscar en LocalAppData
                string[] searchPaths = new string[]
                {
                    Path.Combine(localAppData, "LaserMacsaUser"),
                    Path.Combine(appData, "LaserMacsaUser"),
                };
                
                foreach (string basePath in searchPaths)
                {
                    if (Directory.Exists(basePath))
                    {
                        // Buscar recursivamente archivos user.config
                        string[] userConfigs = Directory.GetFiles(basePath, "user.config", SearchOption.AllDirectories);
                        foreach (string configFile in userConfigs)
                        {
                            // Verificar que el archivo contiene LaserMacsaUser.Properties.Settings
                            try
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.Load(configFile);
                                XmlNode node = doc.SelectSingleNode("//userSettings/LaserMacsaUser.Properties.Settings");
                                if (node != null)
                                {
                                    return configFile;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignorar errores
            }
            
            return string.Empty;
        }

        /// <summary>
        /// Lee la IP desde un archivo de configuración XML
        /// </summary>
        private static string ReadIPFromConfig(string configPath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configPath);

                // Buscar el nodo Laser_IP en diferentes formatos posibles
                XmlNode node = doc.SelectSingleNode(
                    "//userSettings/LaserMacsaUser.Properties.Settings/setting[@name='Laser_IP']/value"
                );

                if (node == null)
                {
                    // Intentar otro formato
                    node = doc.SelectSingleNode("//setting[@name='Laser_IP']/value");
                }

                if (node != null && !string.IsNullOrWhiteSpace(node.InnerText))
                {
                    return node.InnerText.Trim();
                }
            }
            catch
            {
                // Ignorar errores
            }
            
            return string.Empty;
        }

        /// <summary>
        /// Limpia la caché de IP (útil si se cambió la configuración)
        /// </summary>
        public static void ClearCache()
        {
            lock (_lock)
            {
                _cachedIp = null;
            }
        }
    }
}

