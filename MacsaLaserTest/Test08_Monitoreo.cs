using System;
using System.Threading;
using SocketCommNet;

namespace MacsaLaserTest
{
    /// <summary>
    /// PRUEBA 8: Monitoreo en Tiempo Real
    /// Prueba independiente para monitorear estado, contadores y temperatura continuamente
    /// </summary>
    class Test08_Monitoreo
    {
        public static void Run()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  PRUEBA 8: MONITOREO EN TIEMPO REAL");
            Console.WriteLine("  Monitoreo continuo de estado y contadores");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            // Configuración
            string nombreConexion = "MonitoreoTest";
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

                    // Configurar monitoreo
                    Console.Write("  Intervalo de actualización en segundos (default 2): ");
                    string intervaloStr = Console.ReadLine();
                    int intervalo = 2;
                    if (!int.TryParse(intervaloStr, out intervalo) || intervalo < 1)
                    {
                        intervalo = 2;
                    }

                    Console.Write("  Número de actualizaciones (default 10, 0 = infinito): ");
                    string numActualizacionesStr = Console.ReadLine();
                    int numActualizaciones = 10;
                    if (!int.TryParse(numActualizacionesStr, out numActualizaciones))
                    {
                        numActualizaciones = 10;
                    }

                    Console.WriteLine();
                    Console.WriteLine("Iniciando monitoreo...");
                    Console.WriteLine("Presiona cualquier tecla para detener");
                    Console.WriteLine();

                    uint contadorAnterior = 0;
                    uint contadorOKAnterior = 0;
                    int actualizacion = 0;
                    bool continuar = true;

                    // Thread para detectar tecla presionada
                    Thread threadTecla = new Thread(() =>
                    {
                        Console.ReadKey();
                        continuar = false;
                    });
                    threadTecla.IsBackground = true;
                    threadTecla.Start();

                    while (continuar && (numActualizaciones == 0 || actualizacion < numActualizaciones))
                    {
                        actualizacion++;

                        // Obtener estado
                        SocketComm.CSStatusExt estado = new SocketComm.CSStatusExt();
                        Int32 resultado = socketComm.CS_StatusExt(puntero, ref estado);

                        if (resultado == 0)
                        {
                            Console.WriteLine($"--- Actualización {actualizacion} - {DateTime.Now:HH:mm:ss} ---");
                            Console.WriteLine($"  Estado: {(estado.Start == 0 ? "Imprimiendo" : "Detenido")}");
                            Console.WriteLine($"  Mensaje: {estado.messagename}");
                            Console.WriteLine($"  Contador Total: {estado.t_counter:N0}");
                            Console.WriteLine($"  Contador OK: {estado.d_counter:N0} ({(estado.d_counter > contadorOKAnterior ? "+" : "")}{estado.d_counter - contadorOKAnterior})");
                            Console.WriteLine($"  Contador NOK: {estado.s_counter:N0}");
                            Console.WriteLine($"  Copias: {estado.m_copies}");

                            if (estado.err != 0)
                            {
                                Console.WriteLine($"  ALARMA: 0x{estado.err:X8}");
                            }

                            // Obtener temperatura
                            SocketComm.CSSysinfo sysinfo = new SocketComm.CSSysinfo();
                            resultado = socketComm.CS_Sysinfo(puntero, ref sysinfo);
                            if (resultado == 0)
                            {
                                Console.WriteLine($"  Temp. CPU: {sysinfo.cputemp / 1000.0:F2} °C");
                            }

                            contadorAnterior = estado.t_counter;
                            contadorOKAnterior = estado.d_counter;
                        }
                        else
                        {
                            Console.WriteLine($"Error al obtener estado en actualización {actualizacion}");
                        }

                        Console.WriteLine();

                        if (continuar && (numActualizaciones == 0 || actualizacion < numActualizaciones))
                        {
                            Thread.Sleep(intervalo * 1000);
                        }
                    }

                    Console.WriteLine("Monitoreo detenido");

                    Console.WriteLine("\nPrueba de monitoreo completada");
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

