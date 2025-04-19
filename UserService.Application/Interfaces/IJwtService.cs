using UserService.Domain.Entities;

namespace UserService.Application.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);  // Token oluşturma
        bool ValidateToken(string token);    // Token doğrulama
    }
}
