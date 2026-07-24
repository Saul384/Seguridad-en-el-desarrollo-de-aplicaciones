using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace VulnerableApp.Controllers
{
    public class PedidoController : Controller
    {
        private readonly ILogger<PedidoController> _logger;

        public PedidoController(ILogger<PedidoController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("Inicio PedidoController.Index - Consulta de pedidos");
            
            // Simular lógica de negocio
            
            sw.Stop();
            _logger.LogInformation("Fin PedidoController.Index - TiempoEjecucion: {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);
            return Ok("Pedido module is working.");
        }
    }
}
