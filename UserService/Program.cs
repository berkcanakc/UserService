using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.API.Middlewares;
using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Interfaces;
using UserService.Middleware;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        // 1. DbContext Bağlantısını Yapıyoruz
                        services.AddDbContext<AppDbContext>(options =>
                            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                        // 2. DI Konfigürasyonu: Service ve Repository'leri bağlayalım
                        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
                        services.AddScoped<IUserService, Application.Services.UserService>();
                        services.AddScoped<IUserRepository, Infrastructure.Repositories.UserRepository>();
                        services.AddScoped<IJwtService, JwtService>();

                        //// 3. Global Hata Yönetimi Middleware
                        //services.AddTransient<ErrorHandlingMiddleware>();

                        services.AddEndpointsApiExplorer();  // Add support for API Explorer
                        services.AddSwaggerGen();

                        // 4. Controller'ları ekliyoruz
                        services.AddControllers();
                    });

                    webBuilder.Configure(app =>
                    {
                        app.UseMiddleware<ErrorHandlingMiddleware>(); // Global Hata Yönetimi

                        app.UseSwagger(); // Swagger JSON
                        app.UseSwaggerUI(c =>
                        {
                            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService API V1"); // Set Swagger endpoint
                        });
                        // 5. Middleware'leri sırasıyla ekliyoruz

                        app.UseRouting(); // Routing işlemi
                        app.UseAuthorization(); // Authorization işlemi

                        // 6. API endpoint'leri için controller'ları yönlendiriyoruz
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                });
    }
}
