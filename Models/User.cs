using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Models
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            Orders = new HashSet<Order>();
            Ratings = new HashSet<Rating>();
        }

        public int Id { get; set; }
        public int RoleId { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = null!;
        [Required]
        public string Address { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [NotMapped]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
