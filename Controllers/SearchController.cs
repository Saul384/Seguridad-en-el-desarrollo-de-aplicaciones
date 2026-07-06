using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VulnerableApp.Data;
using VulnerableApp.Models;

namespace VulnerableApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SearchController> _logger;
        
        public SearchController(AppDbContext db,
            ILogger<SearchController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index(string search)
        {
            _logger.LogInformation(
                "Entrando a Search.Index");
            
            _logger.LogInformation(
                "Usuario:{User} IP:{IP} Ruta:{Route}",
                HttpContext.Session.GetString("User"),
                HttpContext.Connection.RemoteIpAddress,
                HttpContext.Request.Path);

            if (string.IsNullOrEmpty(search))
                return View(new List<User>());
            
            // LA VULNERABILIDAD: Concatenación directa de cadenas
            string query = "SELECT * FROM Users WHERE Username LIKE '%" + search + "%'";
            var users = _db.Users.FromSqlRaw(query).ToList();
            
            return View(users);
        }
    }
}