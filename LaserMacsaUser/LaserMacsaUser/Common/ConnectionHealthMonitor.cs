using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace LaserMacsaUser.Common
{
    public class ConnectionHealthMonitor
    {
        private readonly string _connectionString;
        private bool _isHealthy = true;
        private DateTime _lastCheck = DateTime.MinValue;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

        public event EventHandler<bool>? ConnectionStateChanged;

        public ConnectionHealthMonitor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool IsConnectionHealthy()
        {
            // Cachear resultado por un tiempo
            if (DateTime.Now - _lastCheck < _checkInterval)
            {
                return _isHealthy;
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();
                
                // Ejecutar consulta simple para verificar
                using var command = new SqlCommand("SELECT 1", connection);
                command.CommandTimeout = 5;
                command.ExecuteScalar();

                bool wasHealthy = _isHealthy;
                _isHealthy = true;
                _lastCheck = DateTime.Now;

                if (!wasHealthy)
                {
                    ConnectionStateChanged?.Invoke(this, true);
                }

                return true;
            }
            catch
            {
                bool wasHealthy = _isHealthy;
                _isHealthy = false;
                _lastCheck = DateTime.Now;

                if (wasHealthy)
                {
                    ConnectionStateChanged?.Invoke(this, false);
                }

                return false;
            }
        }

        public async Task<bool> ReconnectAsync(int maxAttempts = 3)
        {
            var retryPolicy = new RetryPolicy
            {
                MaxAttempts = maxAttempts,
                Strategy = RetryStrategy.Exponential,
                InitialDelayMs = 500,
                IsRetryable = ex => ex is SqlException || ex is InvalidOperationException
            };

            try
            {
                return retryPolicy.Execute(() =>
                {
                    using var connection = new SqlConnection(_connectionString);
                    connection.Open();
                    return true;
                });
            }
            catch
            {
                return false;
            }
        }
    }
}