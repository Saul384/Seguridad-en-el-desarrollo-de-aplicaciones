using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Models;
using VulnerableApp.Data;

namespace VulnerableApp.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index(string search)
    {
        if (string.IsNullOrEmpty(search))
            return View(new List<User>()); 

        // LINQ evita la inyección SQL parametrizando la variable 'search'
        var users = _db.Users
            .Where(u => u.Username.Contains(search))
            .ToList();

        return View(users);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}