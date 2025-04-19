using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Interfaces;

namespace UserService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Kullanıcıyı ID ile alır
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);  // Silinmiş kullanıcıları hariç tutuyoruz
        }

        // Tüm kullanıcıları alır
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                                 .Where(u => !u.IsDeleted)  // Silinmiş kullanıcıları hariç tutuyoruz
                                 .ToListAsync();
        }

        // Yeni bir kullanıcı ekler
        public async Task CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // Kullanıcıyı günceller
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // E-posta adresinin veritabanında var olup olmadığını kontrol et
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                                 .AnyAsync(u => u.Email == email && !u.IsDeleted);  // Silinmiş e-posta adresini kontrol etmiyoruz
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                                 .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);  // E-posta ile kullanıcıyı alıyoruz
        }
    }
}
