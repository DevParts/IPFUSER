using System;
using System.IO;

namespace MacsaLaserTest
{
    /// <summary>
    /// PRUEBA 4: Enviar Archivos a la Impresora
    /// Prueba independiente para copiar archivos .msf a la impresora
    /// </summary>
    class Test04_EnviarArchivos
    {
        public static void Run()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  PRUEBA 4: ENVIAR ARCHIVOS");
            Console.WriteLine("  Copiar archivos .msf a la impresora");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            // Configuración
            string nombreConexion = "EnviarArchivosTest";
            string ipImpresora = TestConfigHelper.GetLaserIP();
            string rutaLocal = ".\\";

            LaserConnection conexion = null;
            bool conexionExitosa = false;

            try
            {
                // Conectar
                Console.WriteLine("Conectando...");
                conexion = new LaserConnection();
                
                if (!conexion.Inicializar(nombreConexion, ipImpresora, rutaLocal))
                {
                    Console.WriteLine($"Error: {conexion.LastError}");
                }
                else if (!conexion.Conectar())
                {
                    Console.WriteLine($"Error: {conexion.LastError}");
                }
                else
                {
                    conexionExitosa = true;
                }

                if (conexionExitosa)
                {
                    Console.WriteLine("Conexión establecida");
                    Console.WriteLine();

                    var socketComm = conexion.SocketComm;
                    var puntero = conexion.Puntero;

                    // PRUEBA: Enviar archivo
                    Console.WriteLine("PRUEBA: Enviar archivo a la impresora");
                    Console.WriteLine("  Opciones de destino:");
                    Console.WriteLine("    0 = RAM Disk");
                    Console.WriteLine("    1 = Hard Disk");
                    Console.WriteLine();
                    Console.Write("  Ingresa la ruta completa del archivo .msf: ");
                    string rutaArchivo = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(rutaArchivo) || !File.Exists(rutaArchivo))
                    {
                        Console.WriteLine("  Archivo no encontrado. Prueba omitida.");
                    }
                    else
                    {
                        string nombreArchivo = Path.GetFileName(rutaArchivo);
                        string directorio = Path.GetDirectoryName(rutaArchivo);
                        
                        if (!directorio.EndsWith("\\"))
                            directorio += "\\";

                        Console.Write("  Destino (0=RAM, 1=HD): ");
                        string destinoStr = Console.ReadLine();
                        byte destino = 0;
                        if (!byte.TryParse(destinoStr, out destino))
                        {
                            destino = 0;
                        }

                        Console.WriteLine($"  Enviando '{nombreArchivo}' desde '{directorio}'...");
                        Int32 resultado = socketComm.CS_CopyFile(puntero, nombreArchivo, directorio, destino);
                        
                        if (resultado == 0)
                        {
                            Console.WriteLine($"  Archivo enviado exitosamente a {(destino == 0 ? "RAM Disk" : "Hard Disk")}");
                        }
                        else
                        {
                            string error = "";
                            socketComm.CS_GetLastError(puntero, ref error);
                            Console.WriteLine($"  Error: {error}");
                        }
                    }
                    Console.WriteLine();

                    // Verificar archivos en la impresora
                    Console.WriteLine("Verificando archivos en la impresora...");
                    string filenames = "";
                    Int32 resultado2 = socketComm.CS_GetFilenames(puntero, "msf", 0, ref filenames);
                    if (resultado2 == 0 && !string.IsNullOrWhiteSpace(filenames))
                    {
                        Console.WriteLine("  Archivos encontrados:");
                        string[] archivos = filenames.Split(new char[] { '\0', ' ', '\n', '\r' }, 
                            StringSplitOptions.RemoveEmptyEntries);
                        foreach (var archivo in archivos)
                        {
                            if (!string.IsNullOrWhiteSpace(archivo))
                                Console.WriteLine($"    - {archivo}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  No se encontraron archivos o error al listar");
                    }

                    Console.WriteLine("\nPrueba de envío de archivos completada");
                }
                else
                {
                    Console.WriteLine("\nNo se pudo establecer la conexión. Las pruebas no se ejecutaron.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
            finally
            {
                if (conexion != null)
                {
                    conexion.Dispose();
                    Console.WriteLine("Conexión cerrada");
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

