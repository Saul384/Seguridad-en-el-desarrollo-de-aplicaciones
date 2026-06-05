using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using VulnerableApp.Data;
using System.Linq;

namespace VulnerableApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ApiController(AppDbContext db) 
        { 
            _db = db; 
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            // 1. Validación de autenticación: verifica si hay una sesión activa
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!currentUserId.HasValue) return Unauthorized();

            // 2. Validación de autorización (Ownership mitigando IDOR): bloquea si intenta ver otro ID
            if (id != currentUserId.Value) return Forbid();

            var user = _db.Users.Find(id);
            if (user == null) return NotFound();

            // 3. Omitimos PasswordHash y Balance para evitar exposición de datos sensibles
            return Ok(new 
            { 
                user.Id, 
                user.Username, 
                user.Email 
            });
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            // Proyectamos únicamente información pública para evitar la fuga masiva de datos
            var users = _db.Users.Select(u => new 
            {
                u.Id,
                u.Username
            }).ToList();
            
            return Ok(users);
        }
    }
}