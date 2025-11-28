using System;

namespace MacsaLaserTest
{
    /// <summary>
    /// Programa de prueba modular para obtener información de la impresora Macsa
    /// Cada módulo está separado para facilitar el debugging
    /// </summary>
    class ProgramInfoTest
    {
        public static void Run()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  PRUEBA DE INFORMACIÓN - MACSA LASER");
            Console.WriteLine("  Protocolo TCP/IP v3.1");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            // Configuración de conexión
            string nombreConexion = "InfoTest";
            string ipImpresora = TestConfigHelper.GetLaserIP();
            string rutaLocal = ".\\";                // Ruta local (puede ser cualquier ruta válida)

            Console.WriteLine("Configuración:");
            Console.WriteLine($"  IP Impresora: {ipImpresora}");
            Console.WriteLine($"  Ruta Local: {rutaLocal}");
            Console.WriteLine();

            // Crear conexión (módulo separado)
            LaserConnection conexion = null;
            LaserInfo info = null;

            try
            {
                // PASO 1: Crear y configurar conexión
                Console.WriteLine("PASO 1: Creando conexión...");
                conexion = new LaserConnection();

                // PASO 2: Inicializar
                Console.WriteLine("PASO 2: Inicializando...");
                if (!conexion.Inicializar(nombreConexion, ipImpresora, rutaLocal))
                {
                    Console.WriteLine($"  Error: {conexion.LastError}");
                    Console.WriteLine("\n[PASO 2 FALLÓ] Presiona cualquier tecla para salir...");
                    Console.ReadKey();
                    return;
                }
                Console.WriteLine("  Inicialización exitosa");

                // PASO 3: Conectar
                Console.WriteLine("PASO 3: Conectando...");
                if (!conexion.Conectar())
                {
                    Console.WriteLine($"  Error: {conexion.LastError}");
                    Console.WriteLine("\n[PASO 3 FALLÓ] Presiona cualquier tecla para salir...");
                    Console.ReadKey();
                    return;
                }
                Console.WriteLine("  Conexión establecida");

                // PASO 4: Verificar conexión
                Console.WriteLine("PASO 4: Verificando conexión...");
                if (!conexion.VerificarConexion())
                {
                    Console.WriteLine($"   Advertencia: La conexión no está activa");
                }
                else
                {
                    Console.WriteLine("   Conexión verificada");
                }
                Console.WriteLine();

                // PASO 5: Obtener información (módulo separado)
                Console.WriteLine("PASO 5: Obteniendo información de la impresora...");
                Console.WriteLine("  (Esto puede tardar unos segundos...)");
                Console.WriteLine();

                info = new LaserInfo(conexion);
                LaserInfoResult resultado = info.ObtenerTodaLaInformacion();

                // PASO 6: Mostrar resultados
                Console.WriteLine("PASO 6: Resultados:");
                resultado.MostrarInformacion();

                if (resultado.Exito)
                {
                    Console.WriteLine("Información obtenida exitosamente");
                }
                else
                {
                    Console.WriteLine($"Error al obtener información: {resultado.MensajeError}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR CRITICO: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            finally
            {
                // PASO 7: Cerrar conexión
                Console.WriteLine("\nPASO 7: Cerrando conexión...");
                if (conexion != null)
                {
                    conexion.Dispose();
                    Console.WriteLine("  Conexión cerrada");
                }
            }

        }

        static void Main(string[] args)
        {
            Run();
            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}

