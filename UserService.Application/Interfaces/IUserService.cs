using UserService.Domain.Entities;

namespace UserService.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<User> AuthenticateAsync(string email, string password);
        Task DeleteUserAsync(int userId);  // Mantıksal silme işlemi

    }
}
