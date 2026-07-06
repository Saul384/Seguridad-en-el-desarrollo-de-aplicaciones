using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Models;
using VulnerableApp.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VulnerableApp.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILogger<HomeController> _logger;

    public HomeController(AppDbContext db, ILogger<HomeController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public IActionResult Index(string search)
    {
        var sw = Stopwatch.StartNew();
        var user = HttpContext.Session.GetString("User") ?? "Anónimo";
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
        
        _logger.LogInformation("Inicio HomeController.Index - Usuario: {User} IP: {IP} Parámetros: search={Search}", user, ip, search);

        try
        {
            if (string.IsNullOrEmpty(search))
            {
                sw.Stop();
                _logger.LogInformation("Fin HomeController.Index - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                return View(new List<User>()); 
            }

            var users = _db.Users
                .Where(u => u.Username.Contains(search))
                .ToList();

            sw.Stop();
            _logger.LogInformation("Fin HomeController.Index - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
            return View(users);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error en HomeController.Index - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
            throw;
        }
    }

    public IActionResult Privacy()
    {
        var sw = Stopwatch.StartNew();
        var user = HttpContext.Session.GetString("User") ?? "Anónimo";
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
        
        _logger.LogInformation("Inicio HomeController.Privacy - Usuario: {User} IP: {IP}", user, ip);

        try
        {
            var result = View();
            sw.Stop();
            _logger.LogInformation("Fin HomeController.Privacy - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error en HomeController.Privacy - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
            throw;
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var sw = Stopwatch.StartNew();
        var user = HttpContext.Session.GetString("User") ?? "Anónimo";
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
        
        _logger.LogInformation("Inicio HomeController.Error - Usuario: {User} IP: {IP}", user, ip);

        try
        {
            var result = View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            sw.Stop();
            _logger.LogInformation("Fin HomeController.Error - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error en HomeController.Error - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
            throw;
        }
    }

    public IActionResult TestError()
    {
        throw new Exception("Excepción provocada intencionalmente para probar el Middleware");
    }
}