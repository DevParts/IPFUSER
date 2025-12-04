using System;
using System.Threading;
using System.Threading.Tasks;

namespace LaserMacsaUser.Common
{
    public enum RetryStrategy
    {
        Fixed,          // Delay fijo entre intentos
        Linear,         // Delay incrementa linealmente
        Exponential     // Delay incrementa exponencialmente (backoff)
    }

    public class RetryPolicy
    {
        public int MaxAttempts { get; set; } = 3;
        public int InitialDelayMs { get; set; } = 100;
        public int MaxDelayMs { get; set; } = 5000;
        public RetryStrategy Strategy { get; set; } = RetryStrategy.Exponential;
        
        // Determina si una excepción es recuperable
        public Func<Exception, bool>? IsRetryable { get; set; }

        public T Execute<T>(Func<T> operation)
        {
            int attempt = 0;
            Exception? lastException = null;

            while (attempt < MaxAttempts)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    attempt++;

                    // Verificar si es recuperable
                    if (IsRetryable != null && !IsRetryable(ex))
                    {
                        throw; // No reintentar si no es recuperable
                    }

                    // Si es el último intento, lanzar excepción
                    if (attempt >= MaxAttempts)
                    {
                        throw new RetryException(
                            $"Operación falló después de {MaxAttempts} intentos", 
                            lastException);
                    }

                    // Calcular delay según estrategia
                    int delay = CalculateDelay(attempt);
                    Thread.Sleep(delay);
                }
            }

            throw new RetryException(
                $"Operación falló después de {MaxAttempts} intentos", 
                lastException);
        }

        private int CalculateDelay(int attempt)
        {
            return Strategy switch
            {
                RetryStrategy.Fixed => InitialDelayMs,
                RetryStrategy.Linear => InitialDelayMs * attempt,
                RetryStrategy.Exponential => Math.Min(
                    InitialDelayMs * (int)Math.Pow(2, attempt - 1), 
                    MaxDelayMs),
                _ => InitialDelayMs
            };
        }
    }

    public class RetryException : Exception
    {
        public RetryException(string message, Exception? innerException) 
            : base(message, innerException) { }
    }
}