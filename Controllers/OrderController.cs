using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace VulnerableApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("Inicio OrderController.Index - Consulta de órdenes");
            
            // Simular lógica de negocio
            
            sw.Stop();
            _logger.LogInformation("Fin OrderController.Index - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
            return Ok("Order module is working.");
        }
    }
}
