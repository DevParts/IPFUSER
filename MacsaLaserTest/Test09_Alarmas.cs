using System;
using System.Threading;
using SocketCommNet;

namespace MacsaLaserTest
{
    /// <summary>
    /// PRUEBA 9: Manejo de Alarmas
    /// Prueba independiente para leer y analizar códigos de alarma
    /// </summary>
    class Test09_Alarmas
    {
        public static void Run()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  PRUEBA 9: MANEJO DE ALARMAS");
            Console.WriteLine("  Leer y analizar códigos de alarma");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            // Configuración
            string nombreConexion = "AlarmasTest";
            string ipImpresora = TestConfigHelper.GetLaserIP();
            string rutaLocal = ".\\";

            LaserConnection conexion = null;

            try
            {
                // Conectar
                Console.WriteLine("Conectando...");
                conexion = new LaserConnection();
                
                if (!conexion.Inicializar(nombreConexion, ipImpresora, rutaLocal))
                {
                    Console.WriteLine($"Error: {conexion.LastError}");
                    return;
                }

                if (!conexion.Conectar())
                {
                    Console.WriteLine($"Error: {conexion.LastError}");
                    return;
                }

                Console.WriteLine("Conexión establecida");
                Console.WriteLine();

                var socketComm = conexion.SocketComm;
                var puntero = conexion.Puntero;

                // PRUEBA 1: Leer estado extendido y alarmas
                Console.WriteLine("PRUEBA 1: Leer códigos de alarma del estado");
                SocketComm.CSStatusExt estado = new SocketComm.CSStatusExt();
                Int32 resultado = socketComm.CS_StatusExt(puntero, ref estado);
                
                if (resultado == 0)
                {
                    Console.WriteLine($"  Código de alarma (err): 0x{estado.err:X8} ({estado.err})");
                    Console.WriteLine($"  Alarm mask 1: 0x{estado.alarmmask1:X8}");
                    Console.WriteLine($"  Alarm mask 2: 0x{estado.alarmmask2:X8}");
                    Console.WriteLine($"  Signal state: 0x{estado.signalstate:X8}");

                    // Analizar código de alarma
                    if (estado.err == 0)
                    {
                        Console.WriteLine("  Estado: Sin alarmas activas");
                    }
                    else
                    {
                        Console.WriteLine("  Estado: ALARMA ACTIVA");
                        
                        // El código de alarma tiene estructura:
                        // Upper WORD: codifica la última alarma activa
                        // Lower WORD: 0 = sin alarma, != 0 = alarma activa
                        UInt16 upperWord = (UInt16)((estado.err >> 16) & 0xFFFF);
                        UInt16 lowerWord = (UInt16)(estado.err & 0xFFFF);
                        
                        Console.WriteLine($"    Upper WORD (última alarma): 0x{upperWord:X4} ({upperWord})");
                        Console.WriteLine($"    Lower WORD (estado): 0x{lowerWord:X4} ({lowerWord})");
                        
                        if (lowerWord != 0)
                        {
                            Console.WriteLine("    Alarma actualmente activa");
                        }
                        else
                        {
                            Console.WriteLine("    Sin alarma activa (solo registro de última alarma)");
                        }
                    }

                    // Analizar alarm masks
                    if (estado.alarmmask1 != 0 || estado.alarmmask2 != 0)
                    {
                        Console.WriteLine("\n  Alarm Masks (códigos de alarmas):");
                        Console.WriteLine($"    Mask 1: 0x{estado.alarmmask1:X8}");
                        Console.WriteLine($"    Mask 2: 0x{estado.alarmmask2:X8}");
                        Console.WriteLine("    (Consulta alarmcodes.pdf para interpretar los códigos)");
                    }
                }
                else
                {
                    string error = "";
                    socketComm.CS_GetLastError(puntero, ref error);
                    Console.WriteLine($"  Error al obtener estado: {error}");
                }
                Console.WriteLine();

                // PRUEBA 2: Monitoreo continuo de alarmas
                Console.WriteLine("PRUEBA 2: Monitoreo continuo de alarmas");
                Console.Write("  ¿Deseas monitorear alarmas cada 2 segundos? (s/n): ");
                string monitorear = Console.ReadLine();
                
                if (monitorear.ToLower() == "s")
                {
                    Console.WriteLine("  Monitoreando... (Presiona cualquier tecla para detener)");
                    Console.WriteLine();

                    bool continuar = true;
                    Thread threadTecla = new Thread(() =>
                    {
                        Console.ReadKey();
                        continuar = false;
                    });
                    threadTecla.IsBackground = true;
                    threadTecla.Start();

                    uint alarmaAnterior = 0;
                    int ciclo = 0;

                    while (continuar)
                    {
                        ciclo++;
                        resultado = socketComm.CS_StatusExt(puntero, ref estado);
                        
                        if (resultado == 0)
                        {
                            if (estado.err != alarmaAnterior)
                            {
                                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Cambio de alarma detectado:");
                                Console.WriteLine($"  Código anterior: 0x{alarmaAnterior:X8}");
                                Console.WriteLine($"  Código actual: 0x{estado.err:X8}");
                                
                                if (estado.err == 0)
                                {
                                    Console.WriteLine("  Estado: Sin alarmas");
                                }
                                else
                                {
                                    Console.WriteLine("  Estado: ALARMA DETECTADA");
                                }
                                Console.WriteLine();
                                
                                alarmaAnterior = estado.err;
                            }
                        }

                        Thread.Sleep(2000);
                    }

                    Console.WriteLine("Monitoreo detenido");
                }
                else
                {
                    Console.WriteLine("  Prueba omitida");
                }

                Console.WriteLine("\nPrueba de alarmas completada");
                Console.WriteLine("\nNota: Consulta el archivo alarmcodes.pdf para interpretar los códigos de alarma específicos");
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

