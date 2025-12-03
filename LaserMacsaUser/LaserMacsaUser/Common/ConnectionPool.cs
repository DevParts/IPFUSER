using System;
using System.Collections.Concurrent;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading;

namespace LaserMacsaUser.Common
{
    public class ConnectionPool : IDisposable
    {
        private readonly ConcurrentQueue<SqlConnection> _availableConnections;
        private readonly string _connectionString;
        private readonly int _minPoolSize;
        private readonly int _maxPoolSize;
        private int _currentPoolSize;
        private readonly object _lockObject = new object();
        private bool _disposed = false;

        public ConnectionPool(
            string connectionString,
            int minPoolSize = 2,
            int maxPoolSize = 10)
        {
            _connectionString = connectionString;
            _minPoolSize = minPoolSize;
            _maxPoolSize = maxPoolSize;
            _availableConnections = new ConcurrentQueue<SqlConnection>();
            _currentPoolSize = 0;
            // inicializar pool minimo
            InitializePool();
        }
        private void InitializePool(){
            for (int i = 0; i< _minPoolSize; i++){
                var connection = CreateConnection();
                if ( connection != null){
                    _availableConnections.Enqueue(connection);
                    Interlocked.Increment(ref _currentPoolSize);
                }
            }
        }
        public SqlConnection GetConnection()
        {
            return GetConnection(TimeSpan.FromSeconds(30));
        }

        public SqlConnection GetConnection(TimeSpan timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ConnectionPool));
            }

            DateTime startTime = DateTime.Now;
            int maxRetries = 100; // Máximo de reintentos para evitar recursión infinita
            int retryCount = 0;

            while (retryCount < maxRetries && (DateTime.Now - startTime) < timeout)
            {
                // Intentar obtener conexion del pool
                if (_availableConnections.TryDequeue(out SqlConnection? connection))
                {
                    //verificar que la conexion este valida
                    if (IsConnectionValid(connection))
                    {
                        return connection;
                    }
                    else
                    {
                        //conexion invalida crear nueva
                        connection?.Dispose();
                        Interlocked.Decrement(ref _currentPoolSize);
                    }
                }

                // no hay conexiones disponibles, crear nueva si no excede maximo
                lock (_lockObject)
                {
                    if (_currentPoolSize < _maxPoolSize)
                    {
                        connection = CreateConnection();
                        if (connection != null)
                        {
                            Interlocked.Increment(ref _currentPoolSize);
                            return connection;
                        }
                    }
                }

                // Si llegamos aquí, esperar un poco y reintentar
                retryCount++;
                Thread.Sleep(50);
            }

            // Si no se pudo obtener conexión después de todos los intentos, lanzar excepción
            throw new InvalidOperationException(
                $"No se pudo obtener una conexión del pool después de {retryCount} intentos " +
                $"o {timeout.TotalSeconds} segundos. Pool size: {_currentPoolSize}/{_maxPoolSize}, " +
                $"Available: {_availableConnections.Count}");
        }

        public void ReturnConnection(SqlConnection connection){
            if (_disposed || connection == null) return;

            // verificar que la conexion este valida antes de devolverla
            if (IsConnectionValid(connection)){
                _availableConnections.Enqueue(connection);
            }
            else {
                // conexion invalida, descartarla
                connection.Dispose();
                Interlocked.Decrement(ref _currentPoolSize);
            }
        }
        private SqlConnection CreateConnection()
        {
            try
            {
                var connection = new SqlConnection(_connectionString);
                connection.Open();
                return connection;
            }
            catch
            {
                return null!;
            }
        }
        private bool IsConnectionValid(SqlConnection connection)
        {
            try{
                return connection != null && connection.State == ConnectionState.Open;
            }catch{
                return false;
            }
        }
        public void Dispose(){
            if(_disposed) return;
            _disposed = true;
            while (_availableConnections.TryDequeue(out SqlConnection? connection))
            {
                connection?.Dispose();
            }
            Interlocked.Exchange(ref _currentPoolSize, 0);
        }
    }
}