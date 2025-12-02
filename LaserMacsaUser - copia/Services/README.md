# ⚙️ Services - Documentación de Servicios

## 📋 Índice
- [Descripción General](#descripción-general)
- [Servicios Disponibles](#servicios-disponibles)
- [Interfaces](#interfaces)
- [Cómo Agregar un Nuevo Servicio](#cómo-agregar-un-nuevo-servicio)

---

## 📖 Descripción General

Los **Services** contienen la lógica de negocio y la comunicación con sistemas externos (base de datos, hardware, APIs). Siguen el principio de responsabilidad única y proporcionan interfaces claras para su uso.

### Responsabilidades
- ✅ Lógica de negocio
- ✅ Comunicación con sistemas externos
- ✅ Validación de datos
- ✅ Manejo de errores
- ❌ NO conocer detalles de la UI
- ❌ NO depender de formularios específicos

---

## 📝 Servicios Disponibles

### 1. LaserService.cs
**Namespace**: `LaserMacsaUser.Services`

**Interfaz**: `ILaserService`

**Propósito**: Gestiona la comunicación con la impresora láser MACSA mediante TCP/IP.

**Dependencias**:
- `SocketCommNet` - Wrapper .NET para SocketCommDll.dll

**Métodos principales**:
```csharp
public bool Initialize(string ipAddress, string messagePath)
{
    // Inicializa conexión TCP/IP con el láser
    // - Crea instancia de SocketComm
    // - Llama a CS_Init()
    // - Llama a CS_StartClient()
    // - Inicia monitoreo de alarmas
}

public LaserStatus GetStatus()
{
    // Obtiene estado actual del láser
    // - Contadores
    // - Estado de impresión
    // - Códigos de alarma
}

public bool StartPrint(string filename, int copies)
{
    // Inicia impresión
    // - Valida parámetros
    // - Envía comando al láser
    // - Verifica respuesta
}
```

**Configuración de IP**:
```csharp
// La IP se lee desde Settings
string ip = Properties.Settings.Default.Laser_IP;
_laserService.Initialize(ip, ".\\");
```

**Eventos**:
```csharp
// Suscribirse a alarmas
_laserService.AlarmDetected += (sender, e) =>
{
    // Manejar alarma
    Console.WriteLine($"Alarma: {e.AlarmCode}");
};
```

**Ver documentación técnica**: [TCPIPver31/README.md](../../TCPIPver31/README.md)

---

### 2. DatabaseService.cs
**Namespace**: `LaserMacsaUser.Services`

**Interfaz**: `IDatabaseService`

**Propósito**: Gestiona el acceso a la base de datos SQL Server.

**Métodos principales**:
```csharp
public DataTable ExecuteQuery(string query)
{
    // Ejecuta consulta SELECT y retorna DataTable
}

public int ExecuteNonQuery(string query)
{
    // Ejecuta INSERT, UPDATE, DELETE
    // Retorna número de filas afectadas
}

public Artwork? GetArtworkById(int id)
{
    // Obtiene artwork de la base de datos
}
```

**Configuración de conexión**:
```csharp
// Se configura desde App.config o Settings
var connectionString = "Server=...;Database=...;...";
_databaseService = new DatabaseService(connectionString);
```

---

### 3. LaserAlarmService.cs
**Namespace**: `LaserMacsaUser.Services`

**Propósito**: Procesa y categoriza códigos de alarma del láser.

**Métodos principales**:
```csharp
public static List<LaserAlarm> ProcessLaserStatus(LaserStatus status)
{
    // Analiza el código de error del láser
    // Categoriza alarmas por tipo
    // Retorna lista de alarmas activas
}

public static string GetAlarmDescription(int alarmCode)
{
    // Obtiene descripción legible de un código de alarma
}
```

**Uso**:
```csharp
var status = _laserService.GetStatus();
var alarms = LaserAlarmService.ProcessLaserStatus(status);

foreach (var alarm in alarms)
{
    Console.WriteLine($"{alarm.Type}: {alarm.Description}");
}
```

---

### 4. QueueService.cs
**Namespace**: `LaserMacsaUser.Services`

**Interfaz**: `IQueueService`

**Propósito**: Gestiona colas de producción y trabajos pendientes.

**Métodos principales**:
```csharp
public void AddToQueue(ProductionBatch batch)
{
    // Agrega lote a la cola de producción
}

public ProductionBatch? GetNextBatch()
{
    // Obtiene siguiente lote pendiente
}

public void MarkBatchComplete(int batchId)
{
    // Marca lote como completado
}
```

---

### 5. SpeedwayService.cs
**Namespace**: `LaserMacsaUser.Services`

**Interfaz**: `ISpeedwayService`

**Propósito**: Gestiona comunicación con lector RFID Speedway.

**Métodos principales**:
```csharp
public bool Connect(string ipAddress, int port)
{
    // Conecta al lector RFID
}

public string? ReadTag()
{
    // Lee tag RFID
}
```

---

## 🔌 Interfaces

Todas las interfaces están en el mismo namespace y siguen el patrón `I[Nombre]Service`:

### ILaserService.cs
```csharp
public interface ILaserService
{
    bool IsConnected { get; }
    event EventHandler<LaserAlarmEventArgs>? AlarmDetected;
    
    bool Initialize(string ipAddress, string messagePath);
    LaserStatus GetStatus();
    bool StartPrint(string filename, int copies);
    bool StopPrint();
    void Dispose();
}
```

### IDatabaseService.cs
```csharp
public interface IDatabaseService
{
    DataTable ExecuteQuery(string query);
    int ExecuteNonQuery(string query);
    Artwork? GetArtworkById(int id);
}
```

**Ventajas de usar interfaces**:
- Facilita testing (mocks)
- Permite cambiar implementaciones
- Mejora desacoplamiento

---

## ➕ Cómo Agregar un Nuevo Servicio

### Paso 1: Crear la Interfaz

```
Services/IMyNewService.cs
```

```csharp
namespace LaserMacsaUser.Services
{
    public interface IMyNewService
    {
        bool DoSomething(int id);
        string GetData();
    }
}
```

### Paso 2: Crear la Implementación

```
Services/MyNewService.cs
```

```csharp
using System;

namespace LaserMacsaUser.Services
{
    public class MyNewService : IMyNewService
    {
        private readonly IDatabaseService _databaseService;
        
        public MyNewService(IDatabaseService databaseService)
        {
            _databaseService = databaseService ?? 
                throw new ArgumentNullException(nameof(databaseService));
        }
        
        public bool DoSomething(int id)
        {
            try
            {
                // Lógica de negocio
                // Acceso a base de datos
                // Comunicación externa
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        
        public string GetData()
        {
            // Implementación
            return "Data";
        }
    }
}
```

### Paso 3: Usar en Controller

```csharp
// En ProductionController.cs
private readonly IMyNewService _myService;

public ProductionController(IMyNewService myService)
{
    _myService = myService;
}

public void SomeOperation()
{
    var result = _myService.DoSomething(id);
    // ...
}
```

---

## 📐 Convenciones

### Nombres
- Interfaz: `I[Nombre]Service.cs`
- Implementación: `[Nombre]Service.cs`
- Namespace: `LaserMacsaUser.Services`

### Manejo de Errores
```csharp
public bool SomeOperation()
{
    try
    {
        // Operación
        return true;
    }
    catch (SpecificException ex)
    {
        // Log específico
        System.Diagnostics.Debug.WriteLine($"Specific error: {ex.Message}");
        return false;
    }
    catch (Exception ex)
    {
        // Log genérico
        System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
        throw; // Re-lanzar si es crítico
    }
}
```

### Logging
```csharp
// Usar System.Diagnostics.Debug para desarrollo
System.Diagnostics.Debug.WriteLine($"Operation started: {parameter}");

// Para producción, considerar usar un framework de logging
// (NLog, Serilog, etc.)
```

---

## 🔗 Referencias

- [README Principal](../README.md) - Arquitectura general
- [Controllers/README.md](../Controllers/README.md) - Cómo usar servicios
- [TCPIPver31/README.md](../../TCPIPver31/README.md) - Documentación técnica de TCP/IP

---

**Última actualización**: 2025-11-23

