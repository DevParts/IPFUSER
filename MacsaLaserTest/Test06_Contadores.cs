using System;
using SocketCommNet;

namespace MacsaLaserTest
{
    /// <summary>
    /// PRUEBA 6: Trabajar con Contadores
    /// Prueba independiente para leer y establecer contadores globales y privados
    /// </summary>
    class Test06_Contadores
    {
        public static void Run()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  PRUEBA 6: CONTADORES");
            Console.WriteLine("  Leer y establecer contadores");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            // Configuración
            string nombreConexion = "ContadoresTest";
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

                    // PRUEBA 1: Leer contadores del estado
                    Console.WriteLine("PRUEBA 1: Leer contadores del estado");
                    SocketComm.CSStatusExt estado = new SocketComm.CSStatusExt();
                    Int32 resultado = socketComm.CS_StatusExt(puntero, ref estado);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Contador Total: {estado.t_counter:N0}");
                        Console.WriteLine($"  Contador OK: {estado.d_counter:N0}");
                        Console.WriteLine($"  Contador NOK: {estado.s_counter:N0}");
                    }
                    Console.WriteLine();

                    // PRUEBA 2: Leer contador global
                    Console.WriteLine("PRUEBA 2: Leer contador global");
                    Console.Write("  Ingresa el número de campo (0-255) o Enter para campo 0: ");
                    string campoStr = Console.ReadLine();
                    int campo = 0;
                    if (!int.TryParse(campoStr, out campo))
                    {
                        campo = 0;
                    }

                    string contadorGlobal = "";
                    resultado = socketComm.CS_GetGlobalCounter(puntero, campo, ref contadorGlobal);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Contador global campo {campo}: '{contadorGlobal}'");
                    }
                    else
                    {
                        string error = "";
                        socketComm.CS_GetLastError(puntero, ref error);
                        Console.WriteLine($"  Error: {error}");
                    }
                    Console.WriteLine();

                    // PRUEBA 3: Establecer contador global
                    Console.WriteLine("PRUEBA 3: Establecer contador global");
                    Console.Write("  Ingresa el valor del contador (o Enter para omitir): ");
                    string valorContador = Console.ReadLine();
                    
                    if (!string.IsNullOrWhiteSpace(valorContador))
                    {
                        resultado = socketComm.CS_SetGlobalCounter(puntero, campo, valorContador);
                        if (resultado == 0)
                        {
                            Console.WriteLine($"  Contador global campo {campo} establecido a: '{valorContador}'");
                            
                            // Verificar que se estableció
                            string contadorVerificado = "";
                            socketComm.CS_GetGlobalCounter(puntero, campo, ref contadorVerificado);
                            Console.WriteLine($"  Valor verificado: '{contadorVerificado}'");
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

                    // PRUEBA 4: Establecer contador privado
                    Console.WriteLine("PRUEBA 4: Establecer contador privado");
                    Console.Write("  Ingresa el número de campo (0-255) o Enter para campo 0: ");
                    string campoPrivadoStr = Console.ReadLine();
                    int campoPrivado = 0;
                    if (!int.TryParse(campoPrivadoStr, out campoPrivado))
                    {
                        campoPrivado = 0;
                    }

                    Console.Write("  Ingresa número de repeticiones (o Enter para 1): ");
                    string repeticionesStr = Console.ReadLine();
                    int repeticiones = 1;
                    if (!int.TryParse(repeticionesStr, out repeticiones))
                    {
                        repeticiones = 1;
                    }

                    Console.Write("  Ingresa número de impresiones (o Enter para 1): ");
                    string impresionesStr = Console.ReadLine();
                    int impresiones = 1;
                    if (!int.TryParse(impresionesStr, out impresiones))
                    {
                        impresiones = 1;
                    }

                    resultado = socketComm.CS_SetPrivateCounter(puntero, campoPrivado, repeticiones, impresiones);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Contador privado campo {campoPrivado} establecido:");
                        Console.WriteLine($"    Repeticiones: {repeticiones}");
                        Console.WriteLine($"    Impresiones: {impresiones}");
                    }
                    else
                    {
                        string error = "";
                        socketComm.CS_GetLastError(puntero, ref error);
                        Console.WriteLine($"  Error: {error}");
                    }
                    Console.WriteLine();

                    // PRUEBA 5: Resetear contadores
                    Console.WriteLine("PRUEBA 5: Resetear contadores (d_counter y s_counter)");
                    Console.Write("  ¿Deseas resetear los contadores? (s/n): ");
                    string resetear = Console.ReadLine();
                    
                    if (resetear.ToLower() == "s")
                    {
                        resultado = socketComm.CS_CounterReset(puntero);
                        if (resultado == 0)
                        {
                            Console.WriteLine("  Contadores reseteados");
                            
                            // Verificar
                            socketComm.CS_StatusExt(puntero, ref estado);
                            Console.WriteLine($"  Contador OK después del reset: {estado.d_counter}");
                            Console.WriteLine($"  Contador NOK después del reset: {estado.s_counter}");
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
                        Console.WriteLine("  Reset omitido");
                    }

                    Console.WriteLine("\nPrueba de contadores completada");
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

