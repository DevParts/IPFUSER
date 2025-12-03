using System;

namespace LaserMacsaUser.Exceptions
{
    public class LaserCommunicationException : Exception
    {
        public string LaserIP { get; }
        public string Operation { get; }
        public int ErrorCode { get; }
        public string ErrorCodeString { get; }

        public LaserCommunicationException(
            string laserIP, 
            string operation, 
            int errorCode, 
            string errorMessage,
            Exception? innerException = null) 
            : base(FormatMessage(laserIP, operation, errorCode, errorMessage), innerException)
        {
            LaserIP = laserIP;
            Operation = operation;
            ErrorCode = errorCode;
            ErrorCodeString = $"LASER_{errorCode:0000}";
        }

        private static string FormatMessage(
            string laserIP, 
            string operation, 
            int errorCode, 
            string errorMessage)
        {
            return $"Error de comunicación con láser.\n" +
                   $"IP del láser: {laserIP}\n" +
                   $"Operación: {operation}\n" +
                   $"Código de error: {errorCode}\n" +
                   $"Mensaje: {errorMessage}\n\n" +
                   $"Sugerencias:\n" +
                   $"1. Verificar que el láser esté encendido y conectado\n" +
                   $"2. Verificar conectividad de red (ping {laserIP})\n" +
                   $"3. Verificar que el puerto TCP esté abierto\n" +
                   $"4. Reiniciar la conexión del láser";
        }
    }
}