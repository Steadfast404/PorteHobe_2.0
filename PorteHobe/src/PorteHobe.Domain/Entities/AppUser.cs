using Microsoft.AspNetCore.Identity;
using Portehobe.src.PorteHobe.Domain.Entities;
using PorteHobe.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Portehobe.Model
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //Relationships: A user can have multiple terms and todo items
        public ICollection<Term> Terms { get; set; } = new List<Term>();
        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
        public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
    }
}
