# 🎮 Controllers - Documentación de Controladores

## 📋 Índice
- [Descripción General](#descripción-general)
- [Controladores Disponibles](#controladores-disponibles)
- [Patrón MVC](#patrón-mvc)
- [Cómo Agregar un Nuevo Controlador](#cómo-agregar-un-nuevo-controlador)

---

## 📖 Descripción General

Los **Controllers** actúan como intermediarios entre las **Views** (formularios) y los **Services** (lógica de negocio). Su responsabilidad es coordinar las operaciones sin contener lógica de negocio compleja.

### Responsabilidades
- ✅ Recibir eventos de las Views
- ✅ Validar datos de entrada
- ✅ Coordinar llamadas a Services
- ✅ Actualizar Views con resultados
- ❌ NO contener lógica de negocio compleja
- ❌ NO acceder directamente a base de datos o hardware

---

## 📝 Controladores Disponibles

### 1. ProductionController.cs
**Namespace**: `LaserMacsaUser.Controllers`

**Propósito**: Controla el flujo de producción de impresión láser.

**Dependencias**:
- `ILaserService` - Comunicación con láser
- `IDatabaseService` - Acceso a base de datos
- `IQueueService` - Gestión de colas

**Métodos principales**:
```csharp
public void StartProduction(int artworkId, int quantity)
{
    // 1. Validar parámetros
    // 2. Obtener artwork de base de datos
    // 3. Inicializar láser
    // 4. Enviar archivo a láser
    // 5. Iniciar impresión
    // 6. Actualizar UI
}

public void StopProduction()
{
    // 1. Detener láser
    // 2. Actualizar estado
    // 3. Actualizar UI
}
```

**Uso desde View**:
```csharp
// En Form1.cs
private ProductionController _productionController;

private void BtnStart_Click(object sender, EventArgs e)
{
    _productionController.StartProduction(artworkId, quantity);
}
```

---

### 2. ArtworkController.cs
**Namespace**: `LaserMacsaUser.Controllers`

**Propósito**: Gestiona operaciones relacionadas con artworks.

**Métodos principales**:
```csharp
public List<Artwork> GetAllArtworks()
{
    // Obtener todos los artworks de la base de datos
}

public Artwork? GetArtworkById(int id)
{
    // Obtener artwork específico
}

public bool ActivateArtwork(int artworkId)
{
    // Activar un artwork en el láser
}
```

---

### 3. PromotionController.cs
**Namespace**: `LaserMacsaUser.Controllers`

**Propósito**: Gestiona promociones y cambios de artwork durante producción.

**Métodos principales**:
```csharp
public void ApplyPromotion(int artworkId, string promotionName)
{
    // 1. Validar promoción
    // 2. Confirmar con usuario
    // 3. Aplicar cambio
    // 4. Actualizar producción
}
```

---

## 🏛️ Patrón MVC

### Flujo Típico

```
┌─────────────┐
│    VIEW     │  Usuario hace clic
│  Form1.cs   │  ──────────────────┐
└──────┬──────┘                    │
       │                           │
       │ BtnStart_Click()          │
       ▼                           │
┌─────────────┐                    │
│ CONTROLLER  │  StartProduction() │
│ Production  │  ──────────────────┼──┐
│ Controller  │                    │  │
└──────┬──────┘                    │  │
       │                           │  │
       ├─→ LaserService            │  │
       ├─→ DatabaseService         │  │
       └─→ QueueService           │  │
       │                           │  │
       │ Resultados                │  │
       ▼                           │  │
┌─────────────┐                    │  │
│   SERVICE   │  Operaciones       │  │
│   Layer     │  ──────────────────┘  │
└─────────────┘                       │
                                      │
       ┌──────────────────────────────┘
       │
       │ Actualizar UI
       ▼
┌─────────────┐
│    VIEW     │  Mostrar resultado
│  Form1.cs   │  al usuario
└─────────────┘
```

---

## ➕ Cómo Agregar un Nuevo Controlador

### Paso 1: Crear el Archivo

```
Controllers/MyNewController.cs
```

### Paso 2: Estructura Básica

```csharp
using LaserMacsaUser.Services;
using LaserMacsaUser.Models;

namespace LaserMacsaUser.Controllers
{
    public class MyNewController
    {
        private readonly IMyService _myService;
        
        // Constructor con inyección de dependencias
        public MyNewController(IMyService myService)
        {
            _myService = myService ?? throw new ArgumentNullException(nameof(myService));
        }
        
        // Métodos públicos que coordinan operaciones
        public void DoSomething(int id)
        {
            // 1. Validar entrada
            if (id <= 0)
                throw new ArgumentException("ID debe ser mayor que 0");
            
            // 2. Llamar a servicio
            var result = _myService.ProcessData(id);
            
            // 3. Procesar resultado (si es necesario)
            // ...
        }
    }
}
```

### Paso 3: Registrar en la View

```csharp
// En Form1.cs o la View correspondiente
private MyNewController _myController;

public Form1()
{
    InitializeComponent();
    
    // Inicializar controlador con servicios
    var myService = new MyService();
    _myController = new MyNewController(myService);
}

private void BtnAction_Click(object sender, EventArgs e)
{
    _myController.DoSomething(artworkId);
}
```

---

## 📐 Convenciones

### Nombres
- Archivo: `[Nombre]Controller.cs`
- Clase: `[Nombre]Controller`
- Namespace: `LaserMacsaUser.Controllers`

### Inyección de Dependencias
- Usar interfaces de servicios (`ILaserService`, `IDatabaseService`)
- Inyectar dependencias en el constructor
- Validar que las dependencias no sean null

### Manejo de Errores
```csharp
public void SomeOperation(int id)
{
    try
    {
        // Operación
    }
    catch (Exception ex)
    {
        // Log error
        System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
        
        // Notificar a la View (opcional)
        // _view.ShowError(ex.Message);
        
        throw; // Re-lanzar o manejar según necesidad
    }
}
```

---

## 🔗 Referencias

- [README Principal](../README.md) - Arquitectura general
- [Services/README.md](../Services/README.md) - Servicios disponibles
- [Views/README.md](../Views/README.md) - Cómo usar desde Views

---

**Última actualización**: 2025-11-23

