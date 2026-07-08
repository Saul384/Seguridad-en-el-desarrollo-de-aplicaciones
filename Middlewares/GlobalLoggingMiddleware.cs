using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace VulnerableApp.Middlewares
{
    public class GlobalLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalLoggingMiddleware> _logger;

        public GlobalLoggingMiddleware(RequestDelegate next, ILogger<GlobalLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // CorrelationId Middleware logic
            var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            context.TraceIdentifier = correlationId;

            var stopwatch = Stopwatch.StartNew();

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                try
                {
                    _logger.LogInformation("Iniciando petición HTTP {Method} {Path}", context.Request.Method, context.Request.Path);

                    await _next(context);

                    stopwatch.Stop();
                    _logger.LogInformation("Petición HTTP {Method} {Path} completada en {ElapsedMilliseconds} ms con código {StatusCode}", 
                        context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds, context.Response.StatusCode);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    // Exception Middleware logic
                    _logger.LogError(ex, "Excepción no controlada en HTTP {Method} {Path} después de {ElapsedMilliseconds} ms", 
                        context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
                    
                    throw;
                }
            }
        }
    }
}
