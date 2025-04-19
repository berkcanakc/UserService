using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Entities
{
    public enum UserRole
    {
        Admin,
        User
    }

    public class User
    {
        public int UserId { get; set; }

        [Required]  // Bu alan null olamaz
        [MaxLength(100)] // Maksimum uzunluk belirleme
        public required string UserName { get; set; }

        [Required]  // Bu alan null olamaz
        [EmailAddress] // Email formatı kontrolü
        public required string Email { get; set; }

        [Required]  // Bu alan null olamaz
        public UserRole Role { get; set; }  // Admin veya User

        [Required]  // Bu alan null olamaz
        public required string PasswordHash { get; set; }

        [Required]  // Bu alan null olamaz
        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; } = false; // Varsayılan değer false olacak
    }
}
