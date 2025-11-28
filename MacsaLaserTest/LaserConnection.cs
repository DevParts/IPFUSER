using System;
using SocketCommNet;

namespace MacsaLaserTest
{
    /// <summary>
    /// Clase modular para manejar la conexión con la impresora laser Macsa
    /// Aísla toda la lógica de conexión para facilitar el debugging
    /// </summary>
    public class LaserConnection : IDisposable
    {
        private SocketComm _socketComm;
        private Int32 _puntero;
        private bool _isConnected;
        private string _lastError;

        /// <summary>
        /// Indica si la conexión está activa
        /// </summary>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Último error ocurrido
        /// </summary>
        public string LastError => _lastError;

        /// <summary>
        /// Puntero interno de la conexión (para uso interno)
        /// </summary>
        internal Int32 Puntero => _puntero;

        /// <summary>
        /// Objeto SocketComm interno (para uso interno)
        /// </summary>
        internal SocketComm SocketComm => _socketComm;

        public LaserConnection()
        {
            _socketComm = new SocketComm();
            _puntero = 0;
            _isConnected = false;
            _lastError = "";
        }

        /// <summary>
        /// Inicializa la conexión con la impresora
        /// </summary>
        /// <param name="nombreConexion">Nombre de la conexión (puede ser cualquier string)</param>
        /// <param name="ipImpresora">Dirección IP de la impresora</param>
        /// <param name="rutaLocal">Ruta local base (puede ser cualquier ruta válida)</param>
        /// <returns>True si la inicialización fue exitosa</returns>
        public bool Inicializar(string nombreConexion, string ipImpresora, string rutaLocal)
        {
            try
            {
                _socketComm.CS_Init(ref _puntero, nombreConexion, ipImpresora, rutaLocal);
                
                // Verificar errores de inicialización
                string errorMsg = "";
                Int32 resultado = _socketComm.CS_GetLastError(_puntero, ref errorMsg);
                if (resultado != 0)
                {
                    _lastError = $"Error en inicialización: {errorMsg}";
                    return false;
                }

                _lastError = "";
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Excepción en Inicializar: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Establece la conexión TCP/IP con la impresora
        /// </summary>
        /// <returns>True si la conexión fue exitosa</returns>
        public bool Conectar()
        {
            try
            {
                if (_puntero == 0)
                {
                    _lastError = "Debe inicializar la conexión primero";
                    return false;
                }

                Int32 resultado = _socketComm.CS_StartClient(_puntero);
                
                if (resultado == 0)
                {
                    _isConnected = true;
                    _lastError = "";
                    return true;
                }
                else
                {
                    string errorMsg = "";
                    _socketComm.CS_GetLastError(_puntero, ref errorMsg);
                    _lastError = $"Error al conectar (código {resultado}): {errorMsg}";
                    _isConnected = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                _lastError = $"Excepción en Conectar: {ex.Message}";
                _isConnected = false;
                return false;
            }
        }

        /// <summary>
        /// Verifica si la conexión está activa
        /// </summary>
        /// <returns>True si está conectado</returns>
        public bool VerificarConexion()
        {
            try
            {
                if (_puntero == 0)
                {
                    _isConnected = false;
                    return false;
                }

                Int32 estado = _socketComm.CS_IsConnected(_puntero);
                _isConnected = (estado == 1);
                return _isConnected;
            }
            catch (Exception ex)
            {
                _lastError = $"Excepción en VerificarConexion: {ex.Message}";
                _isConnected = false;
                return false;
            }
        }

        /// <summary>
        /// Cierra la conexión correctamente
        /// </summary>
        public void Desconectar()
        {
            try
            {
                if (_puntero != 0 && _isConnected)
                {
                    _socketComm.CS_Knockout(_puntero);  // Notificar a la impresora
                }
            }
            catch (Exception ex)
            {
                // Ignorar errores al desconectar
                System.Diagnostics.Debug.WriteLine($"Error al hacer knockout: {ex.Message}");
            }
            finally
            {
                _isConnected = false;
            }
        }

        /// <summary>
        /// Libera los recursos de la conexión
        /// </summary>
        public void Dispose()
        {
            Desconectar();
            
            try
            {
                if (_puntero != 0)
                {
                    _socketComm.CS_Finish(_puntero);
                    _puntero = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al finalizar: {ex.Message}");
            }
        }
    }
}

