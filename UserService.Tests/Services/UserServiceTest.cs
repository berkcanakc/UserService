using Moq;
using System;
using System.Threading.Tasks;
using UserService.Application.Services;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using Xunit;
using Microsoft.AspNetCore.Identity;
using UserService.Infrastructure.Interfaces;

namespace UserService.Tests
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly Application.Services.UserService _userService;

        public UserServiceTest()
        {
            // Mock nesnelerini başlatıyoruz
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _userService = new Application.Services.UserService(_userRepositoryMock.Object, _passwordHasherMock.Object);
        }

        #region CreateUserAsync Testi
        [Fact]
        public async Task CreateUserAsync_Should_HashPassword_And_SaveUser()
        {
            // Arrange: Test verisi hazırlıyoruz
            var user = new User
            {
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "password123",
                CreatedAt = DateTime.UtcNow
            };

            // Mocking metodlar
            _userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _passwordHasherMock.Setup(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>())).Returns("hashedPassword");

            // Act: Metodu çağırıyoruz
            await _userService.CreateUserAsync(user);

            // Assert: HashPassword metodunun bir kez çağrıldığını doğruluyoruz
            _passwordHasherMock.Verify(p => p.HashPassword(It.IsAny<User>(), It.IsAny<string>()), Times.Once);

            // CreateUserAsync metodunun bir kez çağrıldığını doğruluyoruz
            _userRepositoryMock.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
        }
        #endregion

        #region AuthenticateAsync Testi
        [Fact]
        public async Task AuthenticateAsync_Should_Return_User_When_Credentials_Are_Valid()
        {
            // Arrange: Geçerli kullanıcı verisi
            var user = new User
            {
                UserId = 1,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedPassword",
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _passwordHasherMock.Setup(p => p.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                               .Returns(PasswordVerificationResult.Success);

            // Act: Kullanıcı doğrulaması
            var result = await _userService.AuthenticateAsync("test@example.com", "password123");

            // Assert: Sonuç doğrulaması
            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
        }

        [Fact]
        public async Task AuthenticateAsync_Should_Throw_UnauthorizedException_When_Credentials_Are_Invalid()
        {
            // Arrange: Geçersiz kullanıcı
            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            // Act & Assert: Geçersiz bilgilerle giriş yapılmaya çalışıldığında hata fırlatılmalı
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.AuthenticateAsync("test@example.com", "wrongpassword"));
        }
        #endregion

        #region DeleteUserAsync (Soft Delete) Testi
        [Fact]
        public async Task DeleteUserAsync_Should_Soft_Delete_User()
        {
            // Arrange: Silinecek kullanıcı
            var user = new User
            {
                UserId = 1,
                UserName = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedPassword",
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

            // Act: Kullanıcı siliniyor
            await _userService.DeleteUserAsync(user.UserId);

            // Assert: Silme işlemi sonrasında IsDeleted alanının true olduğunu doğruluyoruz
            Assert.True(user.IsDeleted);
            _userRepositoryMock.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Once);  // Kullanıcı güncellenmeli
        }
        #endregion
    }
}
