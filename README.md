# VulnerableApp - Rama Secure

## Descripción

VulnerableApp es una aplicación desarrollada en ASP.NET Core utilizada con fines educativos para el análisis y remediación de vulnerabilidades web incluidas dentro del OWASP Top 10.

La rama **secure** contiene la versión corregida de la aplicación, donde se implementaron controles de seguridad para mitigar diversas vulnerabilidades identificadas durante las prácticas de laboratorio.

---

## Vulnerabilidades Corregidas

### SQL Injection

Se eliminaron consultas vulnerables mediante el uso de Entity Framework Core y consultas LINQ parametrizadas.

### Contraseñas en Texto Plano

Las contraseñas ya no se almacenan directamente en la base de datos. Se implementó hashing seguro mediante BCrypt.

### Autenticación Insegura

El proceso de autenticación valida las credenciales utilizando BCrypt.Verify() para comparar la contraseña ingresada con el hash almacenado.

### Cross-Site Scripting (XSS)

Se implementó codificación HTML de los datos mostrados al usuario para evitar la ejecución de scripts maliciosos.

### Insecure Direct Object Reference (IDOR)

Se agregaron validaciones de autorización para impedir que un usuario acceda a recursos pertenecientes a otros usuarios.

---

## Tecnologías Utilizadas

* ASP.NET Core
* C#
* Entity Framework Core
* SQL Server / LocalDB
* BCrypt.Net
* Git

---

## Requisitos Previos

Antes de ejecutar el proyecto es necesario contar con:

* .NET SDK 8.0 o superior
* Visual Studio 2022 o Visual Studio Code
* SQL Server Express o LocalDB
* Git

Verificar instalación de .NET:

```bash
dotnet --version
```

---

## Clonar el Proyecto

```bash
git clone <URL_DEL_REPOSITORIO>
cd VulnerableApp
```

Cambiar a la rama segura:

```bash
git checkout secure
```

---

## Restaurar Dependencias

```bash
dotnet restore
```

---

## Configuración de Base de Datos

Verificar la cadena de conexión dentro del archivo:

```text
appsettings.json
```

Ejemplo:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=VulnerableAppDB;Trusted_Connection=True;"
}
```

Aplicar migraciones:

```bash
dotnet ef database update
```

---

## Ejecutar la Aplicación

```bash
dotnet run
```

o desde Visual Studio:

```text
F5
```

La aplicación estará disponible en:

```text
https://localhost:xxxx
```

---

## Pruebas de Seguridad

Las siguientes pruebas fueron utilizadas para validar la remediación de vulnerabilidades:

### SQL Injection

Intentar búsquedas utilizando caracteres especiales:

```sql
' OR '1'='1
```

Resultado esperado:

```text
No se devuelven registros no autorizados.
```

### XSS

Ingresar:

```html
<script>alert('XSS')</script>
```

Resultado esperado:

```text
El contenido se muestra como texto y no se ejecuta.
```

### IDOR

Intentar acceder a otro identificador de usuario:

```http
GET /api/user/2
```

Resultado esperado:

```text
403 Forbidden
```

o

```text
401 Unauthorized
```

### Autenticación

Intentar iniciar sesión con credenciales incorrectas.

Resultado esperado:

```text
Credenciales inválidas.
```

---

## Estructura de Seguridad Implementada

| Control       | Implementación                      |
| ------------- | ----------------------------------- |
| SQL Injection | Entity Framework Core               |
| Contraseñas   | BCrypt                              |
| Autenticación | BCrypt.Verify()                     |
| XSS           | Html.Encode()                       |
| IDOR          | Validación de sesión y autorización |

---


