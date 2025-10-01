using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UserService.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);  // Sonraki middleware çalıştırılır
            }
            catch (Exception ex)
            {
                // Inner exception varsa onu da alalım
                var detailedMessage = ex.InnerException?.Message ?? ex.Message;

                // Log'a detaylı mesajı bas
                _logger.LogError(ex, "An error occurred: {Message}", detailedMessage);

                // Response ayarı
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                // Kullanıcıya JSON hata dön
                var errorResponse = new { message = detailedMessage };
                var jsonResponse = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }

}
