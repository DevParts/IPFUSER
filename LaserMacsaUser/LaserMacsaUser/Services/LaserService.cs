using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LaserMacsaUser.Models;
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

        public bool Initialize(string ipAddress, string messagePath = ".\\")
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
                    throw new Exception($"Error al conectar socket principal: {error}");
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
                    throw new Exception($"Error al conectar socket secundario: {error}");
                }

                _isInitialized = true;
                return true;
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                System.Diagnostics.Debug.WriteLine($"Error en Initialize: {ex.Message}");
                return false;
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
                                    IsActive = true
                                });
                            }
                        }

                        // También disparar si hay un código de alarma directo
                        if (status.AlarmCode != 0 && !status.AlarmCodes.Contains((int)status.AlarmCode))
                        {
                            AlarmDetected?.Invoke(this, new LaserAlarmEventArgs
                            {
                                AlarmCode = (int)status.AlarmCode,
                                AlarmDescription = GetAlarmDescription((int)status.AlarmCode),
                                IsActive = true
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

            // Procesar alarmMask1 y alarmMask2 para extraer códigos de alarma activos
            // Esto es una implementación básica - puede necesitar ajustes según la documentación
            if (alarmCode != 0)
            {
                alarms.Add((int)(alarmCode & 0xFFFF)); // Lower WORD
            }

            // Agregar más procesamiento según sea necesario

            return alarms;
        }

        private string GetAlarmDescription(int alarmCode)
        {
            // Mapeo básico de códigos de alarma comunes
            return alarmCode switch
            {
                0 => "Sin errores",
                1 => "Error de comunicación",
                2 => "Error de comunicación con el láser",
                3 => "Error de hardware",
                4 => "Error de archivo",
                5 => "Error de parámetros",
                9 => "Buffer lleno",
                10 => "Temperatura alta",
                16 => "Error de archivo láser",
                _ => $"Alarma desconocida: {alarmCode}"
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

        public bool SendUserMessage(int fieldIndex, string message)
        {
            if (!_isInitialized || _socketComm == null)
                return false;

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
    }
}

