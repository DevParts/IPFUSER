# Common - Componentes de Infraestructura

Este directorio contiene componentes reutilizables que implementan funcionalidades avanzadas de manejo de errores y optimización de rendimiento para el sistema LaserMacsaUser.

## 📋 Componentes Implementados

### 1. **RetryPolicy.cs** - Sistema de Reintentos Automáticos

**Propósito:** Implementa un sistema de reintentos automáticos con diferentes estrategias para operaciones que pueden fallar temporalmente.

**Características:**
- ✅ Tres estrategias de reintento: Fixed, Linear, Exponential (backoff)
- ✅ Configuración de intentos máximos y delays
- ✅ Detección de excepciones recuperables vs no recuperables
- ✅ Lanza `RetryException` cuando se agotan los intentos

**Uso:**
```csharp
var retryPolicy = new RetryPolicy
{
    MaxAttempts = 3,
    Strategy = RetryStrategy.Exponential,
    InitialDelayMs = 100,
    MaxDelayMs = 5000,
    IsRetryable = ex => ex is SqlException || ex is InvalidOperationException
};

// Ejecutar operación con reintentos
var result = retryPolicy.Execute(() => 
{
    // Tu operación aquí
    return databaseService.GetDataTable(sql, "Table");
});
```

**Estrategias:**
- **Fixed:** Delay constante entre intentos (100ms, 100ms, 100ms...)
- **Linear:** Delay incrementa linealmente (100ms, 200ms, 300ms...)
- **Exponential:** Delay incrementa exponencialmente (100ms, 200ms, 400ms, 800ms...)

---

### 2. **ConnectionPool.cs** - Pool de Conexiones SQL

**Propósito:** Gestiona un pool de conexiones SQL Server para reutilizar conexiones y mejorar el rendimiento, evitando crear/destruir conexiones constantemente.

**Características:**
- ✅ Pool de conexiones reutilizables
- ✅ Tamaño mínimo y máximo configurable
- ✅ Validación automática de conexiones
- ✅ Timeout configurable para obtener conexiones
- ✅ Prevención de recursión infinita (StackOverflowException)
- ✅ Implementa `IDisposable` para limpieza adecuada

**Uso:**
```csharp
// Crear pool
var pool = new ConnectionPool(connectionString, minPoolSize: 2, maxPoolSize: 10);

// Obtener conexión (timeout por defecto: 30 segundos)
using var connection = pool.GetConnection();
// ... usar conexión ...

// Obtener conexión con timeout personalizado
using var connection2 = pool.GetConnection(TimeSpan.FromSeconds(10));

// Devolver conexión al pool (automático con using, o manual)
pool.ReturnConnection(connection);

// Limpiar recursos
pool.Dispose();
```

**Beneficios:**
- ⚡ Reduce overhead de crear/destruir conexiones
- ⚡ Mejora rendimiento en operaciones frecuentes
- ⚡ Control de recursos (límite máximo de conexiones)
- ⚡ Manejo robusto de errores y timeouts

**Nota Importante:** 
El método `GetConnection()` usa un bucle en lugar de recursión para evitar `StackOverflowException`. Tiene un límite de 100 reintentos y un timeout configurable.

---

### 3. **ConnectionHealthMonitor.cs** - Monitor de Salud de Conexiones

**Propósito:** Monitorea el estado de salud de las conexiones a la base de datos y permite reconexión automática.

**Características:**
- ✅ Verificación periódica del estado de conexión
- ✅ Cacheo de resultados para evitar verificaciones excesivas
- ✅ Evento `ConnectionStateChanged` para notificar cambios
- ✅ Método de reconexión automática con reintentos

**Uso:**
```csharp
var monitor = new ConnectionHealthMonitor(connectionString);

// Suscribirse a cambios de estado
monitor.ConnectionStateChanged += (sender, isHealthy) =>
{
    if (isHealthy)
        Console.WriteLine("Conexión restaurada");
    else
        Console.WriteLine("Conexión perdida");
};

// Verificar estado
bool isHealthy = monitor.IsConnectionHealthy();

// Intentar reconectar
bool reconnected = await monitor.ReconnectAsync(maxAttempts: 3);
```

**Intervalo de Verificación:** Por defecto verifica cada 30 segundos (configurable).

---

### 4. **CodeCache.cs** - Caché de Códigos

**Propósito:** Implementa un caché en memoria (LRU) para códigos frecuentes, reduciendo consultas a la base de datos.

**Características:**
- ✅ Caché LRU (Least Recently Used) con tamaño máximo configurable
- ✅ TTL (Time To Live) configurable por entrada
- ✅ Limpieza automática de entradas expiradas
- ✅ Separación por JobId (códigos de diferentes trabajos no se mezclan)
- ✅ Thread-safe con locks

**Uso:**
```csharp
// Crear caché (maxSize: 1000, TTL: 10 minutos por defecto)
var cache = new CodeCache(maxSize: 1000, ttl: TimeSpan.FromMinutes(10));

// Agregar código al caché
cache.Add("CODE123", "ABC123456", jobId: 1);

// Obtener código del caché
string? code = cache.Get("CODE123", jobId: 1);
if (code != null)
{
    // Código encontrado en caché
}
else
{
    // Código no está en caché, obtener de BD
}

// Limpiar caché completo
cache.Clear();

// Limpiar códigos de un job específico
cache.ClearByJobId(jobId: 1);
```

**Beneficios:**
- ⚡ Reduce consultas a BD para códigos frecuentes
- ⚡ Mejora tiempo de respuesta
- ⚡ Gestión automática de memoria (expiración y límite de tamaño)

**Limpieza Automática:** El caché se limpia automáticamente cada minuto, eliminando entradas expiradas.

---

## 🔗 Relación con Tareas del README Principal

Estos componentes implementan las siguientes tareas del README principal:

### Tarea 10: Manejo de Errores Avanzado
- ✅ **RetryPolicy.cs** → Sistema de reintentos automáticos
- ✅ **ConnectionHealthMonitor.cs** → Recuperación de errores de conexión
- ✅ **ConnectionPool.cs** → Manejo robusto de errores con timeouts

### Tarea 11: Optimizaciones de Rendimiento
- ✅ **ConnectionPool.cs** → Pool de conexiones
- ✅ **CodeCache.cs** → Caché de códigos frecuentes
- ✅ **ConnectionPool.cs** → Gestión mejorada de memoria (reutilización de conexiones)

---

## 📝 Notas de Implementación

### Thread Safety
- `ConnectionPool`: Usa `ConcurrentQueue` y `lock` para operaciones thread-safe
- `CodeCache`: Usa `lock` para todas las operaciones
- `RetryPolicy`: No es thread-safe por diseño (cada instancia se usa en un contexto específico)

### Gestión de Memoria
- Todos los componentes implementan `IDisposable` donde es necesario
- `ConnectionPool` limpia todas las conexiones al hacer dispose
- `CodeCache` tiene límite de tamaño y limpieza automática

### Manejo de Errores
- Todos los componentes lanzan excepciones descriptivas
- `RetryPolicy` envuelve errores en `RetryException`
- `ConnectionPool` lanza `InvalidOperationException` con detalles del estado

---

## 🚀 Próximos Pasos

Estos componentes están listos para ser integrados en:
- `DatabaseService.cs` → Usar `ConnectionPool` y `RetryPolicy`
- `LaserService.cs` → Usar `RetryPolicy` para operaciones de comunicación
- `QueueService.cs` → Usar `CodeCache` para códigos frecuentes

---

**Última actualización:** Diciembre 2024  
**Versión:** 1.0.0

