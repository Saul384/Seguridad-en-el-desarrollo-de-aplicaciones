using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Diagnostics;
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
            // 1. CorrelationId
            var cid = Guid.NewGuid().ToString();
            context.Response.Headers["X-Correlation-ID"] = cid;

            // Empujar el CorrelationId al contexto de Serilog para que todos los logs de esta petición lo incluyan
            using (LogContext.PushProperty("CorrelationId", cid))
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    // 2. Exception Middleware
                    _logger.LogError(ex, "Unhandled Exception");
                    context.Response.StatusCode = 500;
                }
                finally
                {
                    sw.Stop();
                    
                    // 3. Request Logging
                    _logger.LogInformation(
                        "HTTP {Method} {Path} respondió {StatusCode} en {ElapsedMilliseconds} ms",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        sw.ElapsedMilliseconds);
                }
            }
        }
    }
}
