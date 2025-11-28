using System;
using SocketCommNet;

namespace MacsaLaserTest
{
    /// <summary>
    /// PRUEBA 5: Gestionar Archivos en la Impresora
    /// Prueba independiente para listar, establecer por defecto y eliminar archivos
    /// </summary>
    class Test05_GestionarArchivos
    {
        public static void Run()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  PRUEBA 5: GESTIONAR ARCHIVOS");
            Console.WriteLine("  Listar, establecer por defecto, eliminar");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            // Configuración
            string nombreConexion = "GestionarArchivosTest";
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

                    // PRUEBA 1: Listar archivos .msf
                    Console.WriteLine("PRUEBA 1: Listar archivos .msf");
                    string filenames = "";
                    Int32 resultado = socketComm.CS_GetFilenames(puntero, "msf", 0, ref filenames);
                    if (resultado == 0 && !string.IsNullOrWhiteSpace(filenames))
                    {
                        string[] archivos = filenames.Split(new char[] { '\0', ' ', '\n', '\r' }, 
                            StringSplitOptions.RemoveEmptyEntries);
                        Console.WriteLine($"  Archivos encontrados ({archivos.Length}):");
                        for (int i = 0; i < archivos.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(archivos[i]))
                                Console.WriteLine($"    {i + 1}. {archivos[i]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  No se encontraron archivos .msf");
                    }
                    Console.WriteLine();

                    // PRUEBA 2: Establecer archivo por defecto
                    Console.WriteLine("PRUEBA 2: Establecer archivo por defecto");
                    Console.Write("  Ingresa el nombre del archivo (sin extensión) o Enter para omitir: ");
                    string nombreArchivo = Console.ReadLine();
                    
                    if (!string.IsNullOrWhiteSpace(nombreArchivo))
                    {
                        resultado = socketComm.CS_SetDefault(puntero, nombreArchivo);
                        if (resultado == 0)
                        {
                            Console.WriteLine($"  Archivo '{nombreArchivo}' establecido como por defecto");
                        }
                        else
                        {
                            string error = "";
                            socketComm.CS_GetLastError(puntero, ref error);
                            Console.WriteLine($"  Error: {error}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  Prueba omitida");
                    }
                    Console.WriteLine();

                    // PRUEBA 3: Verificar archivo actual
                    Console.WriteLine("PRUEBA 3: Verificar archivo actual");
                    SocketComm.CSStatusExt estado = new SocketComm.CSStatusExt();
                    resultado = socketComm.CS_StatusExt(puntero, ref estado);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Mensaje activo: {estado.messagename}");
                    }
                    Console.WriteLine();

                    // PRUEBA 4: Eliminar archivo
                    Console.WriteLine("PRUEBA 4: Eliminar archivo");
                    Console.Write("  Ingresa el nombre del archivo a eliminar (con extensión) o Enter para omitir: ");
                    string archivoEliminar = Console.ReadLine();
                    
                    if (!string.IsNullOrWhiteSpace(archivoEliminar))
                    {
                        Console.Write($"  ¿Estás seguro de eliminar '{archivoEliminar}'? (s/n): ");
                        string confirmacion = Console.ReadLine();
                        
                        if (confirmacion.ToLower() == "s")
                        {
                            resultado = socketComm.CS_Delete(puntero, archivoEliminar);
                            if (resultado == 0)
                            {
                                Console.WriteLine($"  Archivo '{archivoEliminar}' eliminado");
                            }
                            else
                            {
                                string error = "";
                                socketComm.CS_GetLastError(puntero, ref error);
                                Console.WriteLine($"  Error: {error}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("  Eliminación cancelada");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  Prueba omitida");
                    }

                    Console.WriteLine("\nPrueba de gestión de archivos completada");
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

