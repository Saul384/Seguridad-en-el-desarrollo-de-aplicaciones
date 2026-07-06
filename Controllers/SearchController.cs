using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VulnerableApp.Data;
using VulnerableApp.Models;
using System.Diagnostics;
using System;
using System.Linq;
using System.Collections.Generic;

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
            var sw = Stopwatch.StartNew();
            var user = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            
            _logger.LogInformation("Inicio SearchController.Index - Usuario: {User} IP: {IP} Ruta: {Route} Parámetros: search={Search}", 
                user, ip, HttpContext.Request.Path, search);

            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    sw.Stop();
                    _logger.LogInformation("Fin SearchController.Index - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                    return View(new List<User>());
                }
                
                // LA VULNERABILIDAD: Concatenación directa de cadenas
                string query = "SELECT * FROM Users WHERE Username LIKE '%" + search + "%'";
                var users = _db.Users.FromSqlRaw(query).ToList();
                
                sw.Stop();
                _logger.LogInformation("Fin SearchController.Index - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                return View(users);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Error en SearchController.Index (posible inyección SQL maliciosa) - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}