using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace LaserMacsaUser
{
    public static class Common
    {
        private static Mutex? _mutex = null;
        private static bool _mutexOwned = false;
        private const string MutexName = "Global\\LaserMacsaUser_SingleInstance_Mutex";
        private const string ProcessName = "LaserMacsaUser";

        /// <summary>
        /// Verifica si hay procesos del programa ejecutándose (excluyendo el actual).
        /// </summary>
        private static bool IsProcessRunning()
        {
            try
            {
                int currentProcessId = Process.GetCurrentProcess().Id;
                Process[] processes = Process.GetProcessesByName(ProcessName);
                
                // Verificar si hay algún proceso ejecutándose que no sea el actual
                foreach (Process proc in processes)
                {
                    if (proc.Id != currentProcessId)
                    {
                        // Verificar que el proceso todavía esté ejecutándose
                        try
                        {
                            // Si podemos acceder al MainModule, el proceso está activo
                            var _ = proc.MainModule;
                            return true;
                        }
                        catch
                        {
                            // Si no podemos acceder, el proceso está terminando o sin permisos
                            // Continuar verificando otros procesos
                        }
                    }
                }
                return false;
            }
            catch
            {
                // En caso de error, asumir que no hay procesos
                return false;
            }
        }

        /// <summary>
        /// Verifica si ya existe una instancia previa del programa ejecutándose.
        /// </summary>
        /// <returns>true si ya existe una instancia previa, false en caso contrario</returns>
        public static bool PrevInstance()
        {
            try
            {
                // Primero verificar si realmente hay procesos ejecutándose
                if (IsProcessRunning())
                {
                    // Hay un proceso ejecutándose - verificar el mutex
                    try
                    {
                        // Intentar abrir el mutex existente
                        bool mutexExists = Mutex.TryOpenExisting(MutexName, out Mutex? existingMutex);
                        if (mutexExists && existingMutex != null)
                        {
                            try
                            {
                                existingMutex.Dispose();
                            }
                            catch { }
                            return true; // Hay proceso y mutex - definitivamente está corriendo
                        }
                        // Si no hay mutex pero hay proceso, puede ser un proceso antiguo sin mutex
                        return true;
                    }
                    catch
                    {
                        // Si no podemos verificar el mutex, pero hay proceso, asumir que está corriendo
                        return true;
                    }
                }

                // No hay procesos ejecutándose - intentar crear el mutex
                try
                {
                    _mutex = new Mutex(true, MutexName, out bool createdNew);
                    _mutexOwned = createdNew;

                    if (!createdNew)
                    {
                        // El mutex existe pero no hay proceso - está abandonado, reclamarlo
                        if (_mutex != null)
                        {
                            try
                            {
                                _mutex.Dispose();
                            }
                            catch { }
                        }
                        // Crear nuevo mutex
                        _mutex = new Mutex(true, MutexName, out createdNew);
                        _mutexOwned = createdNew;
                    }

                    return false; // Esta es la primera instancia
                }
                catch (AbandonedMutexException)
                {
                    // Mutex abandonado - reclamarlo
                    if (_mutex != null)
                    {
                        try
                        {
                            _mutex.Dispose();
                        }
                        catch { }
                    }
                    _mutex = new Mutex(true, MutexName, out bool createdNew);
                    _mutexOwned = createdNew;
                    return false;
                }
            }
            catch (Exception ex)
            {
                // En caso de error, verificar procesos como respaldo
                System.Diagnostics.Debug.WriteLine($"Error al verificar instancia previa: {ex.Message}");
                
                // Si hay procesos ejecutándose, asumir que hay instancia previa
                if (IsProcessRunning())
                {
                    return true;
                }
                
                // Si no hay procesos y hay error, permitir continuar pero limpiar
                if (_mutex != null)
                {
                    try
                    {
                        _mutex.Dispose();
                    }
                    catch { }
                    _mutex = null;
                    _mutexOwned = false;
                }
                return false;
            }
        }

        /// <summary>
        /// Muestra un mensaje de alerta indicando que ya hay una instancia ejecutándose.
        /// </summary>
        public static void ShowAlreadyRunningMessage()
        {
            try
            {
                MessageBox.Show(
                    "The application is already running.\n\nPlease close the existing instance before starting a new one.",
                    "Application Already Running",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch
            {
                // Si MessageBox falla, intentar mostrar en consola
                try
                {
                    Console.WriteLine("The application is already running.");
                    Console.WriteLine("Please close the existing instance before starting a new one.");
                }
                catch
                {
                    // Si todo falla, no hacer nada (evitar errores)
                }
            }
        }

        /// <summary>
        /// Libera el mutex cuando la aplicación se cierra.
        /// El mutex se liberará automáticamente cuando el proceso termine, pero intentamos liberarlo explícitamente.
        /// </summary>
        public static void ReleaseMutex()
        {
            try
            {
                if (_mutex != null && _mutexOwned)
                {
                    try
                    {
                        // Solo intentar liberar si este proceso lo creó
                        // Si el proceso termina abruptamente, Windows liberará el mutex automáticamente
                        _mutex.ReleaseMutex();
                        _mutexOwned = false;
                    }
                    catch (AbandonedMutexException)
                    {
                        // El mutex fue abandonado (proceso anterior cerró sin liberarlo) - está bien
                        _mutexOwned = false;
                    }
                    catch
                    {
                        // Ignorar otros errores al liberar
                    }
                }
                
                // Siempre intentar disponer del mutex
                if (_mutex != null)
                {
                    try
                    {
                        _mutex.Dispose();
                    }
                    catch
                    {
                        // Ignorar errores al disponer - Windows lo limpiará cuando el proceso termine
                    }
                    _mutex = null;
                    _mutexOwned = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al liberar mutex: {ex.Message}");
                // Asegurarnos de que el mutex se marque como no propiedad incluso si hay error
                _mutexOwned = false;
            }
        }
    }
}

