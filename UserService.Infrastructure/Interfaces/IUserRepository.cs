﻿using UserService.Domain.Entities;

namespace UserService.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email); // E-posta var mı kontrolü
    }
}
