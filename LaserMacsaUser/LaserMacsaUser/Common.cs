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
                    try
                    {
                        if (proc.Id != currentProcessId && !proc.HasExited)
                        {
                            // Verificar que el proceso todavía esté ejecutándose
                            try
                            {
                                // Intentar refrescar para ver si el proceso sigue activo
                                proc.Refresh();
                                if (!proc.HasExited)
                                {
                                    return true;
                                }
                            }
                            catch
                            {
                                // Si no podemos acceder, el proceso está terminando
                                // Continuar verificando otros procesos
                            }
                        }
                    }
                    catch
                    {
                        // Ignorar errores con este proceso específico
                        continue;
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
                // Primero verificar SIEMPRE si realmente hay procesos ejecutándose
                // Esta es la verificación más confiable
                bool hasRunningProcess = IsProcessRunning();
                
                if (hasRunningProcess)
                {
                    // Hay un proceso ejecutándose - definitivamente está corriendo
                    return true;
                }

                // No hay procesos ejecutándose - intentar crear el mutex
                // Si el mutex existe pero no hay procesos, está abandonado y lo reclamamos
                try
                {
                    // Esperar un momento para asegurarse de que no hay procesos terminando
                    System.Threading.Thread.Sleep(200);
                    
                    // Verificar una vez más los procesos antes de crear el mutex
                    if (IsProcessRunning())
                    {
                        return true; // Ahora sí hay un proceso ejecutándose
                    }
                    
                    _mutex = new Mutex(true, MutexName, out bool createdNew);
                    _mutexOwned = createdNew;

                    if (!createdNew)
                    {
                        // El mutex existe pero no hay proceso - está abandonado
                        // Esperar un momento más y verificar procesos de nuevo
                        System.Threading.Thread.Sleep(200);
                        
                        // Verificar una vez más si hay procesos (podría haber un delay)
                        if (!IsProcessRunning())
                        {
                            // No hay procesos - el mutex está abandonado, reclamarlo
                            if (_mutex != null)
                            {
                                try
                                {
                                    _mutex.Dispose();
                                }
                                catch { }
                            }
                            // Forzar la creación del mutex (reclamar el abandonado)
                            _mutex = new Mutex(true, MutexName, out createdNew);
                            _mutexOwned = createdNew;
                            return false; // Permitir ejecutar
                        }
                        else
                        {
                            // Ahora sí hay proceso - está corriendo
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
                            return true;
                        }
                    }

                    return false; // Esta es la primera instancia
                }
                catch (AbandonedMutexException)
                {
                    // Mutex abandonado - verificar procesos antes de reclamar
                    if (!IsProcessRunning())
                    {
                        // No hay procesos - reclamar el mutex
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
                        return false; // Permitir ejecutar
                    }
                    else
                    {
                        // Hay procesos - está corriendo
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
                        return true;
                    }
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
                return false; // Permitir continuar si no hay procesos
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

