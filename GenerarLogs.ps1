$baseUrl = "http://localhost:5113"

Write-Host "============================================="
Write-Host "Generador de Logs para SEGG-U2-P4H-2"
Write-Host "============================================="
Write-Host "Asegúrate de que VulnerableApp se esté ejecutando en $baseUrl"
Write-Host "Generando registros, por favor espera..."
Write-Host ""

# 1. Generar log de Application
Write-Host "[1/4] Generando logs de 'Application' (Peticiones básicas)..."
Invoke-WebRequest -Uri "$baseUrl/" -UseBasicParsing -ErrorAction SilentlyContinue | Out-Null
Invoke-WebRequest -Uri "$baseUrl/Home/Privacy" -UseBasicParsing -ErrorAction SilentlyContinue | Out-Null
Start-Sleep -Seconds 1

# 2. Generar log de Audit (Búsqueda o Comentarios)
Write-Host "[2/4] Generando logs de 'Audit' (Comentarios y Búsquedas)..."
Invoke-WebRequest -Uri "$baseUrl/Comment" -UseBasicParsing -ErrorAction SilentlyContinue | Out-Null
Invoke-WebRequest -Uri "$baseUrl/Search?q=test" -UseBasicParsing -ErrorAction SilentlyContinue | Out-Null
Invoke-WebRequest -Uri "$baseUrl/api/users" -UseBasicParsing -ErrorAction SilentlyContinue | Out-Null
Start-Sleep -Seconds 1

# 3. Generar log de Security (Login fallido)
Write-Host "[3/4] Generando logs de 'Security' y 'authentication' (Fallo de login, Auth, Logout)..."
# POST a Auth/Login
try {
    Invoke-WebRequest -Uri "$baseUrl/Auth/Login" -Method Post -Body "Username=hacker&Password=badpassword" -ContentType "application/x-www-form-urlencoded" -UseBasicParsing -ErrorAction SilentlyContinue | Out-Null
} catch {}
# GET a Auth/Logout
Invoke-WebRequest -Uri "$baseUrl/Auth/Logout" -UseBasicParsing -ErrorAction SilentlyContinue | Out-Null
Start-Sleep -Seconds 1

# 4. Generar log de Orders (Reto)
Write-Host "[4/4] Generando logs de 'orders' (Reto)..."
Invoke-WebRequest -Uri "$baseUrl/Order" -UseBasicParsing -ErrorAction SilentlyContinue | Out-Null
Invoke-WebRequest -Uri "$baseUrl/Pedido" -UseBasicParsing -ErrorAction SilentlyContinue | Out-Null

Write-Host ""
Write-Host "============================================="
Write-Host "¡Listo! Ve a Grafana y busca:"
Write-Host ' - {category="Security"}'
Write-Host ' - {category="Audit"}'
Write-Host ' - {module="authentication"}'
Write-Host ' - {module="orders"}'
Write-Host "============================================="
