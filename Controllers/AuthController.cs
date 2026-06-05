using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data;
using System.Linq;

namespace VulnerableApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        public AuthController(AppDbContext db) { _db = db; }

        public IActionResult Login() => View();
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            // 1. Buscamos al usuario de forma segura parametrizando con LINQ
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            
            // 2. Verificamos con BCrypt.Net que la contraseña coincida con el hash
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ViewBag.Error = "Usuario/contraseña inválido";
                return View();
            }
            
            // 3. Inicio de sesión exitoso y seguro
            HttpContext.Session.SetString("User", user.Username);
            HttpContext.Session.SetInt32("UserId", user.Id);
            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction("Login");
            var user = _db.Users.Find(userId.Value);
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}