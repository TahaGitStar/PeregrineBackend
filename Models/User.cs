using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeregrineBackend.API.Models
{
    /// <summary>
    /// يمثل مستخدم النظام
    /// </summary>
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; }

        public string? ProfileImageUrl { get; set; }

        public DateTime LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        // العلاقات
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }

    /// <summary>
    /// يمثل دور المستخدم وصلاحياته
    /// </summary>
    public class UserRole
    {
        [Key]
        public int Id { get; set; }

        public Guid UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }

        public string Permissions { get; set; } // JSON string

        // العلاقات
        public virtual User User { get; set; }
    }
}
