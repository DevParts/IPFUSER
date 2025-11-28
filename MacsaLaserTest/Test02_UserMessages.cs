using System;

namespace MacsaLaserTest
{
    /// <summary>
    /// PRUEBA 2: Envío de Mensajes de Usuario (User Messages)
    /// Prueba independiente para verificar el envío de texto dinámico a la impresora
    /// </summary>
    class Test02_UserMessages
    {
        public static void Run()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  PRUEBA 2: MENSAJES DE USUARIO");
            Console.WriteLine("  Envío de texto dinámico a la impresora");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            // Configuración
            string nombreConexion = "UserMessagesTest";
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

                    // Obtener objeto SocketComm
                    var socketComm = conexion.SocketComm;
                    var puntero = conexion.Puntero;

                    // PRUEBA 1: Enviar mensaje simple a campo 0
                    Console.WriteLine("PRUEBA 1: Enviar mensaje simple a campo 0");
                    string mensaje1 = "Hola desde C#";
                    Int32 resultado = socketComm.CS_FastASCIIUsermessage(puntero, 0, mensaje1);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Mensaje enviado: '{mensaje1}'");
                    }
                    else
                    {
                        string error = "";
                        socketComm.CS_GetLastError(puntero, ref error);
                        Console.WriteLine($"  Error: {error}");
                    }
                    Console.WriteLine();

                    // PRUEBA 2: Leer mensaje del campo 0
                    Console.WriteLine("PRUEBA 2: Leer mensaje del campo 0");
                    string mensajeLeido = "";
                    resultado = socketComm.CS_GetFastASCIIUsermessage(puntero, 0, ref mensajeLeido);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Mensaje leído: '{mensajeLeido}'");
                    }
                    else
                    {
                        string error = "";
                        socketComm.CS_GetLastError(puntero, ref error);
                        Console.WriteLine($"  Error: {error}");
                    }
                    Console.WriteLine();

                    // PRUEBA 3: Enviar múltiples mensajes a diferentes campos
                    Console.WriteLine("PRUEBA 3: Enviar mensajes a múltiples campos");
                    string[] mensajes = { "Campo 0", "Campo 1", "Campo 2" };
                    for (int i = 0; i < mensajes.Length; i++)
                    {
                        resultado = socketComm.CS_FastASCIIUsermessage(puntero, i, mensajes[i]);
                        if (resultado == 0)
                        {
                            Console.WriteLine($"  Campo {i}: '{mensajes[i]}' - Enviado");
                        }
                        else
                        {
                            Console.WriteLine($"  Campo {i}: Error al enviar");
                        }
                    }
                    Console.WriteLine();

                    // PRUEBA 4: Leer todos los campos enviados
                    Console.WriteLine("PRUEBA 4: Leer todos los campos");
                    for (int i = 0; i < mensajes.Length; i++)
                    {
                        string texto = "";
                        resultado = socketComm.CS_GetFastASCIIUsermessage(puntero, i, ref texto);
                        if (resultado == 0)
                        {
                            Console.WriteLine($"  Campo {i}: '{texto}'");
                        }
                    }
                    Console.WriteLine();

                    Console.WriteLine("Prueba de mensajes de usuario completada");
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

