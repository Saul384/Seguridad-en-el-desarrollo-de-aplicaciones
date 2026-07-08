$baseUrl = "http://localhost:5113"
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

function Get-AntiForgeryToken {
    param([string]$url)
    $response = Invoke-WebRequest -Uri $url -WebSession $session -ErrorAction SilentlyContinue
    if ($response.Content -match 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"') {
        return $matches[1]
    }
    return $null
}

Write-Host "Iniciando Pruebas Obligatorias para P3G-5..." -ForegroundColor Cyan

# --- Navegación General ---
Write-Host "1. Navegación General (30 Index, todos los controladores)" -ForegroundColor Yellow
for ($i = 1; $i -le 30; $i++) {
    Invoke-WebRequest -Uri "$baseUrl/" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
    Invoke-WebRequest -Uri "$baseUrl/Home" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}
# Acceder a todos los controladores al menos una vez para generar CorrelationId
Invoke-WebRequest -Uri "$baseUrl/Search" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
Invoke-WebRequest -Uri "$baseUrl/Comment" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
Invoke-WebRequest -Uri "$baseUrl/Auth/Login" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
Invoke-WebRequest -Uri "$baseUrl/api/users" -WebSession $session -ErrorAction SilentlyContinue | Out-Null

# --- Búsquedas ---
Write-Host "2. Búsquedas (100 válidas, 20 vacías, 20 especiales, 20 SQLi)" -ForegroundColor Yellow
for ($i = 1; $i -le 100; $i++) { Invoke-WebRequest -Uri "$baseUrl/Search/Index?search=valida_$i" -WebSession $session -ErrorAction SilentlyContinue | Out-Null }
for ($i = 1; $i -le 20; $i++) { Invoke-WebRequest -Uri "$baseUrl/Search/Index?search=" -WebSession $session -ErrorAction SilentlyContinue | Out-Null }
for ($i = 1; $i -le 20; $i++) { Invoke-WebRequest -Uri "$baseUrl/Search/Index?search=%40%23%24%25" -WebSession $session -ErrorAction SilentlyContinue | Out-Null }
for ($i = 1; $i -le 20; $i++) { Invoke-WebRequest -Uri "$baseUrl/Search/Index?search=' OR '1'='1" -WebSession $session -ErrorAction SilentlyContinue | Out-Null }

# --- Autenticación ---
Write-Host "3. Autenticación (50 válidos, 100 inválidos, usuarios inexistentes)" -ForegroundColor Yellow
for ($i = 1; $i -le 50; $i++) {
    $token = Get-AntiForgeryToken "$baseUrl/Auth/Login"
    $body = @{ username = "admin"; password = "admin"; __RequestVerificationToken = $token }
    Invoke-WebRequest -Uri "$baseUrl/Auth/Login" -Method Post -Body $body -WebSession $session -ErrorAction SilentlyContinue | Out-Null
    Invoke-WebRequest -Uri "$baseUrl/Auth/Logout" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}
for ($i = 1; $i -le 100; $i++) {
    $token = Get-AntiForgeryToken "$baseUrl/Auth/Login"
    $body = @{ username = "admin"; password = "badpassword_$i"; __RequestVerificationToken = $token }
    Invoke-WebRequest -Uri "$baseUrl/Auth/Login" -Method Post -Body $body -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}
for ($i = 1; $i -le 20; $i++) {
    $token = Get-AntiForgeryToken "$baseUrl/Auth/Login"
    $body = @{ username = "noexiste_$i"; password = "123"; __RequestVerificationToken = $token }
    Invoke-WebRequest -Uri "$baseUrl/Auth/Login" -Method Post -Body $body -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}

# --- Comentarios ---
Write-Host "4. Comentarios (100 válidos, 30 XSS, advertencias por vacíos)" -ForegroundColor Yellow
for ($i = 1; $i -le 100; $i++) {
    $token = Get-AntiForgeryToken "$baseUrl/Comment"
    $body = @{ comment = "Comentario valido $i"; __RequestVerificationToken = $token }
    Invoke-WebRequest -Uri "$baseUrl/Comment/AddComment" -Method Post -Body $body -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}
for ($i = 1; $i -le 30; $i++) {
    $token = Get-AntiForgeryToken "$baseUrl/Comment"
    $body = @{ comment = "<script>alert('XSS $i')</script>"; __RequestVerificationToken = $token }
    Invoke-WebRequest -Uri "$baseUrl/Comment/AddComment" -Method Post -Body $body -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}
# Advertencias por comentarios vacíos
for ($i = 1; $i -le 20; $i++) {
    $token = Get-AntiForgeryToken "$baseUrl/Comment"
    $body = @{ comment = ""; __RequestVerificationToken = $token }
    Invoke-WebRequest -Uri "$baseUrl/Comment/AddComment" -Method Post -Body $body -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}

# --- API ---
Write-Host "5. API (200 endpoints, 404, IDs inválidos, IDOR)" -ForegroundColor Yellow
# Login previo para acceder a la API
$token = Get-AntiForgeryToken "$baseUrl/Auth/Login"
$body = @{ username = "admin"; password = "admin"; __RequestVerificationToken = $token }
Invoke-WebRequest -Uri "$baseUrl/Auth/Login" -Method Post -Body $body -WebSession $session -ErrorAction SilentlyContinue | Out-Null

for ($i = 1; $i -le 150; $i++) { Invoke-WebRequest -Uri "$baseUrl/api/users" -WebSession $session -ErrorAction SilentlyContinue | Out-Null }
for ($i = 1; $i -le 50; $i++) { Invoke-WebRequest -Uri "$baseUrl/api/user/1" -WebSession $session -ErrorAction SilentlyContinue | Out-Null }
for ($i = 1; $i -le 20; $i++) { Invoke-WebRequest -Uri "$baseUrl/api/user/9999" -WebSession $session -ErrorAction SilentlyContinue | Out-Null } # IDOR/404
for ($i = 1; $i -le 20; $i++) { Invoke-WebRequest -Uri "$baseUrl/api/recurso_inexistente" -WebSession $session -ErrorAction SilentlyContinue | Out-Null } # 404

# --- Excepciones y Warnings extras ---
Write-Host "6. Excepciones No Controladas (Middleware) y Controladas" -ForegroundColor Yellow
# Excepciones no controladas (las inyecciones SQL que fallan en base de datos ya lo provocaron en Búsquedas)
# Añadiremos unas más solo para estar seguros
for ($i = 1; $i -le 10; $i++) { Invoke-WebRequest -Uri "$baseUrl/Search/Index?search='; DROP TABLE Users--" -WebSession $session -ErrorAction SilentlyContinue | Out-Null }

Write-Host "Pruebas de la Práctica 5 finalizadas." -ForegroundColor Green
