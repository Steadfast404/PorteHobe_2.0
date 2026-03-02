using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;

namespace Portehobe.Model
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
