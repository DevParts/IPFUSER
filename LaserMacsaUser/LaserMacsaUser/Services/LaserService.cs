using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LaserMacsaUser.Models;
using LaserMacsaUser.Exceptions;
using SocketCommNet;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Servicio de comunicación con el láser usando SocketCommNet (TCPIPver31)
    /// 
    /// FLUJO DE CONEXIÓN:
    /// 1. Initialize() -> CS_Init() -> CS_StartClient() -> Conecta a SocketCommDll.dll
    /// 2. SocketCommDll.dll se comunica con el láser vía TCP/IP
    /// 3. Todas las operaciones pasan por SocketCommNet (wrapper) -> SocketCommDll.dll (nativa)
    /// 
    /// SOCKETS:
    /// - Socket principal (_socketHandle): Control y estado
    /// - Socket secundario (_socketHandle2): Envío de códigos (alta velocidad)
    /// </summary>
    public class LaserService : ILaserService
    {
        private SocketComm? _socketComm;
        private int _socketHandle;      // Socket principal (control)
        private int _socketHandle2;     // Socket secundario (envío de códigos)
        private bool _isInitialized = false;
        private string _lastError = string.Empty;

        public bool IsConnected => _isInitialized && _socketComm != null &&
                                   _socketComm.CS_IsConnected(_socketHandle) != 0;

        public event EventHandler<LaserAlarmEventArgs>? AlarmDetected;

        public bool Initialize(string ipAddress, string messagePath = ".\\", int? bufferSize = null, int? userFields = null)
        {
            try
            {
                _socketComm = new SocketComm();
                _socketHandle = 0;
                _socketHandle2 = 0;

                string name = "anyword";
                string path = messagePath;

                // Inicializar socket principal (control)
                _socketComm.CS_Init(ref _socketHandle, name, ipAddress, path);

                int result = _socketComm.CS_StartClient(_socketHandle);
                if (result != 0)
                {
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle, ref error);
                    _lastError = error;
                    throw new LaserCommunicationException(ipAddress, "Initialize (socket principal)", result, error);
                }

                // Inicializar socket secundario (envío de códigos) - como RunThread() en CLaser.cs
                _socketComm.CS_Init(ref _socketHandle2, name, ipAddress, path);
                result = _socketComm.CS_StartClient(_socketHandle2);
                if (result != 0)
                {
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle2, ref error);
                    _lastError = error;
                    
                    // Cerrar socket principal si falla el secundario
                    _socketComm.CS_Finish(_socketHandle);
                    throw new LaserCommunicationException(ipAddress, "Initialize (socket secundario)", result, error);
                }

                _isInitialized = true;

                // Configurar buffer automáticamente si se proporcionan los parámetros
                if (bufferSize.HasValue && userFields.HasValue)
                {
                    if (!ConfigureBuffer(bufferSize.Value, userFields.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Advertencia: No se pudo configurar el buffer. Error: {_lastError}");
                        // No fallar la inicialización si falla la configuración del buffer
                        // El buffer puede configurarse manualmente después
                    }
                }

                return true;
            }
            catch (LaserCommunicationException)
            {
                // Re-lanzar excepciones de comunicación
                throw;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                System.Diagnostics.Debug.WriteLine($"Error en Initialize: {ex.Message}");
                throw new LaserCommunicationException(ipAddress, "Initialize", -1, ex.Message, ex);
            }
        }

        public LaserStatus GetStatus()
        {
            if (!_isInitialized || _socketComm == null)
            {
                return new LaserStatus { IsConnected = false };
            }

            try
            {
                var statusExt = new SocketComm.CSStatusExt();
                int result = _socketComm.CS_StatusExt(_socketHandle, ref statusExt);

                if (result == 0)
                {
                    var status = new LaserStatus
                    {
                        OkCounter = statusExt.d_counter,
                        NokCounter = statusExt.s_counter,
                        TotalCounter = statusExt.t_counter,
                        Start = statusExt.Start,
                        MessageName = statusExt.messagename,
                        EventHandler = statusExt.eventhandler,
                        MessagePort = statusExt.n_messageport,
                        Copies = statusExt.m_copies,
                        AlarmCode = statusExt.err,
                        AlarmMask1 = statusExt.alarmmask1,
                        AlarmMask2 = statusExt.alarmmask2,
                        SignalState = statusExt.signalstate,
                        Extra = statusExt.extra,
                        BufferCount = GetBufferCount(),
                        IsConnected = _socketComm.CS_IsConnected(_socketHandle) != 0
                    };

                    // Procesar códigos de alarma
                    status.AlarmCodes = ProcessAlarmCodes(statusExt.alarmmask1, statusExt.alarmmask2, statusExt.err);

                    // Disparar evento si hay alarmas detectadas
                    if (status.AlarmCodes.Count > 0 || status.AlarmCode != 0)
                    {
                        foreach (int alarmCode in status.AlarmCodes)
                        {
                            if (alarmCode != 0)
                            {
                                AlarmDetected?.Invoke(this, new LaserAlarmEventArgs
                                {
                                    AlarmCode = alarmCode,
                                    AlarmDescription = GetAlarmDescription(alarmCode),
                                    IsActive = true,
                                    IsCritical = IsCriticalAlarm(alarmCode)
                                });
                            }
                        }

                        // También disparar si hay un código de alarma directo
                        if (status.AlarmCode != 0 && !status.AlarmCodes.Contains((int)status.AlarmCode))
                        {
                            int directAlarmCode = (int)status.AlarmCode;
                            AlarmDetected?.Invoke(this, new LaserAlarmEventArgs
                            {
                                AlarmCode = directAlarmCode,
                                AlarmDescription = GetAlarmDescription(directAlarmCode),
                                IsActive = true,
                                IsCritical = IsCriticalAlarm(directAlarmCode)
                            });
                        }
                    }

                    return status;
                }
                else
                {
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle, ref error);
                    _lastError = error;
                    return new LaserStatus { IsConnected = false };
                }
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return new LaserStatus { IsConnected = false };
            }
        }

        private List<int> ProcessAlarmCodes(uint alarmMask1, uint alarmMask2, uint alarmCode)
        {
            var alarms = new List<int>();

            // Procesar alarmCode directo
            if (alarmCode != 0)
            {
                alarms.Add((int)(alarmCode & 0xFFFF));
            }

            // Procesar AlarmMask1 (bits 0-31)
            for (int i = 0; i < 32; i++)
            {
                if ((alarmMask1 & (1u << i)) != 0)
                {
                    alarms.Add(i);
                }
            }

            // Procesar AlarmMask2 (bits 32-63, alarmas extendidas)
            for (int i = 0; i < 32; i++)
            {
                if ((alarmMask2 & (1u << i)) != 0)
                {
                    alarms.Add(32 + i);
                }
            }

            return alarms.Distinct().ToList();
        }

        private string GetAlarmDescription(int alarmCode)
        {
            // Mapeo completo de códigos de alarma según alarmcodes.pdf
            return alarmCode switch
            {
                // Alarmas básicas
                0 => "Sin errores",
                1 => "Error de comunicación",
                2 => "Laser is OFF (interlock open) - Hardware",
                3 => "Shutter is closed (obsolete)",
                4 => "DC Power fails (obsolete)",
                5 => "Overtemp of amplifier (obsolete)",
                6 => "Q-switch error - Hardware",
                7 => "High reverse power (obsolete)",
                8 => "Low forward power (obsolete)",
                9 => "YAG RS232 error (obsolete)",
                0x0A => "Belt stopped - Hardware/Software",
                0x0B => "Program check not ok (obsolete)",
                0x0C => "Wrong figure-types in file (obsolete)",
                0x0D => "No memory available - Software",
                0x10 => "File not found - Software",
                0x11 => "Overpressure (100W DEOS systems) - Hardware",
                0x12 => "Water temperature (100W DEOS systems) - Hardware",
                0x13 => "Water level (100W DEOS systems) - Hardware",
                0x15 => "Invalid font (or not existing) - Software",
                0x16 => "Overtemperature - Hardware (CRÍTICA)",
                0x24 => "Warmup cycle still active - Hardware",
                0x25 => "Shutter closed - Hardware",
                0x26 => "Laser not ready - Hardware",
                0x27 => "OEM shutter - Hardware",
                0x28 => "Power off - Hardware",
                0x30 => "Overspeed - Software",
                0x31 => "Harddisk full - Software",
                0x40 => "Client timeout - Software",
                0x41 => "Scanner X alarm - Hardware",
                0x42 => "Scanner Y alarm - Hardware",
                0x43 => "Empty message alarm - Software",
                0x44 => "Initialization alarm - Software",
                0x45 => "User-define alarm - Hardware",
                0x46 => "Z scanner error",
                0x47 => "Laser not armed",
                0x48 => "XY out of range",
                0x49 => "Laser measurement failed",
                0x50 => "UV laser not ready",
                0x51 => "Pixmap out of range",
                0x52 => "Channel status error",
                0x53 => "PWM out of range",
                0x54 => "RTC battery failure",
                0x55 => "CPU temperature alarm",
                0x56 => "Board temperature alarm",
                0x57 => "Undervoltage 5V",
                0x58 => "Undervoltage 3.3V",
                0x61 => "Watchdog",
                0x62 => "DSP paused",
                0x63 => "FPGA failure",
                0x64 => "DSP alarmmask",
                0x65 => "Shutter sensor not open",
                0x66 => "Shutter sensor not closed",
                _ => $"Alarma desconocida: 0x{alarmCode:X2} ({alarmCode})"
            };
        }

        /// <summary>
        /// Determina si una alarma es crítica (detiene la impresión)
        /// </summary>
        private bool IsCriticalAlarm(int alarmCode)
        {
            // Alarmas que detienen la impresión automáticamente
            return alarmCode switch
            {
                0x02 => true,  // Laser OFF (interlock open)
                0x06 => true,  // Q-switch error
                0x16 => true,  // Overtemperature
                0x24 => true,  // Warmup cycle still active
                0x25 => true,  // Shutter closed
                0x26 => true,  // Laser not ready
                0x28 => true,  // Power off
                0x41 => true,  // Scanner X alarm
                0x42 => true,  // Scanner Y alarm
                0x44 => true,  // Initialization alarm
                0x46 => true,  // Z scanner error
                0x47 => true,  // Laser not armed
                0x61 => true,  // Watchdog
                0x62 => true,  // DSP paused
                0x63 => true,  // FPGA failure
                _ => false
            };
        }

        public bool StartPrint(string filename, int copies)
        {
            if (!_isInitialized || _socketComm == null)
                return false;

            try
            {
                // Extraer nombre sin extensión si viene con extensión
                string nameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(filename);
                int result = _socketComm.CS_Start(_socketHandle, nameWithoutExt, copies);

                if (result != 0)
                {
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle, ref error);
                    _lastError = error;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }
        }

        public int SendUserMessage(int fieldIndex, string message)
        {
            if (!_isInitialized || _socketComm == null)
            {
                _lastError = "Láser no inicializado";
                return -1; // Error de inicialización
            }

            try
            {
                // Usar socket secundario para envío de códigos (alta velocidad)
                // CS_FastUsermessage usa UTF-8 internamente
                int result = _socketComm.CS_FastUsermessage(_socketHandle2, fieldIndex, message);

                if (result != 0)
                {
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle2, ref error);
                    _lastError = error;
                    
                    // Para errores críticos de comunicación, lanzar excepción
                    if (result == -1 || result < -1)
                    {
                        throw new LaserCommunicationException(
                            "N/A", 
                            $"SendUserMessage (Field {fieldIndex})", 
                            result, 
                            error);
                    }
                }

                return result; // Retornar código de error directamente (0 = éxito, 8 = buffer lleno)
            }
            catch (LaserCommunicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                throw new LaserCommunicationException(
                    "N/A", 
                    $"SendUserMessage (Field {fieldIndex})", 
                    -1, 
                    ex.Message, 
                    ex);
            }
        }

        public void Stop()
        {
            if (!_isInitialized || _socketComm == null)
                return;

            try
            {
                // Detener impresión
                _socketComm.CS_Stop(_socketHandle, 2000);

                // Limpiar buffer (Knockout)
                _socketComm.CS_Knockout(_socketHandle);
                _socketComm.CS_Knockout(_socketHandle2);

                // Cerrar sockets
                _socketComm.CS_Finish(_socketHandle);
                _socketComm.CS_Finish(_socketHandle2);

                _socketHandle = 0;
                _socketHandle2 = 0;
                _isInitialized = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en Stop: {ex.Message}");
            }
        }

        public int GetBufferCount()
        {
            if (!_isInitialized || _socketComm == null)
                return 0;

            try
            {
                // Usar CS_EnableBufferedUMExt con get=1 para obtener fillStatus (número de elementos en buffer)
                // Basado en CLaser.InBufferCount() del sistema original
                int actSize = 0;
                int field = 0;
                int fillStatus = 0;
                int defSize = 100; // Tamaño por defecto del buffer

                // get=1: obtener estado del buffer
                _socketComm.CS_EnableBufferedUMExt(_socketHandle, 1, ref actSize, ref field, ref fillStatus, defSize);
                
                return fillStatus; // fillStatus contiene el número de elementos en el buffer
            }
            catch
            {
                return 0;
            }
        }

        public string GetLastError()
        {
            return _lastError;
        }

        public bool CopyMessageFile(string messageName, string messagePath)
        {
            if (!_isInitialized || _socketComm == null)
                return false;

            try
            {
                // Según el código original (CLaser.cs línea 239):
                // - messageName debe ser el nombre del archivo (con extensión)
                // - messagePath debe terminar con "\\"
                // - option: 0 = harddisk, 1 = ramdisk

                string filename = messageName;
                string path = messagePath;

                // Si messageName es una ruta completa, extraer solo el nombre del archivo
                if (Path.IsPathRooted(messageName))
                {
                    filename = Path.GetFileName(messageName);
                    // Si no se proporcionó path, extraerlo de messageName
                    if (string.IsNullOrEmpty(messagePath) || messagePath == ".\\" || messagePath == "./")
                    {
                        path = Path.GetDirectoryName(messageName) ?? string.Empty;
                    }
                }

                // Asegurar que path termine con "\\" (según documentación de CS_CopyFile)
                if (!string.IsNullOrEmpty(path))
                {
                    if (!path.EndsWith("\\") && !path.EndsWith("/"))
                    {
                        path += "\\";
                    }
                }
                else
                {
                    path = ".\\"; // Ruta por defecto
                }

                // Verificar que el archivo existe localmente antes de copiar
                string fullLocalPath = Path.IsPathRooted(messageName) ? messageName : Path.Combine(path, filename);
                if (!File.Exists(fullLocalPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Advertencia: Archivo no encontrado localmente: '{fullLocalPath}'. Intentando copiar de todas formas.");
                }

                // option = 0: copiar al disco duro del láser (harddisk)
                // option = 1: copiar a la RAM disk del láser
                byte option = 0;
                int result = _socketComm.CS_CopyFile(_socketHandle, filename, path, option);

                if (result != 0)
                {
                    // Obtener error para diagnóstico
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle, ref error);
                    _lastError = $"Error al copiar archivo: {error}. Archivo: '{filename}', Ruta: '{path}'";
                    System.Diagnostics.Debug.WriteLine(_lastError);
                    return false;
                }

                // Pequeña espera después de copiar para asegurar que el archivo esté disponible en el láser
                System.Threading.Thread.Sleep(100);

                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Excepción en CopyMessageFile: {ex.Message}";
                System.Diagnostics.Debug.WriteLine(_lastError);
                return false;
            }
        }

        public bool SetDefaultMessage(string messageName)
        {
            if (!_isInitialized || _socketComm == null)
                return false;

            try
            {
                // Según el código original (CLaser.cs línea 240):
                // SetDefault recibe el nombre del archivo (puede ser con o sin extensión)
                // El láser internamente maneja ambos casos

                // Extraer solo el nombre del archivo si viene como ruta completa
                string filename = Path.IsPathRooted(messageName)
                    ? Path.GetFileName(messageName)  // Con extensión
                    : messageName;

                // Intentar primero con el nombre completo (con extensión) como en el código original
                int result = _socketComm.CS_SetDefault(_socketHandle, filename);

                if (result == 0)
                {
                    return true;
                }

                // Si falla, intentar sin extensión (según documentación)
                string nameWithoutExt = Path.GetFileNameWithoutExtension(filename);
                result = _socketComm.CS_SetDefault(_socketHandle, nameWithoutExt);

                if (result == 0)
                {
                    return true;
                }

                // Si aún falla, obtener el error para diagnóstico
                string error = string.Empty;
                _socketComm.CS_GetLastError(_socketHandle, ref error);
                _lastError = $"Error en SetDefaultMessage: {error}. Intentado: '{filename}' y '{nameWithoutExt}'";
                System.Diagnostics.Debug.WriteLine(_lastError);

                return false;
            }
            catch (Exception ex)
            {
                _lastError = $"Excepción en SetDefaultMessage: {ex.Message}";
                System.Diagnostics.Debug.WriteLine(_lastError);
                return false;
            }
        }

        public string GetFastUsermessage(int fieldIndex)
        {
            if (!_isInitialized || _socketComm == null)
                return string.Empty;

            try
            {
                string message = string.Empty;
                int result = _socketComm.CS_GetFastUsermessage(_socketHandle, fieldIndex, ref message);

                if (result == 0)
                {
                    return message;
                }
                else
                {
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle, ref error);
                    _lastError = error;
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return string.Empty;
            }
        }

        public bool ConfigureBuffer(int bufferSize, int userFields)
        {
            if (!_isInitialized || _socketComm == null)
                return false;

            try
            {
                // Configurar buffer para cada campo de usuario
                // set=0: configurar buffer
                int actSize = 0;
                int field = 0;
                int fillStatus = 0;

                for (int i = 0; i < userFields; i++)
                {
                    field = i;
                    int result = _socketComm.CS_EnableBufferedUMExt(_socketHandle, 0, ref actSize, ref field, ref fillStatus, bufferSize);
                    
                    if (result != 0)
                    {
                        string error = string.Empty;
                        _socketComm.CS_GetLastError(_socketHandle, ref error);
                        _lastError = $"Error al configurar buffer para campo {i}: {error}";
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }
        }

        public bool ResetBuffer(int fieldIndex)
        {
            if (!_isInitialized || _socketComm == null)
                return false;

            try
            {
                // set=2: resetear buffer
                int actSize = 0;
                int field = fieldIndex;
                int fillStatus = 0;
                int defSize = 100; // Valor por defecto, no se usa en reset

                int result = _socketComm.CS_EnableBufferedUMExt(_socketHandle, 2, ref actSize, ref field, ref fillStatus, defSize);

                if (result != 0)
                {
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle, ref error);
                    _lastError = $"Error al resetear buffer para campo {fieldIndex}: {error}";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }
        }
    }
}

