using System;

namespace MacsaLaserTest
{
    /// <summary>
    /// PRUEBA 7: Configurar Parámetros de Impresión
    /// Prueba independiente para modo, offset, defocus, powerscale
    /// </summary>
    class Test07_Configuracion
    {
        public static void Run()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  PRUEBA 7: CONFIGURACIÓN");
            Console.WriteLine("  Modo, Offset, Defocus, Powerscale");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            // Configuración
            string nombreConexion = "ConfiguracionTest";
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

                    // PRUEBA 1: Leer/Establecer modo de impresión
                    Console.WriteLine("PRUEBA 1: Modo de impresión");
                    Console.WriteLine("  0 = Default, 1 = UMT, 2 = Solo leer, 4 = Batchjob");
                    UInt32 modo = 2;  // Solo leer
                    Int32 resultado = socketComm.CS_PrintMode(puntero, ref modo);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Modo actual: {modo} (0=Default, 1=UMT, 4=Batchjob)");
                    }
                    else
                    {
                        string error = "";
                        socketComm.CS_GetLastError(puntero, ref error);
                        Console.WriteLine($"  Error: {error}");
                    }
                    Console.WriteLine();

                    // PRUEBA 2: Leer/Establecer modo estático/dinámico
                    Console.WriteLine("PRUEBA 2: Modo estático/dinámico");
                    Console.WriteLine("  0 = Estático, 1 = Dinámico estándar, 2 = Dinámico distancia, 3 = Dinámico-estático, 8 = Solo leer");
                    Int32 modoDinamico = 8;  // Solo leer
                    resultado = socketComm.CS_Mode(puntero, ref modoDinamico);
                    if (resultado == 0)
                    {
                        string modoTexto = "";
                        switch (modoDinamico)
                        {
                            case 0: modoTexto = "Estático"; break;
                            case 1: modoTexto = "Dinámico estándar"; break;
                            case 2: modoTexto = "Dinámico distancia"; break;
                            case 3: modoTexto = "Dinámico-estático"; break;
                            default: modoTexto = $"Desconocido ({modoDinamico})"; break;
                        }
                        Console.WriteLine($"  Modo actual: {modoTexto}");
                    }
                    else
                    {
                        string error = "";
                        socketComm.CS_GetLastError(puntero, ref error);
                        Console.WriteLine($"  Error: {error}");
                    }
                    Console.WriteLine();

                    // PRUEBA 3: Establecer offset
                    Console.WriteLine("PRUEBA 3: Establecer offset");
                    Console.Write("  Offset X en micrones (o Enter para 0): ");
                    string offsetXStr = Console.ReadLine();
                    Int32 offsetX = 0;
                    if (!Int32.TryParse(offsetXStr, out offsetX))
                    {
                        offsetX = 0;
                    }

                    Console.Write("  Offset Y en micrones (o Enter para 0): ");
                    string offsetYStr = Console.ReadLine();
                    Int32 offsetY = 0;
                    if (!Int32.TryParse(offsetYStr, out offsetY))
                    {
                        offsetY = 0;
                    }

                    // format: 0 = ideal, 1 = micrones, 2 = 0.1mm
                    // relative: 0 = absoluto, 1 = relativo
                    // reset: 0 = no reset, 1 = reset después de imprimir
                    resultado = socketComm.CS_Offset(puntero, ref offsetX, ref offsetY, 0, 1, 0);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Offset establecido: X={offsetX} μm, Y={offsetY} μm");
                    }
                    else
                    {
                        string error = "";
                        socketComm.CS_GetLastError(puntero, ref error);
                        Console.WriteLine($"  Error: {error}");
                    }
                    Console.WriteLine();

                    // PRUEBA 4: Establecer defocus (Z)
                    Console.WriteLine("PRUEBA 4: Establecer defocus (Z)");
                    Console.Write("  Defocus Z en micrones (o Enter para 0): ");
                    string defocusStr = Console.ReadLine();
                    Int32 defocusZ = 0;
                    if (!Int32.TryParse(defocusStr, out defocusZ))
                    {
                        defocusZ = 0;
                    }

                    resultado = socketComm.CS_Defocus(puntero, ref defocusZ, 0, 1);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Defocus establecido: Z={defocusZ} μm");
                    }
                    else
                    {
                        string error = "";
                        socketComm.CS_GetLastError(puntero, ref error);
                        Console.WriteLine($"  Error: {error}");
                    }
                    Console.WriteLine();

                    // PRUEBA 5: Leer/Establecer powerscale
                    Console.WriteLine("PRUEBA 5: Powerscale");
                    Console.WriteLine("  Member: 0-3 (diferentes escalas)");
                    Console.Write("  Member (0-3) o Enter para 0: ");
                    string memberStr = Console.ReadLine();
                    Int32 member = 0;
                    if (!Int32.TryParse(memberStr, out member))
                    {
                        member = 0;
                    }

                    // Primero leer
                    Int32 value = 0;
                    resultado = socketComm.CS_Powerscale(puntero, 0, member, ref value);  // set=0 para leer
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Powerscale member {member} actual: {value}");
                    }

                    Console.Write("  ¿Deseas establecer un nuevo valor? (s/n): ");
                    string establecer = Console.ReadLine();
                    if (establecer.ToLower() == "s")
                    {
                        Console.Write("  Nuevo valor (o Enter para mantener): ");
                        string nuevoValorStr = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(nuevoValorStr))
                        {
                            Int32 nuevoValor = 0;
                            if (Int32.TryParse(nuevoValorStr, out nuevoValor))
                            {
                                value = nuevoValor;
                                resultado = socketComm.CS_Powerscale(puntero, 1, member, ref value);  // set=1 para escribir
                                if (resultado == 0)
                                {
                                    Console.WriteLine($"  Powerscale member {member} establecido a: {value}");
                                }
                                else
                                {
                                    string error = "";
                                    socketComm.CS_GetLastError(puntero, ref error);
                                    Console.WriteLine($"  Error: {error}");
                                }
                            }
                        }
                    }

                    Console.WriteLine("\nPrueba de configuración completada");
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

