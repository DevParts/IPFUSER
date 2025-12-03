# Script para compilar cerrando procesos previos si están bloqueando archivos
Write-Host "Verificando procesos de LaserMacsaUser..." -ForegroundColor Yellow

$processes = Get-Process -Name "LaserMacsaUser" -ErrorAction SilentlyContinue

if ($processes) {
    Write-Host "Se encontraron $($processes.Count) proceso(s) ejecutándose. Cerrándolos..." -ForegroundColor Yellow
    $processes | ForEach-Object {
        Write-Host "Cerrando proceso: $($_.Id)" -ForegroundColor Cyan
        Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
    }
    Start-Sleep -Seconds 2
    Write-Host "Procesos cerrados. Compilando..." -ForegroundColor Green
} else {
    Write-Host "No hay procesos ejecutándose. Compilando..." -ForegroundColor Green
}

# Compilar el proyecto
dotnet build LaserMacsaUser.csproj

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nCompilación exitosa!" -ForegroundColor Green
} else {
    Write-Host "`nError en la compilación." -ForegroundColor Red
}

