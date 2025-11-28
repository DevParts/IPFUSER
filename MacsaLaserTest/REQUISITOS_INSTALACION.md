# Requisitos para Ejecutar MacsaLaserTest en Otras Computadoras Windows

Este documento describe todo lo necesario para ejecutar la aplicación `MacsaLaserTest` en otras computadoras Windows.

## 📋 Requisitos del Sistema

### 1. Sistema Operativo
- **Windows 7 SP1 o superior** (Windows 10/11 recomendado)
- Arquitectura: **32-bit (x86)** o **64-bit (x64)** - La aplicación está compilada para x86 pero funciona en ambos

### 2. .NET Framework
- **.NET Framework 4.7.2 o superior** (requerido)
- Puedes descargarlo desde: https://dotnet.microsoft.com/download/dotnet-framework/net472
- Para verificar si está instalado, ejecuta en PowerShell:
  ```powershell
  Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\" | Select-Object Release
  ```
  Si el valor es >= 461808, entonces tienes .NET Framework 4.7.2 o superior

### 3. Archivos DLL Requeridos

La aplicación necesita dos DLLs que deben estar en el mismo directorio que el ejecutable:

#### Archivos que se copian automáticamente:
- ✅ `SocketCommNet.dll` - DLL .NET (se copia automáticamente al compilar)
- ✅ `SocketCommDll.dll` - DLL nativa (se copia automáticamente al compilar)

**Ubicación de las DLLs en el proyecto:**
- `SocketCommNet.dll`: `..\TCPIPver31\libs\x86\SocketCommNet.dll`
- `SocketCommDll.dll`: `..\TCPIPver31\libs\x86\SocketCommDll.dll`

## 📦 Opciones de Distribución

### Opción 1: Distribuir el Ejecutable Compilado (Recomendado)

1. **Compilar el proyecto en modo Release:**
   ```powershell
   dotnet build MacsaLaserTest.csproj -c Release
   ```

2. **Copiar la carpeta completa `bin\Release\`** que contiene:
   - `MacsaLaserTest.exe`
   - `MacsaLaserTest.exe.config`
   - `SocketCommNet.dll`
   - `SocketCommDll.dll`
   - `MacsaLaserTest.pdb` (opcional, solo para debugging)

3. **En la computadora destino:**
   - Copia toda la carpeta `bin\Release\` a cualquier ubicación
   - Asegúrate de que .NET Framework 4.7.2 esté instalado
   - Ejecuta `MacsaLaserTest.exe`

### Opción 2: Compilar en la Computadora Destino

1. **Copiar todo el proyecto** (código fuente completo)
2. **Asegurar que las DLLs estén en la ruta correcta:**
   - Las DLLs deben estar en: `..\TCPIPver31\libs\x86\` (relativo al proyecto)
3. **Instalar Visual Studio** o **Build Tools para Visual Studio**
4. **Compilar el proyecto:**
   ```powershell
   dotnet build MacsaLaserTest.csproj
   ```

## 🔧 Verificación de Requisitos

### Script de Verificación (PowerShell)

Crea un archivo `verificar_requisitos.ps1`:

```powershell
Write-Host "Verificando requisitos para MacsaLaserTest..." -ForegroundColor Cyan

# Verificar .NET Framework
$netVersion = Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\" -ErrorAction SilentlyContinue
if ($netVersion -and $netVersion.Release -ge 461808) {
    Write-Host "✓ .NET Framework 4.7.2 o superior: INSTALADO" -ForegroundColor Green
} else {
    Write-Host "✗ .NET Framework 4.7.2 o superior: NO INSTALADO" -ForegroundColor Red
    Write-Host "  Descarga desde: https://dotnet.microsoft.com/download/dotnet-framework/net472" -ForegroundColor Yellow
}

# Verificar DLLs
$dlls = @("SocketCommNet.dll", "SocketCommDll.dll")
$allPresent = $true
foreach ($dll in $dlls) {
    if (Test-Path $dll) {
        Write-Host "✓ $dll: PRESENTE" -ForegroundColor Green
    } else {
        Write-Host "✗ $dll: NO ENCONTRADO" -ForegroundColor Red
        $allPresent = $false
    }
}

if ($allPresent) {
    Write-Host "`n✓ Todos los requisitos están cumplidos" -ForegroundColor Green
} else {
    Write-Host "`n✗ Faltan algunos requisitos" -ForegroundColor Red
}
```

## ⚙️ Configuración Necesaria

### Antes de Ejecutar

Edita el archivo de código fuente y cambia la IP de la impresora en los archivos de prueba:

- `ProgramInfoTest.cs` - línea 21
- `Test02_UserMessages.cs` - línea 21
- `Test03_ControlImpresion.cs` - línea 21
- `Test04_EnviarArchivos.cs` - línea 21
- `Test05_GestionarArchivos.cs` - línea 21
- `Test06_Contadores.cs` - línea 21
- `Test07_Configuracion.cs` - línea 21
- `Test08_Monitoreo.cs` - línea 21
- `Test09_Alarmas.cs` - línea 21

Cambia:
```csharp
string ipImpresora = "192.168.16.180";  // Cambia por la IP de tu impresora
```

## 🚀 Instalación Rápida (Checklist)

Para instalar en una nueva computadora Windows:

- [ ] Verificar que Windows 7 SP1 o superior esté instalado
- [ ] Instalar .NET Framework 4.7.2 o superior
- [ ] Copiar la carpeta `bin\Release\` completa
- [ ] Verificar que `SocketCommNet.dll` y `SocketCommDll.dll` estén presentes
- [ ] Configurar la IP de la impresora en el código (si es necesario)
- [ ] Ejecutar `MacsaLaserTest.exe`

## 🔍 Solución de Problemas

### Error: "No se puede cargar el archivo o ensamblado 'SocketCommNet'"
- **Solución:** Asegúrate de que `SocketCommNet.dll` esté en el mismo directorio que el ejecutable

### Error: "No se puede cargar la DLL 'SocketCommDll.dll'"
- **Solución:** 
  - Verifica que `SocketCommDll.dll` esté en el mismo directorio que el ejecutable
  - Asegúrate de que Visual C++ Redistributable esté instalado (si la DLL lo requiere)

### Error: "Esta aplicación requiere .NET Framework 4.7.2"
- **Solución:** Instala .NET Framework 4.7.2 desde el sitio oficial de Microsoft

### Error de conexión a la impresora
- **Solución:**
  - Verifica que la IP de la impresora sea correcta
  - Asegúrate de que la impresora esté encendida y en la red
  - Verifica que no haya firewall bloqueando la conexión
  - Comprueba la conectividad de red: `ping [IP_IMPRESORA]`

## 📝 Notas Adicionales

- La aplicación está compilada para **x86 (32-bit)**, pero funciona en sistemas 64-bit gracias a WOW64
- Si necesitas compilar para x64, cambia `<PlatformTarget>x86</PlatformTarget>` a `<PlatformTarget>x64</PlatformTarget>` en el archivo `.csproj`
- Las DLLs deben ser de la misma arquitectura (x86 o x64) que la aplicación

## 📞 Soporte

Si encuentras problemas al ejecutar la aplicación en otra computadora, verifica:
1. Versión de .NET Framework instalada
2. Presencia de todas las DLLs necesarias
3. Configuración de red y conectividad con la impresora
4. Permisos de ejecución (algunos antivirus pueden bloquear DLLs no firmadas)

