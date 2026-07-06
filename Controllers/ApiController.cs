using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using VulnerableApp.Data;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System;

namespace VulnerableApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ApiController> _logger;

        public ApiController(AppDbContext db, ILogger<ApiController> logger) 
        { 
            _db = db; 
            _logger = logger;
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            var sw = Stopwatch.StartNew();
            var user = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            
            _logger.LogInformation("Inicio ApiController.GetUser - Usuario: {User} IP: {IP} Parámetros: id={Id}", user, ip, id);

            try
            {
                // 1. Validación de autenticación: verifica si hay una sesión activa
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (!currentUserId.HasValue) 
                {
                    _logger.LogWarning("Intento de acceso a API no autorizado (sin sesión) - Usuario: {User} IP: {IP}", user, ip);
                    sw.Stop();
                    _logger.LogInformation("Fin ApiController.GetUser - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                    return Unauthorized();
                }

                // 2. Validación de autorización (Ownership mitigando IDOR): bloquea si intenta ver otro ID
                if (id != currentUserId.Value) 
                {
                    _logger.LogWarning("Intento de acceso IDOR detectado. Usuario '{User}' (ID:{CurrentUserId}) intentó acceder al ID:{RequestedId} desde IP: {IP}", user, currentUserId.Value, id, ip);
                    sw.Stop();
                    _logger.LogInformation("Fin ApiController.GetUser - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                    return Forbid();
                }

                var userDb = _db.Users.Find(id);
                if (userDb == null) 
                {
                    sw.Stop();
                    _logger.LogInformation("Fin ApiController.GetUser (No encontrado) - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                    return NotFound();
                }

                // 3. Omitimos PasswordHash y Balance para evitar exposición de datos sensibles
                var result = Ok(new 
                { 
                    userDb.Id, 
                    userDb.Username, 
                    userDb.Email 
                });
                
                sw.Stop();
                _logger.LogInformation("Fin ApiController.GetUser - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                return result;
            }
            catch(Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Error en ApiController.GetUser - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                throw;
            }
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var sw = Stopwatch.StartNew();
            var user = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            
            _logger.LogInformation("Inicio ApiController.GetAllUsers - Usuario: {User} IP: {IP}", user, ip);

            try
            {
                // Proyectamos únicamente información pública para evitar la fuga masiva de datos
                var users = _db.Users.Select(u => new 
                {
                    u.Id,
                    u.Username
                }).ToList();
                
                var result = Ok(users);
                sw.Stop();
                _logger.LogInformation("Fin ApiController.GetAllUsers - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                return result;
            }
            catch(Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Error en ApiController.GetAllUsers - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}