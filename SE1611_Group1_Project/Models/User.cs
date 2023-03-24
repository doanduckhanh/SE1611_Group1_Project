using System;
using System.Collections.Generic;

namespace SE1611_Group1_Project.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual Cart? Cart { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
