using Microsoft.AspNetCore.Mvc;
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
            var user = _db.Users.Find(id);
            if (user == null) return NotFound();

            // Retorna un objeto anónimo con propiedades sensibles
            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Balance,
                user.Password
            });
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            // Retorna toda la tabla sin filtros
            var users = _db.Users.ToList();
            return Ok(users);
        }
    }
}