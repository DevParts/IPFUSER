using System;
using SocketCommNet;

namespace MacsaLaserTest
{
    /// <summary>
    /// PRUEBA 3: Control Básico de Impresión
    /// Prueba independiente para Start, Stop, Trigger, Reload
    /// </summary>
    class Test03_ControlImpresion
    {
        public static void Run()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  PRUEBA 3: CONTROL DE IMPRESIÓN");
            Console.WriteLine("  Start, Stop, Trigger, Reload");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            // Configuración
            string nombreConexion = "ControlTest";
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

                    // Obtener estado actual
                    Console.WriteLine("Estado actual de la impresora:");
                    SocketComm.CSStatusExt estado = new SocketComm.CSStatusExt();
                    Int32 resultado = socketComm.CS_StatusExt(puntero, ref estado);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Mensaje activo: {estado.messagename}");
                        Console.WriteLine($"  Estado: {(estado.Start == 0 ? "Imprimiendo" : "Detenido")}");
                    }
                    Console.WriteLine();

                    // PRUEBA 1: Detener impresión
                    Console.WriteLine("PRUEBA 1: Detener impresión");
                    resultado = socketComm.CS_Stop(puntero, 1000);  // timeout 1 segundo
                    if (resultado == 0)
                    {
                        Console.WriteLine("  Impresión detenida");
                    }
                    else
                    {
                        string error = "";
                        socketComm.CS_GetLastError(puntero, ref error);
                        Console.WriteLine($"  Error: {error}");
                    }
                    Console.WriteLine();

                    // PRUEBA 2: Recargar archivo actual
                    Console.WriteLine("PRUEBA 2: Recargar archivo actual");
                    resultado = socketComm.CS_Reload(puntero);
                    if (resultado == 0)
                    {
                        Console.WriteLine("  Archivo recargado");
                    }
                    else
                    {
                        string error = "";
                        socketComm.CS_GetLastError(puntero, ref error);
                        Console.WriteLine($"  Error: {error}");
                    }
                    Console.WriteLine();

                    // PRUEBA 3: Trigger de impresión (disparo software)
                    Console.WriteLine("PRUEBA 3: Trigger de impresión (disparo software)");
                    resultado = socketComm.CS_TriggerPrint(puntero);
                    if (resultado == 0)
                    {
                        Console.WriteLine("  Trigger enviado");
                    }
                    else
                    {
                        string error = "";
                        socketComm.CS_GetLastError(puntero, ref error);
                        Console.WriteLine($"  Error: {error}");
                    }
                    Console.WriteLine();

                    // PRUEBA 4: Iniciar impresión con archivo específico
                    Console.WriteLine("PRUEBA 4: Iniciar impresión");
                    Console.WriteLine("  (Nota: Necesitas un archivo .msf en la impresora)");
                    Console.Write("  Ingresa el nombre del archivo (sin extensión) o Enter para omitir: ");
                    string nombreArchivo = Console.ReadLine();
                    
                    if (!string.IsNullOrWhiteSpace(nombreArchivo))
                    {
                        // nr = 1 significa 1 impresión (test print)
                        resultado = socketComm.CS_Start(puntero, nombreArchivo, 1);
                        if (resultado == 0)
                        {
                            Console.WriteLine($"  Impresión iniciada con archivo: {nombreArchivo}");
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

                    // Verificar estado final
                    Console.WriteLine("Estado final de la impresora:");
                    resultado = socketComm.CS_StatusExt(puntero, ref estado);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Estado: {(estado.Start == 0 ? "Imprimiendo" : "Detenido")}");
                    }

                    Console.WriteLine("\nPrueba de control de impresión completada");
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

