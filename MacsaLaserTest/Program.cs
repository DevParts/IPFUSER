using System;
using SocketCommNet;

namespace MacsaLaserTest
{
    /// <summary>
    /// Proyecto básico para conectar con impresoras laser Macsa usando TCP/IP
    /// Desarrollado paso a paso para entender el protocolo
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("  Prueba de Conexión Macsa Laser");
            Console.WriteLine("  Protocolo TCP/IP v3.1");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            // PASO 1: Configuración básica de conexión
            // Cambia estos valores según tu configuración
            string nombreConexion = "TestConnection";
            string ipImpresora = "192.168.16.180";  // Cambia por la IP de tu impresora
            string rutaLocal = "C:\\Fly";            // Ruta local (puede ser cualquier ruta)

            Console.WriteLine("PASO 1: Configuración de conexión");
            Console.WriteLine($"  Nombre: {nombreConexion}");
            Console.WriteLine($"  IP Impresora: {ipImpresora}");
            Console.WriteLine($"  Ruta Local: {rutaLocal}");
            Console.WriteLine();

            // Crear instancia del objeto de comunicación
            SocketComm socketComm = new SocketComm();
            Int32 puntero = 0;  // Puntero que identifica la conexión
            Int32 resultado = 0;
            string mensajeError = "";

            try
            {
                // PASO 2: Inicializar la conexión
                Console.WriteLine("PASO 2: Inicializando conexión...");
                socketComm.CS_Init(ref puntero, nombreConexion, ipImpresora, rutaLocal);
                
                // Verificar si hubo errores en la inicialización
                resultado = socketComm.CS_GetLastError(puntero, ref mensajeError);
                if (resultado != 0)
                {
                    Console.WriteLine($"  Advertencia: {mensajeError}");
                }
                else
                {
                    Console.WriteLine("  Inicialización completada");
                }
                Console.WriteLine();

                // PASO 3: Establecer conexión con la impresora
                Console.WriteLine("PASO 3: Conectando con la impresora...");
                resultado = socketComm.CS_StartClient(puntero);
                
                if (resultado == 0)
                {
                    Console.WriteLine("  Conexión establecida exitosamente!");
                    Console.WriteLine();

                    // PASO 4: Verificar estado de la conexión
                    Console.WriteLine("PASO 4: Verificando estado de la conexión...");
                    Int32 estadoConexion = socketComm.CS_IsConnected(puntero);
                    if (estadoConexion == 1)
                    {
                        Console.WriteLine("  La impresora está conectada");
                    }
                    else
                    {
                        Console.WriteLine("  Advertencia: La impresora no está conectada");
                    }
                    Console.WriteLine();

                    // PASO 5: Obtener información básica de la impresora
                    Console.WriteLine("PASO 5: Obteniendo información de la impresora...");
                    
                    // Obtener versión de la DLL
                    Int32 versionDll = socketComm.CS_GetDllVersion();
                    Console.WriteLine($"  Versión DLL: {versionDll}");

                    // Obtener versión del firmware de la impresora
                    string versionFirmware = "";
                    resultado = socketComm.CS_GetVersionString(puntero, ref versionFirmware, 0);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Versión Firmware: {versionFirmware}");
                    }

                    // Obtener estado extendido de la impresora
                    SocketComm.CSStatusExt estado = new SocketComm.CSStatusExt();
                    resultado = socketComm.CS_StatusExt(puntero, ref estado);
                    if (resultado == 0)
                    {
                        Console.WriteLine($"  Mensaje activo: {estado.messagename}");
                        Console.WriteLine($"  Contador total: {estado.t_counter}");
                        Console.WriteLine($"  Contador OK: {estado.d_counter}");
                        Console.WriteLine($"  Contador NOK: {estado.s_counter}");
                        Console.WriteLine($"  Estado: {(estado.Start == 0 ? "Imprimiendo" : "Detenido")}");
                    }
                    else
                    {
                        socketComm.CS_GetLastError(puntero, ref mensajeError);
                        Console.WriteLine($"  Advertencia - Error al obtener estado: {mensajeError}");
                    }
                    Console.WriteLine();

                    Console.WriteLine("==========================================");
                    Console.WriteLine("  Prueba básica completada exitosamente");
                    Console.WriteLine("==========================================");
                }
                else
                {
                    socketComm.CS_GetLastError(puntero, ref mensajeError);
                    Console.WriteLine($"  Error al conectar: Código {resultado}");
                    Console.WriteLine($"  Mensaje: {mensajeError}");
                    Console.WriteLine();
                    Console.WriteLine("Posibles causas:");
                    Console.WriteLine("  - La IP de la impresora es incorrecta");
                    Console.WriteLine("  - La impresora no está encendida o no está en la red");
                    Console.WriteLine("  - Hay un firewall bloqueando la conexión");
                    Console.WriteLine("  - El puerto TCP/IP no está disponible");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error inesperado: {ex.Message}");
                Console.WriteLine($"  Detalles: {ex.StackTrace}");
            }
            finally
            {
                // PASO 6: Cerrar la conexión correctamente
                if (puntero != 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("PASO 6: Cerrando conexión...");
                    try
                    {
                        socketComm.CS_Knockout(puntero);  // Notificar a la impresora que vamos a desconectar
                        socketComm.CS_Finish(puntero);    // Cerrar el socket
                        Console.WriteLine("  Conexión cerrada correctamente");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Advertencia - Error al cerrar: {ex.Message}");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}

