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

Write-Host "Iniciando generación de tráfico para Seq..." -ForegroundColor Cyan

# 1. 100 búsquedas
Write-Host "Generando 100 búsquedas..." -ForegroundColor Yellow
for ($i = 1; $i -le 100; $i++) {
    $search = "busqueda_$i"
    Invoke-WebRequest -Uri "$baseUrl/Search/Index?search=$search" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}

# 2. 50 logins válidos (usando admin/admin o user1/user1 que existen en la BD)
Write-Host "Generando 50 logins válidos..." -ForegroundColor Yellow
for ($i = 1; $i -le 50; $i++) {
    $token = Get-AntiForgeryToken "$baseUrl/Auth/Login"
    $body = @{
        username = "admin"
        password = "admin"
        __RequestVerificationToken = $token
    }
    Invoke-WebRequest -Uri "$baseUrl/Auth/Login" -Method Post -Body $body -WebSession $session -ErrorAction SilentlyContinue | Out-Null
    # Logout for next iteration
    Invoke-WebRequest -Uri "$baseUrl/Auth/Logout" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}

# 3. 100 logins inválidos
Write-Host "Generando 100 logins inválidos..." -ForegroundColor Yellow
for ($i = 1; $i -le 100; $i++) {
    $token = Get-AntiForgeryToken "$baseUrl/Auth/Login"
    $body = @{
        username = "usuario_invalido"
        password = "badpassword_$i"
        __RequestVerificationToken = $token
    }
    Invoke-WebRequest -Uri "$baseUrl/Auth/Login" -Method Post -Body $body -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}

# 4. 30 SQL Injection
Write-Host "Generando 30 SQL Injection..." -ForegroundColor Yellow
$sqliPayloads = @("' OR '1'='1", "admin' --", "' UNION SELECT * FROM Users--", "'; DROP TABLE Users--")
for ($i = 1; $i -le 30; $i++) {
    $payload = $sqliPayloads[$i % $sqliPayloads.Count]
    $encoded = [uri]::EscapeDataString($payload)
    Invoke-WebRequest -Uri "$baseUrl/Search/Index?search=$encoded" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}

# 5. 30 XSS
Write-Host "Generando 30 XSS..." -ForegroundColor Yellow
$xssPayloads = @("<script>alert('XSS')</script>", "<img src=x onerror=alert(1)>", "<svg/onload=alert('XSS')>")
for ($i = 1; $i -le 30; $i++) {
    $token = Get-AntiForgeryToken "$baseUrl/Comment"
    $payload = $xssPayloads[$i % $xssPayloads.Count]
    $body = @{
        comment = $payload
        __RequestVerificationToken = $token
    }
    Invoke-WebRequest -Uri "$baseUrl/Comment/AddComment" -Method Post -Body $body -WebSession $session -ErrorAction SilentlyContinue | Out-Null
}

# 6. 200 consultas API
Write-Host "Generando 200 consultas API..." -ForegroundColor Yellow
# First login to get access
$token = Get-AntiForgeryToken "$baseUrl/Auth/Login"
$body = @{
    username = "admin"
    password = "admin"
    __RequestVerificationToken = $token
}
Invoke-WebRequest -Uri "$baseUrl/Auth/Login" -Method Post -Body $body -WebSession $session -ErrorAction SilentlyContinue | Out-Null

for ($i = 1; $i -le 200; $i++) {
    # Mezclamos peticiones válidas, inválidas (IDOR) y a la lista general
    if ($i % 3 -eq 0) {
        Invoke-WebRequest -Uri "$baseUrl/api/users" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
    } elseif ($i % 3 -eq 1) {
        Invoke-WebRequest -Uri "$baseUrl/api/user/1" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
    } else {
        # Intento IDOR - el admin es el ID 1, acceder al ID 2 dará Forbid (pero generará logs)
        Invoke-WebRequest -Uri "$baseUrl/api/user/2" -WebSession $session -ErrorAction SilentlyContinue | Out-Null
    }
}

Write-Host "Tráfico generado con éxito. Revisa Seq en http://localhost:5341" -ForegroundColor Green
