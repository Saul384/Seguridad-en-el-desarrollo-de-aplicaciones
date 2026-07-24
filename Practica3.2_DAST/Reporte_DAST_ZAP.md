# Reporte DAST OWASP ZAP - Práctica 3.2

## 4.1 Tabla de hallazgos (Riesgo High o Medium)

| # | Nombre del hallazgo | Riesgo ZAP | CWE | CVSS estimado | URL afectada | Evidencia (request/response) | Remediación propuesta |
|---|---------------------|------------|-----|---------------|--------------|------------------------------|-----------------------|
| 1 | SQL Injection | High | 89 | 9.8 (Crítico) | `http://vulnerable_app:8080/Search/Index` | `search=test' OR 1=1--` | Usar consultas parametrizadas (ej. con LINQ o parámetros SQL). |
| 2 | Cross Site Scripting (Reflected) | High | 79 | 6.1 (Medio) | `http://vulnerable_app:8080/Search/Index` | `search=<script>alert(1)</script>` | Usar `Html.Encode()` en las vistas. |
| 3 | Content Security Policy (CSP) Header Not Set | Medium | 693 | 4.3 (Medio) | `http://vulnerable_app:8080/Search/Index` | Cabeceras HTTP de la respuesta. | Configurar middleware para enviar CSP. |
| 4 | Missing Anti-clickjacking Header | Medium | 1021 | 4.3 (Medio) | `http://vulnerable_app:8080/` | Cabeceras HTTP de la respuesta. | Añadir la cabecera `X-Frame-Options: DENY`. |

## 4.2 Análisis de un hallazgo en detalle

**1. Nombre del hallazgo y tipo según OWASP Top 10:**
SQL Injection (Inyección SQL) - OWASP Top 10 A03:2021-Injection.

**2. Request HTTP exacto que ZAP envió para detectarlo:**
```http
GET /Search/Index?search=test%27%20OR%201%3D1-- HTTP/1.1
Host: vulnerable_app:8080
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64)
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
```

**3. Response HTTP que reveló la vulnerabilidad:**
```http
HTTP/1.1 200 OK
Content-Type: text/html; charset=utf-8
Server: Kestrel

...
<tr>
    <td>1</td>
    <td>admin</td>
</tr>
<tr>
    <td>2</td>
    <td>testuser</td>
</tr>
...
```

**4. Por qué esta respuesta confirma la vulnerabilidad:**
Al inyectar `' OR 1=1--`, la consulta en el servidor se convierte en `SELECT * FROM Users WHERE Username LIKE '%test' OR 1=1--%'`. La condición `OR 1=1` siempre es verdadera y el resto se comenta, lo que hace que la base de datos devuelva todos los registros de la tabla `Users`, revelando usuarios que no coinciden con la búsqueda original.

**5. Qué datos o accesos podría obtener un atacante real:**
Un atacante podría extraer información sensible de la base de datos completa, incluyendo contraseñas (hashes), datos personales, correos, y potencialmente obtener ejecución remota de comandos en el servidor de base de datos si tiene permisos suficientes.

**6. Código correcto que elimina la vulnerabilidad:**
```csharp
// Reemplazar la concatenación por LINQ parametrizado en SearchController.cs
var users = _db.Users
    .Where(u => u.Username.Contains(search))
    .ToList();
```

## 4.3 Verificar que las correcciones funcionan

Se modificó el código fuente en `SearchController.cs` reemplazando el `FromSqlRaw` concatenado por una consulta parametrizada usando LINQ, y se validó que las vistas de Razor realicen *encoding* automático. Al reconstruir el contenedor y volver a ejecutar el escaneo base (Baseline scan), los hallazgos de Inyección SQL y XSS desaparecen. Los hallazgos relacionados a las cabeceras (CSP y Clickjacking) persisten, ya que no se implementó un middleware para añadirlas en esta fase.
