# Exceptions - Excepciones Personalizadas

Este directorio contiene excepciones personalizadas que proporcionan mensajes de error más informativos y contextuales para facilitar el diagnóstico y resolución de problemas.

## 📋 Excepciones Implementadas

### 1. **DatabaseConnectionException.cs** - Error de Conexión a Base de Datos

**Propósito:** Excepción especializada para errores de conexión a SQL Server con información detallada y sugerencias de solución.

**Características:**
- ✅ Información del servidor y base de datos
- ✅ Código de error único generado automáticamente
- ✅ Mensaje formateado con sugerencias de solución
- ✅ Preserva la excepción original como `InnerException`

**Propiedades:**
```csharp
public string DataSource { get; }      // Servidor SQL (ej: "(local)\\SQLEXPRESS")
public string Database { get; }        // Nombre de la base de datos
public string ErrorCode { get; }       // Código único (ej: "DB_CONN_20241215143022")
```

**Uso:**
```csharp
try
{
    using var connection = new SqlConnection(connectionString);
    connection.Open();
}
catch (SqlException ex)
{
    throw new DatabaseConnectionException(
        dataSource: "(local)\\SQLEXPRESS",
        database: "IPFEu",
        message: ex.Message,
        innerException: ex
    );
}
```

**Mensaje de Error Generado:**
```
Error de conexión a base de datos.
Servidor: (local)\SQLEXPRESS
Base de datos: IPFEu
Detalle: [mensaje de error SQL]

Sugerencias:
1. Verificar que SQL Server esté ejecutándose
2. Verificar credenciales de acceso
3. Verificar que la base de datos exista
4. Verificar firewall y permisos de red
```

**Código de Error:** Formato `DB_CONN_YYYYMMDDHHMMSS` para facilitar búsqueda en logs.

---

### 2. **LaserCommunicationException.cs** - Error de Comunicación con Láser

**Propósito:** Excepción especializada para errores de comunicación TCP/IP con el láser, incluyendo información de la operación y código de error.

**Características:**
- ✅ Información de IP del láser y operación que falló
- ✅ Código de error numérico y string formateado
- ✅ Mensaje con sugerencias específicas para problemas de láser
- ✅ Preserva la excepción original como `InnerException`

**Propiedades:**
```csharp
public string LaserIP { get; }         // IP del láser (ej: "192.168.0.180")
public string Operation { get; }       // Operación que falló (ej: "Initialize", "SendUserMessage")
public int ErrorCode { get; }          // Código de error numérico
public string ErrorCodeString { get; } // Código formateado (ej: "LASER_0001")
```

**Uso:**
```csharp
try
{
    int result = _socketComm.CS_StartClient(_socketHandle);
    if (result != 0)
    {
        string error = string.Empty;
        _socketComm.CS_GetLastError(_socketHandle, ref error);
        
        throw new LaserCommunicationException(
            laserIP: "192.168.0.180",
            operation: "Initialize",
            errorCode: result,
            errorMessage: error
        );
    }
}
catch (LaserCommunicationException ex)
{
    Console.WriteLine($"Error: {ex.ErrorCodeString} - {ex.Message}");
    // Acceder a propiedades específicas
    Console.WriteLine($"Láser IP: {ex.LaserIP}");
    Console.WriteLine($"Operación: {ex.Operation}");
}
```

**Mensaje de Error Generado:**
```
Error de comunicación con láser.
IP del láser: 192.168.0.180
Operación: Initialize
Código de error: 1
Mensaje: [mensaje de error del láser]

Sugerencias:
1. Verificar que el láser esté encendido y conectado
2. Verificar conectividad de red (ping 192.168.0.180)
3. Verificar que el puerto TCP esté abierto
4. Reiniciar la conexión del láser
```

**Código de Error:** Formato `LASER_####` donde `####` es el código de error con padding de ceros.

---

### 3. **RetryException.cs** - Error de Reintentos Agotados

**Ubicación:** `LaserMacsaUser/Common/RetryPolicy.cs` (clase interna)

**Propósito:** Excepción lanzada cuando una operación falla después de agotar todos los reintentos configurados en `RetryPolicy`.

**Características:**
- ✅ Indica cuántos intentos se realizaron
- ✅ Preserva la última excepción como `InnerException`
- ✅ Mensaje descriptivo del fallo

**Uso:**
```csharp
try
{
    var result = retryPolicy.Execute(() => SomeOperation());
}
catch (RetryException ex)
{
    Console.WriteLine($"Operación falló después de todos los reintentos");
    Console.WriteLine($"Último error: {ex.InnerException?.Message}");
}
```

---

## 🔗 Integración con Servicios

### DatabaseService
```csharp
// Reemplazar excepciones genéricas
catch (SqlException ex)
{
    throw new DatabaseConnectionException(
        _dataSource, 
        _dbName, 
        ex.Message, 
        ex
    );
}
```

### LaserService
```csharp
// Reemplazar excepciones genéricas
if (result != 0)
{
    string error = string.Empty;
    _socketComm.CS_GetLastError(_socketHandle, ref error);
    
    throw new LaserCommunicationException(
        ipAddress,
        "Initialize",
        result,
        error
    );
}
```

---

## 📊 Comparación: Antes vs Después

### Antes (Excepciones Genéricas)
```csharp
catch (Exception ex)
{
    throw new Exception($"Error al conectar: {ex.Message}");
}
```
**Problemas:**
- ❌ Mensaje genérico sin contexto
- ❌ No hay sugerencias de solución
- ❌ Difícil de diagnosticar
- ❌ No hay códigos de error para búsqueda

### Después (Excepciones Personalizadas)
```csharp
catch (SqlException ex)
{
    throw new DatabaseConnectionException(
        dataSource, 
        database, 
        ex.Message, 
        ex
    );
}
```
**Beneficios:**
- ✅ Mensaje contextual con información relevante
- ✅ Sugerencias de solución incluidas
- ✅ Códigos de error únicos para búsqueda
- ✅ Facilita diagnóstico y soporte técnico

---

## 🎯 Relación con Tareas del README Principal

### Tarea 10: Manejo de Errores Avanzado
- ✅ **Mensajes de error más informativos** → Implementado con excepciones personalizadas
- ✅ **Contexto completo** → Cada excepción incluye información relevante
- ✅ **Sugerencias de solución** → Mensajes incluyen pasos para resolver problemas

---

## 📝 Mejores Prácticas

### 1. Siempre Preservar InnerException
```csharp
// ✅ Correcto
throw new DatabaseConnectionException(..., innerException: ex);

// ❌ Incorrecto
throw new DatabaseConnectionException(..., innerException: null);
```

### 2. Proporcionar Información Contextual
```csharp
// ✅ Correcto - Incluye IP, operación, código
throw new LaserCommunicationException(ip, "SendUserMessage", errorCode, error);

// ❌ Incorrecto - Información genérica
throw new Exception("Error de comunicación");
```

### 3. Usar Códigos de Error para Búsqueda
```csharp
// Los códigos de error permiten buscar en logs
catch (DatabaseConnectionException ex)
{
    LogError($"Error {ex.ErrorCode}: {ex.Message}");
    // Buscar en logs: DB_CONN_20241215143022
}
```

---

## 🚀 Próximos Pasos

Estas excepciones están listas para ser integradas en:
- `DatabaseService.cs` → Reemplazar `Exception` genéricas por `DatabaseConnectionException`
- `LaserService.cs` → Reemplazar `Exception` genéricas por `LaserCommunicationException`
- `QueueService.cs` → Usar excepciones apropiadas según el contexto

---

**Última actualización:** Diciembre 2024  
**Versión:** 1.0.0

