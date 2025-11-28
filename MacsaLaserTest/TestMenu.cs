using System;

namespace MacsaLaserTest
{
    /// <summary>
    /// Menú principal para ejecutar los diferentes tests
    /// </summary>
    class TestMenu
    {
        static void Main(string[] args)
        {
            bool continuar = true;

            while (continuar)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("  MENÚ DE PRUEBAS - MACSA LASER");
                Console.WriteLine("  Protocolo TCP/IP v3.1");
                Console.WriteLine("==========================================");
                Console.WriteLine();

                // Mostrar IP actual
                string ipActual = TestConfigHelper.GetLaserIP();
                Console.WriteLine($"IP Configurada: {ipActual}");
                Console.WriteLine("(La IP se lee desde la configuración de LaserMacsaUser)");
                Console.WriteLine();

                Console.WriteLine("Seleccione una prueba:");
                Console.WriteLine();
                Console.WriteLine("  1. Información Completa de la Impresora");
                Console.WriteLine("  2. Mensajes de Usuario (User Messages)");
                Console.WriteLine("  3. Control de Impresión");
                Console.WriteLine("  4. Enviar Archivos");
                Console.WriteLine("  5. Gestionar Archivos");
                Console.WriteLine("  6. Contadores");
                Console.WriteLine("  7. Configuración (Modo, Offset, Defocus, Powerscale)");
                Console.WriteLine("  8. Monitoreo");
                Console.WriteLine("  9. Alarmas");
                Console.WriteLine();
                Console.WriteLine("  R. Recargar IP desde configuración");
                Console.WriteLine("  0. Salir");
                Console.WriteLine();
                Console.Write("Ingrese su opción: ");

                string opcion = Console.ReadLine();

                Console.WriteLine();
                Console.WriteLine("==========================================");
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case "1":
                            ProgramInfoTest.Run();
                            break;
                        case "2":
                            Test02_UserMessages.Run();
                            break;
                        case "3":
                            Test03_ControlImpresion.Run();
                            break;
                        case "4":
                            Test04_EnviarArchivos.Run();
                            break;
                        case "5":
                            Test05_GestionarArchivos.Run();
                            break;
                        case "6":
                            Test06_Contadores.Run();
                            break;
                        case "7":
                            Test07_Configuracion.Run();
                            break;
                        case "8":
                            Test08_Monitoreo.Run();
                            break;
                        case "9":
                            Test09_Alarmas.Run();
                            break;
                        case "R":
                        case "r":
                            TestConfigHelper.ClearCache();
                            Console.WriteLine("Caché de IP limpiada. La próxima vez se leerá la IP actualizada desde la configuración.");
                            Console.WriteLine($"IP actual: {TestConfigHelper.GetLaserIP()}");
                            Console.WriteLine();
                            Console.WriteLine("Presione cualquier tecla para continuar...");
                            Console.ReadKey();
                            break;
                        case "0":
                            continuar = false;
                            Console.WriteLine("Saliendo...");
                            break;
                        default:
                            Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al ejecutar la prueba: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    Console.WriteLine();
                    Console.WriteLine("Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                }

                if (continuar && opcion != "0")
                {
                    Console.WriteLine();
                    Console.WriteLine("Presione cualquier tecla para volver al menú...");
                    Console.ReadKey();
                }
            }
        }
    }
}

