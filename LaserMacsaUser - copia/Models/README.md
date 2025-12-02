# 📦 Models - Documentación de Modelos

## 📋 Índice
- [Descripción General](#descripción-general)
- [Modelos Disponibles](#modelos-disponibles)
- [Convenciones](#convenciones)
- [Cómo Agregar un Nuevo Modelo](#cómo-agregar-un-nuevo-modelo)

---

## 📖 Descripción General

Los **Models** son clases simples que representan datos (POCOs - Plain Old CLR Objects). No contienen lógica de negocio, solo propiedades para almacenar y transportar datos entre capas.

### Características
- ✅ Propiedades simples (get/set)
- ✅ Representan entidades del dominio
- ✅ Fáciles de serializar
- ❌ NO contienen lógica de negocio
- ❌ NO acceden a base de datos directamente
- ❌ NO dependen de otras capas

---

## 📝 Modelos Disponibles

### 1. Artwork.cs
**Namespace**: `LaserMacsaUser.Models`

**Propósito**: Representa un artwork (diseño) que se puede imprimir.

**Propiedades**:
```csharp
public class Artwork
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FilePath { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}
```

**Uso**:
```csharp
var artwork = new Artwork
{
    Id = 1,
    Name = "Logo Empresa",
    FilePath = "C:\\Artworks\\logo.msf",
    CreatedDate = DateTime.Now,
    IsActive = true
};
```

---

### 2. LaserStatus.cs
**Namespace**: `LaserMacsaUser.Models`

**Propósito**: Representa el estado actual del láser.

**Propiedades**:
```csharp
public class LaserStatus
{
    public uint OkCounter { get; set; }        // Contador de impresiones OK
    public uint NokCounter { get; set; }       // Contador de impresiones NOK
    public uint TotalCounter { get; set; }     // Contador total
    public bool IsPrinting { get; set; }       // ¿Está imprimiendo?
    public int ErrorCode { get; set; }         // Código de error/alarma
    public string ActiveFileName { get; set; } // Archivo activo
    public uint Copies { get; set; }           // Copias a imprimir
}
```

**Uso**:
```csharp
var status = _laserService.GetStatus();
Console.WriteLine($"Imprimiendo: {status.IsPrinting}");
Console.WriteLine($"OK: {status.OkCounter}, NOK: {status.NokCounter}");
```

---

### 3. ProductionBatch.cs
**Namespace**: `LaserMacsaUser.Models`

**Propósito**: Representa un lote de producción.

**Propiedades**:
```csharp
public class ProductionBatch
{
    public int Id { get; set; }
    public int ArtworkId { get; set; }
    public int Quantity { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public BatchStatus Status { get; set; }
}

public enum BatchStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}
```

---

### 4. Promotion.cs
**Namespace**: `LaserMacsaUser.Models`

**Propósito**: Representa una promoción (cambio de artwork durante producción).

**Propiedades**:
```csharp
public class Promotion
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int FromArtworkId { get; set; }
    public int ToArtworkId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}
```

---

## 📐 Convenciones

### Nombres
- Archivo: `[Nombre].cs` (singular)
- Clase: `[Nombre]` (singular)
- Namespace: `LaserMacsaUser.Models`

### Estructura
```csharp
namespace LaserMacsaUser.Models
{
    public class MyModel
    {
        // Propiedades públicas con get/set
        public int Id { get; set; }
        public string Name { get; set; }
        
        // Propiedades calculadas (opcional)
        public string FullName => $"{FirstName} {LastName}";
        
        // Constructores (opcional)
        public MyModel() { }
        
        public MyModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
```

### Validación
Si necesitas validación, considera usar Data Annotations:

```csharp
using System.ComponentModel.DataAnnotations;

public class Artwork
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Required]
    [Url]
    public string FilePath { get; set; }
}
```

---

## ➕ Cómo Agregar un Nuevo Modelo

### Paso 1: Crear el Archivo

```
Models/MyNewModel.cs
```

### Paso 2: Estructura Básica

```csharp
namespace LaserMacsaUser.Models
{
    public class MyNewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
```

### Paso 3: Usar en Services/Controllers

```csharp
// En DatabaseService.cs
public MyNewModel? GetMyNewModel(int id)
{
    // Consulta a base de datos
    // Mapear a MyNewModel
    return new MyNewModel
    {
        Id = reader.GetInt32("Id"),
        Name = reader.GetString("Name"),
        // ...
    };
}

// En Controller
var model = _databaseService.GetMyNewModel(id);
```

---

## 🔄 Mapeo desde Base de Datos

### Ejemplo: Mapear desde DataReader

```csharp
public Artwork? MapArtworkFromReader(SqlDataReader reader)
{
    if (!reader.Read())
        return null;
    
    return new Artwork
    {
        Id = reader.GetInt32("Id"),
        Name = reader.GetString("Name"),
        FilePath = reader.GetString("FilePath"),
        CreatedDate = reader.GetDateTime("CreatedDate"),
        IsActive = reader.GetBoolean("IsActive")
    };
}
```

### Ejemplo: Mapear desde DataTable

```csharp
public List<Artwork> MapArtworksFromTable(DataTable table)
{
    var artworks = new List<Artwork>();
    
    foreach (DataRow row in table.Rows)
    {
        artworks.Add(new Artwork
        {
            Id = Convert.ToInt32(row["Id"]),
            Name = row["Name"].ToString(),
            FilePath = row["FilePath"].ToString(),
            CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
            IsActive = Convert.ToBoolean(row["IsActive"])
        });
    }
    
    return artworks;
}
```

---

## 🔗 Referencias

- [README Principal](../README.md) - Arquitectura general
- [Services/README.md](../Services/README.md) - Cómo usar modelos en servicios
- [Controllers/README.md](../Controllers/README.md) - Cómo usar modelos en controladores

---

**Última actualización**: 2025-11-23

