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
                // Hata loglanır
                _logger.LogError($"An error occurred: {ex.Message}");

                // Hata kodu ve mesajı ayarlanır
                context.Response.StatusCode = 500;  // 500 Internal Server Error
                context.Response.ContentType = "application/json";  // JSON olarak dönüyoruz

                // Hata mesajı döndürülür
                var errorResponse = new { message = "An unexpected error occurred." };
                var jsonResponse = JsonSerializer.Serialize(errorResponse);  // JSON formatına dönüştürme
                await context.Response.WriteAsync(jsonResponse);  // JSON olarak response yazılır
            }
        }
    }

}
