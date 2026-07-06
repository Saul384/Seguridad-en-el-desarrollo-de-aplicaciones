using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System;

namespace VulnerableApp.Controllers
{
    public class CommentController : Controller
    {
        // Lista estática para almacenar los comentarios en memoria
        private static List<string> _comments = new();
        private readonly ILogger<CommentController> _logger;

        public CommentController(ILogger<CommentController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var sw = Stopwatch.StartNew();
            var user = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            
            _logger.LogInformation("Inicio CommentController.Index - Usuario: {User} IP: {IP}", user, ip);

            try
            {
                var result = View(_comments);
                sw.Stop();
                _logger.LogInformation("Fin CommentController.Index - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                return result;
            }
            catch(Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Error en CommentController.Index - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(string comment)
        {
            var sw = Stopwatch.StartNew();
            var user = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
            
            _logger.LogInformation("Inicio CommentController.AddComment - Usuario: {User} IP: {IP} Parámetros: comment={Comment}", user, ip, comment);

            try
            {
                if (!string.IsNullOrEmpty(comment))
                {
                    _comments.Add(comment);
                }
                else
                {
                    _logger.LogWarning("Intento de agregar un comentario vacío - Usuario: {User} IP: {IP}", user, ip);
                }
                
                sw.Stop();
                _logger.LogInformation("Fin CommentController.AddComment - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Error en CommentController.AddComment - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}