using LaserMacsaUser.Views;
using System;
using System.Windows.Forms;

namespace LaserMacsaUser

{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Verificar si ya existe una instancia ejecutándose ANTES de inicializar la aplicación
            if (Common.PrevInstance())
            {
                // Inicializar Windows Forms solo para mostrar el mensaje
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    // Mostrar mensaje de alerta en inglés
                    Common.ShowAlreadyRunningMessage();
                }
                catch
                {
                    // Si no se puede mostrar el mensaje gráfico, mostrar en consola
                    Console.WriteLine("The application is already running.");
                }
                // Liberar recursos antes de salir
                Common.ReleaseMutex();
                return; // Salir sin iniciar la aplicación
            }

            // Configurar eventos para liberar mutex cuando la aplicación termine
            Application.ApplicationExit += (sender, e) => 
            {
                Common.ReleaseMutex();
            };
            
            try
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new SplashForm());
            }
            catch (Exception ex)
            {
                // Capturar cualquier error de inicialización
                MessageBox.Show(
                    $"Error starting application: {ex.Message}",
                    "Application Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Liberar el mutex cuando la aplicación se cierra (backup en caso de que ApplicationExit no se dispare)
                Common.ReleaseMutex();
            }
        }
    }
}