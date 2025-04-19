using UserService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Interfaces;

namespace UserService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;  // Şifre güvenliği için

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        // Kullanıcıyı ID'ye göre alır
        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.IsDeleted)
            {
                throw new ArgumentException("User not found");
            }

            return user;
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return user;  // Doğrulandıysa kullanıcıyı döndürüyoruz
        }

        // Tüm kullanıcıları alır (silinmiş kullanıcılar hariç)
        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Where(u => !u.IsDeleted).ToList();  // Silinmiş kullanıcılar hariç
        }

        // Yeni kullanıcı oluşturur
        public async Task CreateUserAsync(User user)
        {
            // E-posta kontrolü
            if (await _userRepository.EmailExistsAsync(user.Email))
            {
                throw new ArgumentException("Email already exists.");
            }

            // Şifreyi hash'leyerek güvenli bir şekilde saklarız
            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;

            await _userRepository.CreateUserAsync(user);
        }

        // Kullanıcıyı günceller
        public async Task UpdateUserAsync(User user)
        {
            if (user.UserId <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            await _userRepository.UpdateUserAsync(user);
        }

        // Kullanıcıyı mantıksal olarak siler (isDeleted = true)
        public async Task DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.IsDeleted)
            {
                throw new ArgumentException("User not found or already deleted.");
            }

            user.IsDeleted = true;  // Mantıksal silme
            await _userRepository.UpdateUserAsync(user);
        }
    }

}
