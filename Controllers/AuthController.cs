using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System;

namespace VulnerableApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext db, ILogger<AuthController> logger) 
        { 
            _db = db; 
            _logger = logger;
        }

        public IActionResult Login()
        {
            var sw = Stopwatch.StartNew();
            var user = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            
            _logger.LogInformation("Inicio AuthController.Login(GET) - Usuario: {User} IP: {IP}", user, ip);

            try
            {
                var result = View();
                sw.Stop();
                _logger.LogInformation("Fin AuthController.Login(GET) - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                return result;
            }
            catch(Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Error en AuthController.Login(GET) - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                throw;
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var sw = Stopwatch.StartNew();
            var currentUser = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            
            _logger.LogInformation("Inicio AuthController.Login(POST) - UsuarioActual: {User} IP: {IP} Parámetros: username={Username}", currentUser, ip, username);

            try
            {
                // 1. Buscamos al usuario de forma segura parametrizando con LINQ
                var user = _db.Users.FirstOrDefault(u => u.Username == username);
                
                // 2. Verificamos con BCrypt.Net que la contraseña coincida con el hash
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning("Evento de Autenticación: Fallo de login para el usuario '{Username}' desde IP: {IP}", username, ip);
                    ViewBag.Error = "Usuario/contraseña inválido";
                    sw.Stop();
                    _logger.LogInformation("Fin AuthController.Login(POST) - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                    return View();
                }
                
                // 3. Inicio de sesión exitoso y seguro
                _logger.LogInformation("Evento de Autenticación: Login exitoso para el usuario '{Username}' desde IP: {IP}", username, ip);
                HttpContext.Session.SetString("User", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);
                
                sw.Stop();
                _logger.LogInformation("Fin AuthController.Login(POST) - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                return RedirectToAction("Dashboard");
            }
            catch(Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Error en AuthController.Login(POST) al intentar autenticar a '{Username}' - TiempoEjecucion: {ElapsedMilliseconds} ms", username, sw.ElapsedMilliseconds);
                throw;
            }
        }

        public IActionResult Dashboard()
        {
            var sw = Stopwatch.StartNew();
            var currentUser = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            
            _logger.LogInformation("Inicio AuthController.Dashboard - Usuario: {User} IP: {IP}", currentUser, ip);

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue) 
                {
                    _logger.LogWarning("Intento de acceso no autorizado a Dashboard desde IP: {IP}", ip);
                    sw.Stop();
                    _logger.LogInformation("Fin AuthController.Dashboard - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                    return RedirectToAction("Login");
                }
                
                var user = _db.Users.Find(userId.Value);
                sw.Stop();
                _logger.LogInformation("Fin AuthController.Dashboard - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                return View(user);
            }
            catch(Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Error en AuthController.Dashboard - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                throw;
            }
        }

        public IActionResult Logout()
        {
            var sw = Stopwatch.StartNew();
            var currentUser = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            
            _logger.LogInformation("Inicio AuthController.Logout - Usuario: {User} IP: {IP}", currentUser, ip);

            try
            {
                _logger.LogInformation("Evento de Autenticación: Logout para el usuario '{User}' desde IP: {IP}", currentUser, ip);
                HttpContext.Session.Clear();
                
                sw.Stop();
                _logger.LogInformation("Fin AuthController.Logout - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                return RedirectToAction("Index", "Home");
            }
            catch(Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Error en AuthController.Logout - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}